# 패시브 시스템 (Passive System)

## 개요

패시브 시스템은 게임 내 캐릭터의 자동 발동 능력을 관리하는 핵심 시스템입니다. 트리거 조건이 만족되면 자동으로 효과를 적용하며, RestTime을 통해 발동 빈도를 제어합니다.

## 주요 컴포넌트

### 1. PassiveSystem
- 캐릭터별 패시브 관리
- RestTime 중앙화 관리
- 패시브 활성화/비활성화

### 2. PassiveManager
- 전역 패시브 실행 관리
- 지연 실행 등록 및 처리
- 실행 횟수 제한 및 모니터링

### 3. PassiveEntity
- 개별 패시브 인스턴스
- 효과 적용 및 트리거 처리
- 오브젝트 풀링 지원

### 4. PassiveRestTimeController
- RestTime 중앙화 관리
- 시간 기반 Polling 방식
- 최적화된 체크 시점 관리

## 시스템 아키텍처

```
PassiveManager (전역)
├── PassiveSystem (캐릭터별)
│   ├── PassiveRestTimeController
│   └── PassiveEntity[] (오브젝트 풀)
└── PassiveExecution[] (지연 실행 큐)
```

## 주요 기능

### 1. 트리거 시스템
- 스킬 사용, 공격, 피격 등 다양한 트리거 지원
- 조건부 발동 및 확률 기반 발동
- 트리거 카운트 관리

### 2. 효과 시스템
- **버프/디버프**: 상태 효과 추가/제거
- **공격**: 자동 공격 실행
- **쿨다운 감소**: 스킬 쿨다운 단축
- **보상**: 아이템, 경험치 등 획득
- **강제 이동**: 캐릭터 위치 변경
- **지속시간 증가**: 기존 효과 지속시간 연장

### 3. RestTime 관리
- 패시브별 독립적인 쿨다운
- 중앙화된 관리로 성능 최적화
- 장비 변경 시에도 유지 (치팅 방지)

### 4. 오브젝트 풀링
- PassiveEntity 재사용으로 메모리 효율성
- 안전한 활성화/비활성화 처리

## 사용법

### 패시브 등록
```csharp
// 캐릭터에 패시브 추가
character.Passive.AddPassive(PassiveNames.Fireball, level);

// 패시브 활성화
passiveEntity.Activate();

// 패시브 비활성화
passiveEntity.Deactivate();
```

### 트리거 등록
```csharp
// 스킬 사용 시 패시브 발동
PassiveManager.Instance.RegisterExecute(delayTime, triggerInfo, passiveEntity);
```

### RestTime 확인
```csharp
// 패시브가 RestTime 상태인지 확인
bool isResting = character.Passive.IsPassiveResting(PassiveNames.Fireball);
```

## 성능 최적화

### 1. Polling 최적화
- 다음 체크 시점 기억으로 불필요한 연산 감소
- 프레임별 실행 횟수 제한 (기본값: 100회)

### 2. 오브젝트 풀링
- PassiveEntity 재사용으로 GC 압박 감소
- 안전한 상태 초기화

### 3. 중앙화된 관리
- RestTime 정보 중앙화로 메모리 효율성
- 캐릭터별 독립적인 관리

## 디버깅 및 모니터링

### 로깅 시스템
- 패시브 발동 시점 로깅
- 효과 적용 여부 추적
- 프레임별 실행 횟수 모니터링

### AppliedEffects 플래그
```csharp
// 적용된 효과 타입 추적
AppliedEffects effects = passiveEntity.AppliedEffects;
// EffectAdd, EffectRemove, Cooldown, Attack, Duration, Reward, ForceVelocity
```

## 확장성

### 새로운 효과 타입 추가
1. `AppliedEffects` 열거형에 새 플래그 추가
2. 효과 적용 메서드에서 플래그 설정
3. `LogAppliedEffects()` 메서드에 로깅 추가

### 새로운 트리거 타입 추가
1. `PassiveTrigger` 구조체 확장
2. 트리거 조건 로직 구현
3. PassiveManager에 등록

## 주의사항

1. **RestTime 유지**: 장비 변경 시에도 RestTime 정보 유지
2. **안전한 제거**: 오브젝트 풀링으로 인한 상태 변경 시 등록된 실행 안전 제거
3. **성능 모니터링**: 프레임별 실행 횟수 제한 준수
4. **메모리 관리**: PassiveEntity 적절한 반환으로 메모리 누수 방지 