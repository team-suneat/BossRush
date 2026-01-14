# CharacterAnimator 클래스 기본 리팩토링 문서

## 목차
1. [현재 구조 분석](#현재-구조-분석)
2. [발견된 문제점](#발견된-문제점)
3. [리팩토링 제안](#리팩토링-제안)
4. [구현 계획](#구현-계획)
5. [우선순위](#우선순위)
6. [권장 진행 순서](#권장-진행-순서)

---

## 현재 구조 분석

### 파일 구조
```
CharacterAnimator/
├── CharacterAnimator.cs              (407 lines) - 메인 클래스 및 상태 관리
├── CharacterAnimator.Attack.cs       (150 lines) - 공격 애니메이션 로직
├── CharacterAnimator.Flip.cs         (35 lines)  - 반전 제어
├── CharacterAnimator.Movement.cs    (44 lines)  - 이동 잠금 관리
├── CharacterAnimator.Parameter.cs   (121 lines) - 애니메이터 파라미터 정의
├── CharacterAnimator.Parry.cs       (42 lines)  - 패리 애니메이션
├── CharacterAnimator.State.cs       (85 lines)  - 상태 플래그 관리
└── Log/
    └── CharacterAnimatorLog.cs       - 로깅 유틸리티
```

### 클래스 계층 구조
- `CharacterAnimator : XBehaviour, IAnimatorStateMachine`
- `CharacterAnimator`는 `partial class`로 여러 파일에 분리되어 있음
- `Character` 클래스의 자식 컴포넌트로 사용됨

### 주요 책임
1. **애니메이션 재생**: Spawn, Dash, Attack, Damage, Death, Parry 등
2. **상태 관리**: IsAttacking, IsDashing, IsDamaging 등 플래그 관리
3. **파라미터 관리**: Animator Controller 파라미터 설정 및 업데이트
4. **상태 전환 처리**: OnAnimatorStateEnter/Exit 이벤트 처리
5. **이동/반전 제어**: 공격 중 이동 잠금, 반전 제어

---

## 발견된 문제점

### 1. 애니메이터 파라미터 관리 복잡성
**문제:**
- 파라미터 이름과 ID가 분리되어 선언되어 중복 관리
- 파라미터 타입별로 그룹화되지 않아 가독성 저하
- 파라미터 초기화 로직이 길고 반복적임

**위치:**
```15:82:Assets/Scripts/Runtime/Character/Animator/CharacterAnimator.Parameter.cs
private const string ANIMATOR_IS_SPAWNING_PARAMETER_NAME = "IsSpawning";
// ... 많은 상수 선언
private int ANIMATOR_IS_SPAWNING_PARAMETER_ID;
// ... 많은 ID 선언
```

### 2. 상태 전환 로직 분산 및 복잡성
**문제:**
- `OnAnimatorStateEnter/Exit`에서 긴 if-else 체인 사용
- 상태별 핸들러가 여러 파일에 분산되어 있음
- 새로운 상태 추가 시 메서드 수정이 필요함

**위치:**
```209:241:Assets/Scripts/Runtime/Character/Animator/CharacterAnimator.cs
public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
{
    if (CheckStateName(stateInfo, "Spawn"))
    {
        OnAnimatorSpawnStateEnter();
    }
    else if (CheckStateNames(stateInfo, "Dash"))
    {
        OnAnimatorDashStateEnter();
    }
    // ... 긴 if-else 체인
}
```

### 3. 델리게이트 등록/해제 로직 중복
**문제:**
- `RegisterOnStateEnterDelegate`와 `RegisterOnStateExitDelegate`에서 중복 체크 로직 반복
- 델리게이트 등록/해제 패턴이 동일함에도 코드 중복

**위치:**
```301:359:Assets/Scripts/Runtime/Character/Animator/CharacterAnimator.cs
public void RegisterOnStateEnterDelegate(OnStateEnterDelegate action)
{
    if (OnStateEnter != null)
    {
        System.Delegate[] delegateArray = OnStateEnter.GetInvocationList();
        if (delegateArray.Contains(action))
        {
            // ... 중복 체크 로직
        }
    }
    // ... 등록 로직
}
// RegisterOnStateExitDelegate도 동일한 패턴
```

### 4. 공격 애니메이션 상태 관리 불명확
**문제:**
- `_isSequenceAttackAnimation` 불리언 플래그로 상태 관리
- 공격 타입이 명시적이지 않아 가독성 저하
- 상태 전환 로직이 복잡함

**위치:**
```13:41:Assets/Scripts/Runtime/Character/Animator/CharacterAnimator.Attack.cs
private bool _isSequenceAttackAnimation;
// ...
private bool IsAttackState(AnimatorStateInfo stateInfo, bool isEnter)
{
    if (!isEnter && _isSequenceAttackAnimation)
    {
        return stateInfo.IsName(_attackAnimationName + "Complete");
    }
    return stateInfo.IsName(_attackAnimationName);
}
```

---

## 리팩토링 제안

### 1. 애니메이터 파라미터 구조화

**목표:** 파라미터 ID를 구조체로 그룹화하여 관리 용이성 향상

**구현:**
- `AnimatorParameterIds` 구조체 생성
- 파라미터 타입별로 그룹화
- 파라미터 이름 상수는 유지 (디버깅 및 로깅용)

**파일:** `CharacterAnimator.Parameter.cs` (수정)

```csharp
public partial class CharacterAnimator
{
    // 파라미터 이름 상수 (로깅 및 디버깅용)
    private const string ANIMATOR_IS_SPAWNING_PARAMETER_NAME = "IsSpawning";
    // ... 기타 상수 유지

    // 파라미터 ID를 구조체로 그룹화
    private struct AnimatorParameterIds
    {
        // Bool Parameters
        public int IsSpawning;
        public int IsSpawned;
        public int IsAttacking;
        public int IsDamaging;
        public int IsGrounded;
        public int IsLeftCollision;
        public int IsRightCollision;
        public int IsSlippery;
        public int IsParrying;
        public int IsParrySuccess;
        public int UseWallSliding;

        // Trigger Parameters
        public int Spawn;
        public int Dash;
        public int Interact;
        public int Parry;
        public int Damage;
        public int Death;
        public int Disable;

        // Float Parameters
        public int AttackSpeed;
        public int BossPhase;
        public int DamageType;
        public int DirectionalX;
        public int DirectionalY;
        public int SpeedX;
        public int SpeedY;
        public int ForceSpeedX;
        public int ForceSpeedY;
        public int ParryingType;
    }

    private AnimatorParameterIds _parameterIds;
    protected HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

    protected virtual void InitializeAnimatorParameters()
    {
        AnimatorParameters.Clear();

        // Bool 파라미터 초기화
        _animator.AddAnimatorParameterIfExists(ANIMATOR_IS_SPAWNING_PARAMETER_NAME, 
            out _parameterIds.IsSpawning, AnimatorControllerParameterType.Bool, AnimatorParameters);
        // ... 나머지 Bool 파라미터들

        // Trigger 파라미터 초기화
        _animator.AddAnimatorParameterIfExists(ANIMATOR_SPAWN_PARAMETER_NAME, 
            out _parameterIds.Spawn, AnimatorControllerParameterType.Trigger, AnimatorParameters);
        // ... 나머지 Trigger 파라미터들

        // Float 파라미터 초기화
        _animator.AddAnimatorParameterIfExists(ANIMATOR_ATTACK_SPEED_PARAMETER_NAME, 
            out _parameterIds.AttackSpeed, AnimatorControllerParameterType.Float, AnimatorParameters);
        // ... 나머지 Float 파라미터들
    }

    // 파라미터 접근 메서드 수정
    public void SetIsGrounded(bool isGrounded)
    {
        _animator.UpdateAnimatorBool(_parameterIds.IsGrounded, isGrounded, AnimatorParameters);
    }

    public void SetIsAttacking(bool isAttacking)
    {
        _animator.UpdateAnimatorBool(_parameterIds.IsAttacking, isAttacking, AnimatorParameters);
    }
    // ... 기타 파라미터 설정 메서드들
}
```

**수정 필요 파일:**
- `CharacterAnimator.Parameter.cs`: 구조체 도입 및 초기화 로직 수정
- 모든 파라미터 ID를 사용하는 파일: `_parameterIds` 사용으로 변경

---

### 2. 상태 전환 로직 개선

**목표:** if-else 체인을 딕셔너리 기반 매핑으로 변경하여 확장성 향상

**구현:**
- 상태 이름과 핸들러를 딕셔너리로 매핑
- 공격 상태는 특수 처리 (기존 로직 유지)
- 새로운 상태 추가 시 딕셔너리만 수정

**파일:** `CharacterAnimator.StateMachine.cs` (신규)

```csharp
public partial class CharacterAnimator
{
    private Dictionary<string, StateHandler> _stateEnterHandlers;
    private Dictionary<string, StateHandler> _stateExitHandlers;

    private delegate void StateHandler(AnimatorStateInfo stateInfo, int layerIndex);

    private void InitializeStateHandlers()
    {
        _stateEnterHandlers = new Dictionary<string, StateHandler>
        {
            { "Spawn", (info, layer) => OnAnimatorSpawnStateEnter() },
            { "Dash", (info, layer) => OnAnimatorDashStateEnter() },
            { "Damage", (info, layer) => OnAnimatorDamageStateEnter() },
            { "DamageGround", (info, layer) => OnAnimatorDamageStateEnter() },
            { "Stun", (info, layer) => AnimatorLog.LogInfo("기절 상태의 애니메이션에 진입했습니다.") },
            { "Phase2", (info, layer) => LockMovement() },
            { "Phase3", (info, layer) => LockMovement() },
            { "Phase4", (info, layer) => LockMovement() },
            { "Parry", (info, layer) => OnAnimatorParryStateEnter() },
            { "ParrySuccess", (info, layer) => OnAnimatorParryStateEnter() }
        };

        _stateExitHandlers = new Dictionary<string, StateHandler>
        {
            { "Spawn", (info, layer) => OnAnimatorSpawnStateExit() },
            { "Dash", (info, layer) => OnAnimatorDashStateExit() },
            { "DashEnd", (info, layer) => OnAnimatorDashStateExit() },
            { "Damage", (info, layer) => OnAnimatorDamageStateExit() },
            { "DamageGround", (info, layer) => OnAnimatorDamageStateExit() },
            { "Phase2", (info, layer) => UnlockMovement() },
            { "Phase3", (info, layer) => UnlockMovement() },
            { "Phase4", (info, layer) => UnlockMovement() },
            { "Parry", (info, layer) => OnAnimatorParryStateExit() },
            { "ParrySuccess", (info, layer) => OnAnimatorParryStateExit() }
        };
    }

    public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 공격 상태 체크 (특수 처리)
        if (IsAttackState(stateInfo, true))
        {
            OnAnimatorAttackStateEnter();
        }
        else if (_stateEnterHandlers != null)
        {
            // 딕셔너리 기반 핸들러 호출
            foreach (var kvp in _stateEnterHandlers)
            {
                if (stateInfo.IsName(kvp.Key))
                {
                    kvp.Value(stateInfo, layerIndex);
                    break;
                }
            }
        }

        OnStateEnter?.Invoke(animator, stateInfo, layerIndex);
    }

    public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 공격 상태 체크 (특수 처리)
        if (IsAttackState(stateInfo, false))
        {
            OnAnimatorAttackStateExit();
        }
        else if (_stateExitHandlers != null)
        {
            // 딕셔너리 기반 핸들러 호출
            foreach (var kvp in _stateExitHandlers)
            {
                if (stateInfo.IsName(kvp.Key))
                {
                    kvp.Value(stateInfo, layerIndex);
                    break;
                }
            }
        }

        OnStateExit?.Invoke(animator, stateInfo, layerIndex);
    }
}
```

**수정 필요 파일:**
- `CharacterAnimator.cs`: `OnAnimatorStateEnter/Exit` 메서드 수정
- `Initialize()` 메서드에 `InitializeStateHandlers()` 호출 추가

---

### 3. 델리게이트 관리 개선

**목표:** 중복 코드 제거 및 재사용 가능한 델리게이트 관리 로직 구현

**구현:**
- 제네릭 메서드로 델리게이트 등록/해제 로직 통합
- 중복 체크 및 등록 로직 재사용

**파일:** `CharacterAnimator.Delegate.cs` (신규)

```csharp
public partial class CharacterAnimator
{
    private void RegisterDelegate<T>(ref T delegateField, T action, string delegateType) where T : System.Delegate
    {
        if (delegateField != null)
        {
            var invocationList = delegateField.GetInvocationList();
            if (System.Array.Exists(invocationList, d => d.Equals(action)))
            {
                Log.Warning(LogTags.Animation, "중복된 애니메이션 상태 {0} 이벤트를 등록할 수 없습니다. {1}", 
                    delegateType, action.Method);
                return;
            }
        }

        delegateField = (T)System.Delegate.Combine(delegateField, action);
    }

    private void UnregisterDelegate<T>(ref T delegateField, T action, string delegateType) where T : System.Delegate
    {
        if (delegateField != null)
        {
            var invocationList = delegateField.GetInvocationList();
            if (System.Array.Exists(invocationList, d => d.Equals(action)))
            {
                delegateField = (T)System.Delegate.Remove(delegateField, action);
                return;
            }
        }

        Log.Warning(LogTags.Animation, "등록되지 않은 애니메이션 상태 {0} 이벤트를 등록 해제할 수 없습니다. {1}", 
            delegateType, action.Method);
    }

    public void RegisterOnStateEnterDelegate(OnStateEnterDelegate action)
    {
        RegisterDelegate(ref OnStateEnter, action, "입장");
    }

    public void UnregisterOnStateEnterDelegate(OnStateEnterDelegate action)
    {
        UnregisterDelegate(ref OnStateEnter, action, "입장");
    }

    public void RegisterOnStateExitDelegate(OnStateExitDelegate action)
    {
        RegisterDelegate(ref OnStateExit, action, "퇴장");
    }

    public void UnregisterOnStateExitDelegate(OnStateExitDelegate action)
    {
        UnregisterDelegate(ref OnStateExit, action, "퇴장");
    }
}
```

**수정 필요 파일:**
- `CharacterAnimator.cs`: 기존 델리게이트 등록/해제 메서드 제거

---

### 4. 공격 애니메이션 상태 관리 개선

**목표:** 불리언 플래그 대신 명시적인 enum 사용으로 가독성 향상

**구현:**
- `AttackAnimationType` enum 도입
- 상태 전환 로직 명확화

**파일:** `CharacterAnimator.Attack.cs` (수정)

```csharp
public partial class CharacterAnimator
{
    private enum AttackAnimationType
    {
        None,
        Single,
        Sequence
    }

    private AttackAnimationType _currentAttackType = AttackAnimationType.None;
    private string _attackAnimationName;

    private bool IsAttackState(AnimatorStateInfo stateInfo, bool isEnter)
    {
        if (_currentAttackType == AttackAnimationType.None)
        {
            return false;
        }

        if (!isEnter && _currentAttackType == AttackAnimationType.Sequence)
        {
            return stateInfo.IsName(_attackAnimationName + "Complete");
        }

        return stateInfo.IsName(_attackAnimationName);
    }

    public void PlayAttackAnimation(string animationName)
    {
        _animator.Play(animationName, 0);
        AnimatorLog.LogInfo("공격 애니메이션을 재생합니다. {0}", animationName);
        _attackAnimationName = animationName;
        _currentAttackType = AttackAnimationType.Single;
    }

    public bool PlaySequenceAttackAnimation(string animationName)
    {
        if (_animator.UpdateAnimatorTriggerIfExists(animationName))
        {
            AnimatorLog.LogInfo("연속되는 공격 애니메이션을 재생합니다. {0}", animationName);
            _attackAnimationName = animationName;
            _currentAttackType = AttackAnimationType.Sequence;
            _animator.UpdateAnimatorBoolIfExists(animationName + "Progress", true);
            return true;
        }

        AnimatorLog.LogWarning("연속되는 공격 애니메이션 재생에 실패했습니다. {0}", animationName);
        return false;
    }

    protected virtual void OnAnimatorAttackStateExit()
    {
        StopAttacking();
        SetAttacking(false);
        ResetBlockDamageAnimationWhileAttack();

        // 공격 타입 초기화
        _currentAttackType = AttackAnimationType.None;
        _attackAnimationName = null;
    }
}
```

**수정 필요 파일:**
- `CharacterAnimator.Attack.cs`: enum 도입 및 관련 로직 수정

---

## 구현 계획

### Phase 1: 애니메이터 파라미터 구조화 (우선순위: 높음)
1. `CharacterAnimator.Parameter.cs`에 `AnimatorParameterIds` 구조체 추가
2. 파라미터 초기화 로직 수정
3. 모든 파라미터 ID 사용처를 `_parameterIds` 사용으로 변경
4. 테스트 및 검증

**예상 작업 시간:** 2-3시간

### Phase 2: 상태 전환 로직 개선 (우선순위: 중간)
1. `CharacterAnimator.StateMachine.cs` 파일 생성
2. 딕셔너리 기반 핸들러 매핑 구현
3. `OnAnimatorStateEnter/Exit` 메서드 수정
4. `Initialize()`에 `InitializeStateHandlers()` 호출 추가
5. 테스트 및 검증

**예상 작업 시간:** 1-2시간

### Phase 3: 델리게이트 관리 개선 (우선순위: 중간)
1. `CharacterAnimator.Delegate.cs` 파일 생성
2. 제네릭 델리게이트 관리 메서드 구현
3. 기존 델리게이트 등록/해제 메서드 제거 및 교체
4. 테스트 및 검증

**예상 작업 시간:** 1시간

### Phase 4: 공격 애니메이션 상태 관리 개선 (우선순위: 낮음)
1. `CharacterAnimator.Attack.cs`에 enum 도입
2. 관련 로직 수정
3. 테스트 및 검증

**예상 작업 시간:** 1시간

---

## 우선순위

### 높음 (즉시 진행)
1. **애니메이터 파라미터 구조화**: 유지보수성 향상 및 실수 방지

### 중간 (단기 계획)
2. **상태 전환 로직 개선**: 확장성 향상 및 코드 가독성 개선
3. **델리게이트 관리 개선**: 코드 중복 제거

### 낮음 (장기 계획)
4. **공격 애니메이션 상태 관리 개선**: 가독성 향상 (기능적 문제 없음)

---

## 권장 진행 순서

### 단계적 접근 방식

기본 리팩토링을 모두 완료한 후, 필요에 따라 고급 리팩토링을 진행하는 것을 권장합니다.

```
1단계: 기본 리팩토링 (Phase 1-4)
   ↓
   [테스트 및 안정화]
   ↓
2단계: 고급 리팩토링 (필요 시)
   ↓
   [최종 테스트 및 검증]
```

### 1단계: 기본 리팩토링 (1-2주)
- Phase 1: 애니메이터 파라미터 구조화
- Phase 2: 상태 전환 로직 개선
- Phase 3: 델리게이트 관리 개선
- Phase 4: 공격 애니메이션 상태 관리 개선

**중간 평가:**
- 코드 품질 개선 확인
- 버그 발생 여부 확인
- 팀 피드백 수집

### 2단계: 고급 리팩토링 (2-3주, 필요 시)
기본 리팩토링이 안정적으로 동작하는 것을 확인한 후, 더 근본적인 아키텍처 개선이 필요할 때 진행합니다.

**고급 리팩토링 문서:** [REFACTORING_ADVANCED.md](./REFACTORING_ADVANCED.md)

### 단계적 접근의 장점
1. **즉시 개선 효과**: 기본 리팩토링으로 빠르게 코드 품질 향상
2. **리스크 최소화**: 작은 단계로 나누어 안정성 확보
3. **학습 효과**: 기본 리팩토링을 통해 코드 구조를 이해한 후 고급 리팩토링 진행
4. **비즈니스 가치**: 기본 리팩토링만으로도 충분한 가치 제공 가능

---

## 주의사항

### 리팩토링 시 고려사항
1. **하위 호환성**: 기존 코드와의 호환성 유지 필요
2. **테스트**: 각 Phase 완료 후 충분한 테스트 수행
3. **점진적 적용**: 한 번에 모든 변경을 적용하지 말고 Phase별로 진행
4. **기존 동작 보장**: 리팩토링 후 기존 동작과 동일하게 작동해야 함

### 테스트 체크리스트
- [ ] 모든 애니메이션 상태 전환이 정상 작동
- [ ] 상태 플래그가 올바르게 설정/해제됨
- [ ] 애니메이터 파라미터가 정상적으로 업데이트됨
- [ ] 델리게이트 등록/해제가 정상 작동
- [ ] 공격 애니메이션이 정상 재생됨
- [ ] 이동/반전 제어가 정상 작동
- [ ] 패리 애니메이션이 정상 작동

---

## 참고사항

### 관련 파일
- `Character.cs`: CharacterAnimator를 사용하는 상위 클래스
- `IAnimatorStateMachine.cs`: 인터페이스 정의
- `AnimatorStateMachine.cs`: StateMachineBehaviour 구현

### 추가 개선 가능 영역
1. **인터페이스 분리**: `IAnimationStateProvider`, `IAnimationParameterSetter` 등으로 책임 분리
2. **이벤트 시스템**: UnityEvent 대신 C# 이벤트 사용 검토
3. **성능 최적화**: 딕셔너리 조회 대신 switch 표현식 사용 검토 (C# 8.0+)
4. **테스트 가능성**: 상태 변경 로직을 테스트 가능한 구조로 분리

### 고급 리팩토링
더 근본적인 아키텍처 개선이 필요하다면, 별도 클래스로 분리하는 고급 리팩토링을 고려해보세요.

**자세한 내용:** [REFACTORING_ADVANCED.md](./REFACTORING_ADVANCED.md)

---

**작성일:** 2024년
**작성자:** AI Assistant
**버전:** 1.0
