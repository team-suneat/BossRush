# CharacterAnimator 클래스 고급 리팩토링 문서

## 목차
1. [개요](#개요)
2. [현재 구조의 근본적 문제점](#현재-구조의-근본적-문제점)
3. [고급 리팩토링 제안](#고급-리팩토링-제안)
4. [구현 계획](#구현-계획)
5. [기본 리팩토링 vs 고급 리팩토링 비교](#기본-리팩토링-vs-고급-리팩토링-비교)

---

## 개요

> **참고**: 이 문서는 **기본 리팩토링을 완료한 후** 진행하는 고급 리팩토링 제안입니다.  
> 기본 리팩토링은 partial class 내부에서 구조를 개선하는 방향이고,  
> 이 문서는 **더 근본적인 아키텍처 개선**을 제안하며, 별도 클래스로 분리하여 단일 책임 원칙을 강화하는 방향입니다.

**기본 리팩토링 문서:** [REFACTORING.md](./REFACTORING.md)

---

## 현재 구조의 근본적 문제점

### 단일 책임 원칙 위반
`CharacterAnimator`가 다음을 모두 담당:
- 애니메이터 파라미터 초기화 및 SetXXX 메서드들
- 공격 전용 로직 (시퀀스 공격, 이동락/플립락, 쿨다운 이벤트)
- 패링 전용 로직 (패링 트리거, 성공 여부, IsParrying 플래그)
- 이동락/Coroutines, 대미지/대시/스폰/죽음 상태에서의 LockMovement/UnlockMovement
- Animator StateMachineHook (OnAnimatorStateEnter/Exit에서 문자열 기반 분기)

### 상태 플래그 중복
- `IsAttacking`, `IsDashing`, `IsDamaging`, `IsParrying` 등 bool 필드와 Animator bool 파라미터가 각각 존재
- 양방향 동기화를 사람이 보장해야 함 (실수 가능성 높음)

### 문자열 기반 상태 분기
- `CheckStateName/CheckStateNames`로 "Spawn, Dash, Damage, Parry..." 문자열 비교를 매 프레임 Animator State 콜백에서 수행
- 성능 저하 및 오타 위험

### 플로우가 숨겨짐
- 공격 → 이동락 → 대미지 블록 → 쿨다운 이벤트 순서를 이해하려면 여러 partial 파일을 오가야 함

---

## 고급 리팩토링 제안

### 제안 1: 기능 모듈 분리 (전략/서비스 객체)

**목표:** partial 분리를 넘어 실제 타입 분리로 단일 책임 원칙 강화

**구현 방향:**

#### 1.1 전투 관련 모듈

**`CharacterAttackAnimator`** (별도 클래스)
- 책임: 공격/시퀀스 공격/쿨다운/대미지 차단
- 포함 내용:
  - `PlayAttackAnimation()`, `PlaySequenceAttackAnimation()`
  - 공격 중 이동락/플립락 제어
  - 쿨다운 이벤트 관리 (`RefreshAttackCooldown`)
  - 대미지 애니메이션 차단 (`IsBlockingDamageAnimationWhileAttack`)

**`CharacterParryAnimator`** (별도 클래스)
- 책임: 패링, 패링 성공/실패
- 포함 내용:
  - `PlayParryAnimation()`
  - `SetParrySuccess()`
  - `IsParrying` 상태 관리

#### 1.2 이동/락 관련 모듈

**`CharacterMovementLockController`** (별도 클래스)
- 책임: 이동 잠금 관리
- 포함 내용:
  - `IsMovementLocked` 상태
  - `LockMovement()`, `UnlockMovement()`
  - 코루틴 기반 자동 잠금해제

#### 1.3 상태/파라미터 관리 모듈

**`CharacterAnimationStateSync`** (별도 클래스)
- 책임: C# 상태와 Animator 파라미터 동기화
- 포함 내용:
  - `IsAttacking`, `IsDamaging`, `IsDashing` 등 C# 필드 관리
  - Animator bool 파라미터와의 자동 동기화
  - "C# 필드 = 소스 오브 트루스" 규칙 적용

**`AnimatorParameterRegistry`** (별도 클래스)
- 책임: 이름–ID 관리, 파라미터 존재 체크
- 포함 내용:
  - `Dictionary<string, AnimatorParam>` 또는 `struct AnimatorParam { int Id; AnimatorControllerParameterType Type; }`
  - `TryGet(string name, out AnimatorParam param)`
  - `Initialize(Animator animator)`에서 `AddAnimatorParameterIfExists` 호출

#### 1.4 CharacterAnimator의 역할

`CharacterAnimator`는 위 모듈들을 조합하는 **파사드(Facade)** 역할만 담당:
- 공개 API는 최소한으로 유지
- 내부적으로 모듈들을 조합하여 동작
- 기존 코드와의 호환성 유지

**예시 구조:**
```csharp
public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
{
    private CharacterAttackAnimator _attackAnimator;
    private CharacterParryAnimator _parryAnimator;
    private CharacterMovementLockController _movementLockController;
    private CharacterAnimationStateSync _stateSync;
    private AnimatorParameterRegistry _parameterRegistry;

    private void Awake()
    {
        _animator ??= GetComponent<Animator>();
        _owner ??= this.FindFirstParentComponent<Character>();
        
        _parameterRegistry = new AnimatorParameterRegistry();
        _parameterRegistry.Initialize(_animator);
        
        _stateSync = new CharacterAnimationStateSync(_animator, _parameterRegistry);
        _attackAnimator = new CharacterAttackAnimator(_animator, _parameterRegistry, _stateSync, _movementLockController);
        _parryAnimator = new CharacterParryAnimator(_animator, _parameterRegistry, _stateSync);
        _movementLockController = new CharacterMovementLockController();
    }

    // 파사드 메서드
    public void PlayAttackAnimation(string animationName)
    {
        _attackAnimator.PlayAttackAnimation(animationName);
    }

    public bool IsAttacking => _stateSync.IsAttacking;
}
```

**장점:**
- 각 모듈이 독립적으로 테스트 가능
- 모듈별 책임이 명확함
- 재사용성 향상 (다른 캐릭터 타입에서도 사용 가능)

**단점:**
- 초기 구현 비용 증가
- 기존 코드와의 호환성 유지 필요 (파사드 패턴으로 해결)

---

### 제안 2: AnimatorParameterRegistry 별도 클래스 도입

**목표:** 파라미터 관리 로직을 완전히 분리하여 CharacterAnimator를 가볍게 만들기

**구현:**

```csharp
public sealed class AnimatorParameterRegistry
{
    private struct AnimatorParam
    {
        public int Id;
        public AnimatorControllerParameterType Type;
        public bool IsValid => Id != 0;
    }

    private Dictionary<string, AnimatorParam> _parameters = new Dictionary<string, AnimatorParam>();
    private HashSet<int> _validParameterIds = new HashSet<int>();

    public void Initialize(Animator animator)
    {
        _parameters.Clear();
        _validParameterIds.Clear();

        // 기존 InitializeAnimatorParameters 로직을 여기로 이동
        AddParameterIfExists(animator, "IsSpawning", AnimatorControllerParameterType.Bool);
        AddParameterIfExists(animator, "IsSpawned", AnimatorControllerParameterType.Bool);
        // ... 나머지 파라미터들
    }

    private void AddParameterIfExists(Animator animator, string name, AnimatorControllerParameterType type)
    {
        if (animator.AddAnimatorParameterIfExists(name, out int id, type, _validParameterIds))
        {
            _parameters[name] = new AnimatorParam { Id = id, Type = type };
        }
    }

    public bool TryGet(string name, out AnimatorParam param)
    {
        return _parameters.TryGetValue(name, out param);
    }

    public bool TryGetId(string name, out int id)
    {
        if (_parameters.TryGetValue(name, out var param))
        {
            id = param.Id;
            return param.IsValid;
        }
        id = 0;
        return false;
    }

    public HashSet<int> GetValidParameterIds() => _validParameterIds;
}

// CharacterAnimator에서 사용
public partial class CharacterAnimator
{
    private AnimatorParameterRegistry _parameterRegistry;

    public void SetIsGrounded(bool isGrounded)
    {
        if (_parameterRegistry.TryGetId("IsGrounded", out int id))
        {
            _animator.UpdateAnimatorBool(id, isGrounded, _parameterRegistry.GetValidParameterIds());
        }
    }

    // 또는 더 추상화된 메서드
    public void SetBool(string parameterName, bool value)
    {
        if (_parameterRegistry.TryGetId(parameterName, out int id))
        {
            _animator.UpdateAnimatorBool(id, value, _parameterRegistry.GetValidParameterIds());
        }
    }
}
```

**장점:**
- const string, int 필드들이 CharacterAnimator에서 사라져 클래스가 가벼워짐
- 파라미터 추가/삭제 시 변경 지점이 한 곳으로 모임
- 파라미터 관리 로직 재사용 가능

**기존 제안과의 차이:**
- 기본 리팩토링: 구조체로 그룹화 (partial class 내부)
- 고급 리팩토링: 완전히 별도 클래스로 분리

---

### 제안 3: 상태 플래그와 Animator 파라미터 동기화 규칙 정리

**목표:** C# 필드와 Animator 파라미터의 불일치를 제거

**규칙: "C# 필드 = 소스 오브 트루스"**

- C# 필드(`IsAttacking` 등)를 소스 오브 트루스로 정함
- Animator 파라미터는 항상 헬퍼를 통해 C# 필드에서 동기화
- 한 곳에서만 상태 변경 가능하도록 제한

**구현:**

```csharp
public class CharacterAnimationStateSync
{
    private Animator _animator;
    private AnimatorParameterRegistry _parameterRegistry;

    // C# 필드 (소스 오브 트루스)
    private bool _isAttacking;
    private bool _isDamaging;
    private bool _isDashing;
    private bool _isParrying;

    // 읽기 전용 프로퍼티
    public bool IsAttacking => _isAttacking;
    public bool IsDamaging => _isDamaging;
    public bool IsDashing => _isDashing;
    public bool IsParrying => _isParrying;

    public void SetIsAttacking(bool value)
    {
        if (_isAttacking == value) return;
        
        _isAttacking = value;
        SyncToAnimator("IsAttacking", value);
    }

    public void SetIsDamaging(bool value)
    {
        if (_isDamaging == value) return;
        
        _isDamaging = value;
        SyncToAnimator("IsDamaging", value);
    }

    // 동일한 패턴을 IsDashing, IsParrying 등에도 적용
    private void SyncToAnimator(string parameterName, bool value)
    {
        if (_parameterRegistry.TryGetId(parameterName, out int id))
        {
            _animator.UpdateAnimatorBool(id, value, _parameterRegistry.GetValidParameterIds());
        }
    }

    public void ResetAll()
    {
        SetIsAttacking(false);
        SetIsDamaging(false);
        SetIsDashing(false);
        SetIsParrying(false);
    }
}
```

**사용 예시:**
```csharp
// 공격 모듈에서 사용
public class CharacterAttackAnimator
{
    private CharacterAnimationStateSync _stateSync;

    protected virtual void OnAnimatorAttackStateEnter()
    {
        _stateSync.SetIsAttacking(true); // 한 곳에서만 상태 변경
    }

    protected virtual void OnAnimatorAttackStateExit()
    {
        _stateSync.SetIsAttacking(false);
    }
}
```

**장점:**
- 상태 변경 지점이 명확함
- Animator와 C# 필드의 불일치 방지
- 동기화 로직이 한 곳에 집중

**기존 제안과의 차이:**
- 기본 리팩토링: 구조체로 그룹화 + 내부 메서드로 제한
- 고급 리팩토링: 별도 클래스로 분리 + 자동 동기화 보장

---

### 제안 4: 문자열 기반 StateMachine 분기 개선 (고급)

**목표:** 문자열 비교를 제거하고 성능 및 타입 안정성 향상

#### 4.1 Enum과 nameHash 기반 매핑

**작은 개선:**
```csharp
public enum AnimationStateId
{
    None = 0,
    Spawn,
    Dash,
    DashEnd,
    Damage,
    DamageGround,
    Stun,
    Phase2,
    Phase3,
    Phase4,
    Parry,
    ParrySuccess
}

public partial class CharacterAnimator
{
    private Dictionary<int, AnimationStateId> _stateHashToId = new Dictionary<int, AnimationStateId>();

    private void InitializeStateHandlers()
    {
        // AnimatorStateInfo.nameHash를 미리 매핑
        _stateHashToId[Animator.StringToHash("Spawn")] = AnimationStateId.Spawn;
        _stateHashToId[Animator.StringToHash("Dash")] = AnimationStateId.Dash;
        // ... 나머지 상태들

        // 핸들러를 enum 기반으로 매핑
        _stateEnterHandlers = new Dictionary<AnimationStateId, StateHandler>
        {
            { AnimationStateId.Spawn, (info, layer) => OnAnimatorSpawnStateEnter() },
            { AnimationStateId.Dash, (info, layer) => OnAnimatorDashStateEnter() },
            // ...
        };
    }

    public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // nameHash로 조회 (문자열 비교 없음)
        if (_stateHashToId.TryGetValue(stateInfo.fullPathHash, out var stateId))
        {
            if (_stateEnterHandlers.TryGetValue(stateId, out var handler))
            {
                handler(stateInfo, layerIndex);
            }
        }

        OnStateEnter?.Invoke(animator, stateInfo, layerIndex);
    }
}
```

**장점:**
- 문자열 비교 제거로 성능 향상
- enum 사용으로 타입 안정성 향상
- 오타 위험 제거

#### 4.2 StateMachineBehaviour 활용 (더 나아가)

각 애니메이터 상태에 직접 스크립트를 붙이고, 그 안에서 `CharacterAnimator`의 API 호출:

```csharp
// 각 상태별 StateMachineBehaviour
public class SpawnStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var characterAnimator = animator.GetComponentInParent<CharacterAnimator>();
        characterAnimator?.OnSpawnStateEnter();
    }
}

public class DashStateBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var characterAnimator = animator.GetComponentInParent<CharacterAnimator>();
        characterAnimator?.OnDashStateEnter();
    }
}
```

**장점:**
- `CharacterAnimator`는 "상태 변화 시 호출되는 퍼블릭 메서드"만 제공
- state 이름 문자열 의존이 완전히 제거됨
- Unity 에디터에서 직접 연결 가능 (시각적 관리)

**단점:**
- 각 상태에 스크립트를 붙여야 함 (작업량 증가)
- 기존 Animator Controller 수정 필요

**기존 제안과의 차이:**
- 기본 리팩토링: 딕셔너리 기반 문자열 매핑
- 고급 리팩토링: enum + nameHash 사용 또는 StateMachineBehaviour 활용

---

### 제안 5: 공격/패링/이동락 로직의 정책화

**목표:** 인터페이스 기반 정책 패턴으로 재사용성 및 확장성 향상

**구현:**

```csharp
// 이동락 정책 인터페이스
public interface IMovementLockPolicy
{
    bool ShouldLockOnAttack(string attackName);
    bool ShouldLockOnParry();
    float GetLockDuration(string attackName);
}

// 공격 시퀀스 정책 인터페이스
public interface IAttackSequencePolicy
{
    string GetCompleteStateName(string baseName);
    bool CanCancelAttack(string currentAttack);
    bool IsSequenceAttack(string attackName);
}

// 패링 결과 처리 인터페이스
public interface IParryResultHandler
{
    void OnParrySuccess();
    void OnParryFailure();
}

// 기본 구현
public class DefaultMovementLockPolicy : IMovementLockPolicy
{
    public bool ShouldLockOnAttack(string attackName) => true;
    public bool ShouldLockOnParry() => true;
    public float GetLockDuration(string attackName) => 0f; // 무한
}

// 보스 전용 정책
public class BossMovementLockPolicy : IMovementLockPolicy
{
    public bool ShouldLockOnAttack(string attackName)
    {
        // 특정 공격만 이동락
        return attackName.Contains("Charge");
    }
    // ...
}
```

**사용 예시:**
```csharp
public class CharacterAttackAnimator
{
    private IMovementLockPolicy _movementLockPolicy;
    private IAttackSequencePolicy _attackSequencePolicy;

    public void PlayAttackAnimation(string animationName)
    {
        // 정책에 따라 이동락 결정
        if (_movementLockPolicy.ShouldLockOnAttack(animationName))
        {
            _movementLockController.LockMovement();
        }
        // ...
    }
}
```

**장점:**
- 보스/플레이어/몹마다 다른 정책 주입 가능
- 정책 변경이 코드 수정 없이 가능
- 테스트 시 Mock 정책 주입 가능

**기존 제안과의 차이:**
- 기본 리팩토링: 하드코딩된 로직 (예: `DetermineMovementLockWhileAttack`)
- 고급 리팩토링: 인터페이스 기반 정책 패턴

---

## 구현 계획

### Phase A: AnimatorParameterRegistry 분리 (우선순위: 높음)
1. `AnimatorParameterRegistry` 클래스 생성
2. 파라미터 관리 로직 이동
3. `CharacterAnimator`에서 사용하도록 수정
4. 테스트 및 검증

**예상 작업 시간:** 2-3시간

### Phase B: CharacterAnimationStateSync 분리 (우선순위: 높음)
1. `CharacterAnimationStateSync` 클래스 생성
2. 상태 플래그 관리 및 동기화 로직 구현
3. 모든 상태 변경을 `StateSync`를 통해 하도록 수정
4. 테스트 및 검증

**예상 작업 시간:** 3-4시간

### Phase C: 전투 모듈 분리 (우선순위: 중간)
1. `CharacterAttackAnimator` 클래스 생성
2. `CharacterParryAnimator` 클래스 생성
3. 관련 로직 이동
4. `CharacterAnimator`를 파사드로 변경
5. 테스트 및 검증

**예상 작업 시간:** 4-5시간

### Phase D: 이동락 모듈 분리 (우선순위: 중간)
1. `CharacterMovementLockController` 클래스 생성
2. 관련 로직 이동
3. 테스트 및 검증

**예상 작업 시간:** 1-2시간

### Phase E: 상태 전환 개선 (우선순위: 낮음)
1. enum 기반 상태 ID 도입
2. nameHash 기반 매핑 구현
3. 또는 StateMachineBehaviour 활용
4. 테스트 및 검증

**예상 작업 시간:** 2-3시간

### Phase F: 정책 패턴 도입 (우선순위: 낮음)
1. 정책 인터페이스 정의
2. 기본 구현 생성
3. 모듈에 정책 주입
4. 테스트 및 검증

**예상 작업 시간:** 2-3시간

---

## 기본 리팩토링 vs 고급 리팩토링 비교

| 항목 | 기본 리팩토링 | 고급 리팩토링 |
|------|--------------|--------------|
| **접근 방식** | Partial class 내부 구조 개선 | 별도 클래스로 완전 분리 |
| **구현 난이도** | 낮음-중간 | 중간-높음 |
| **작업 시간** | 7-10시간 | 14-20시간 |
| **기존 코드 영향** | 적음 (내부 구조 변경) | 많음 (아키텍처 변경) |
| **테스트 용이성** | 중간 | 높음 (모듈별 독립 테스트) |
| **재사용성** | 낮음 | 높음 |
| **유지보수성** | 향상 | 크게 향상 |
| **권장 시점** | 즉시 적용 가능 | 기본 리팩토링 완료 후, 필요 시 적용 |

---

## 주의사항

### 고급 리팩토링 전 확인사항
1. **기본 리팩토링 완료**: 기본 리팩토링이 안정적으로 동작하는지 확인
2. **필요성 평가**: 고급 리팩토링의 필요성과 비용 대비 효과 평가
3. **팀 리소스**: 충분한 시간과 리소스 확보
4. **테스트 계획**: 각 Phase별 충분한 테스트 계획 수립

### 테스트 체크리스트
- [ ] 모든 모듈이 독립적으로 테스트 가능
- [ ] 파사드 패턴이 기존 API와 호환됨
- [ ] 상태 동기화가 정상 작동
- [ ] 정책 패턴이 올바르게 주입됨
- [ ] 성능 저하가 없음

---

**작성일:** 2024년
**작성자:** AI Assistant
**버전:** 1.0
