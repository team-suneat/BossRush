# PassiveRestTimeController

## 개요

PassiveRestTimeController는 패시브의 RestTime(쿨다운)을 중앙화하여 관리하는 클래스입니다. 각 PassiveEntity의 개별 코루틴 방식에서 개선된 시간 기반 Polling 방식으로 성능을 최적화했습니다.

## 주요 특징

### 1. 중앙화된 관리
- 모든 패시브의 RestTime을 한 곳에서 관리
- 메모리 효율성 및 성능 향상
- 일관된 쿨다운 처리

### 2. 최적화된 Polling
- 코루틴 대신 시간 기반 Polling 사용
- 다음 체크 시점 기억으로 불필요한 연산 감소
- 프레임별 부하 분산

### 3. 순수 C# 클래스
- MonoBehaviour 상속하지 않음
- Unity 의존성 최소화
- 테스트 용이성 향상

## 클래스 구조

```csharp
public class PassiveRestTimeController
{
    private Dictionary<PassiveNames, float> _restTimeEndTimes;
    private float _nextCheckTime;
    private const float CHECK_INTERVAL = 0.1f; // 0.1초마다 체크
}
```

## 주요 메서드

### RestTime 관리
```csharp
// RestTime 시작
public void StartRestTime(PassiveNames passiveName, float duration)

// RestTime 종료 확인
public bool IsResting(PassiveNames passiveName)

// RestTime 강제 종료
public void StopRestTime(PassiveNames passiveName)

// 모든 RestTime 초기화
public void ClearAllRestTimes()
```

### Polling 관리
```csharp
// 업데이트 (매 프레임 호출)
public void Update()

// 다음 체크 시점 계산
private void UpdateNextCheckTime()
```

## 동작 원리

### 1. RestTime 등록
```csharp
// 패시브 발동 시 RestTime 시작
restTimeController.StartRestTime(PassiveNames.Fireball, 5.0f);
// _restTimeEndTimes[PassiveNames.Fireball] = Time.time + 5.0f
```

### 2. Polling 체크
```csharp
// 매 프레임 Update() 호출
public void Update()
{
    if (Time.time < _nextCheckTime)
        return; // 아직 체크할 시간이 아님
    
    // 만료된 RestTime 정리
    CleanupExpiredRestTimes();
    
    // 다음 체크 시점 업데이트
    UpdateNextCheckTime();
}
```

### 3. 만료된 RestTime 정리
```csharp
private void CleanupExpiredRestTimes()
{
    var expiredPassives = new List<PassiveNames>();
    
    foreach (var kvp in _restTimeEndTimes)
    {
        if (Time.time >= kvp.Value)
        {
            expiredPassives.Add(kvp.Key);
        }
    }
    
    foreach (var passiveName in expiredPassives)
    {
        _restTimeEndTimes.Remove(passiveName);
    }
}
```

## 성능 최적화

### 1. 체크 간격 최적화
- 기본 체크 간격: 0.1초
- 불필요한 매 프레임 체크 방지
- 성능과 정확성의 균형

### 2. 다음 체크 시점 기억
```csharp
private void UpdateNextCheckTime()
{
    if (_restTimeEndTimes.Count == 0)
    {
        _nextCheckTime = float.MaxValue;
        return;
    }
    
    // 가장 빠른 만료 시점을 다음 체크 시점으로 설정
    _nextCheckTime = _restTimeEndTimes.Values.Min();
}
```

### 3. 메모리 효율성
- Dictionary 사용으로 빠른 검색
- 만료된 항목 즉시 제거
- 불필요한 메모리 사용 방지

## 사용법

### PassiveSystem에서 사용
```csharp
public class PassiveSystem : MonoBehaviour
{
    private PassiveRestTimeController _restTimeController;
    
    private void Awake()
    {
        _restTimeController = new PassiveRestTimeController();
    }
    
    private void Update()
    {
        _restTimeController.Update();
    }
    
    public bool IsPassiveResting(PassiveNames passiveName)
    {
        return _restTimeController.IsResting(passiveName);
    }
    
    public void StartRestTime(PassiveNames passiveName, float duration)
    {
        _restTimeController.StartRestTime(passiveName, duration);
    }
}
```

### PassiveEntity에서 사용
```csharp
public class PassiveEntity : XBehaviour
{
    public bool IsResting => Owner.Passive.IsPassiveResting(Name);
    
    private void StartRestTimer(PassiveEffectSettings effectSettings)
    {
        if (effectSettings.RestTime > 0)
        {
            Owner.Passive.StartRestTime(Name, effectSettings.RestTime);
        }
    }
}
```

## 장점

### 1. 성능 향상
- 코루틴 오버헤드 제거
- 중앙화된 관리로 메모리 효율성
- 최적화된 Polling으로 CPU 사용량 감소

### 2. 안정성
- Unity 생명주기와 독립적
- 예외 상황에 대한 안전한 처리
- 일관된 쿨다운 관리

### 3. 유지보수성
- 단일 책임 원칙 준수
- 테스트 용이성
- 확장 가능한 구조

## 디버깅 및 모니터링

### RestTime 상태 확인
```csharp
// 특정 패시브의 RestTime 상태 확인
bool isResting = restTimeController.IsResting(PassiveNames.Fireball);

// 모든 RestTime 정보 출력
foreach (var kvp in _restTimeEndTimes)
{
    float remainingTime = kvp.Value - Time.time;
    Debug.Log($"패시브 {kvp.Key}: {remainingTime:F1}초 남음");
}
```

### 성능 모니터링
```csharp
// 체크 간격 조정으로 성능 최적화
private const float CHECK_INTERVAL = 0.1f; // 필요에 따라 조정

// RestTime 개수 모니터링
int activeRestTimeCount = _restTimeEndTimes.Count;
```

## 확장성

### 새로운 기능 추가
```csharp
// RestTime 일시정지/재개
public void PauseRestTime(PassiveNames passiveName)
public void ResumeRestTime(PassiveNames passiveName)

// RestTime 속도 조절
public void SetRestTimeSpeed(PassiveNames passiveName, float speedMultiplier)

// RestTime 이벤트 콜백
public event Action<PassiveNames> OnRestTimeExpired;
```

### 설정 가능한 옵션
```csharp
public class RestTimeControllerConfig
{
    public float CheckInterval { get; set; } = 0.1f;
    public int MaxRestTimeCount { get; set; } = 1000;
    public bool EnableLogging { get; set; } = false;
}
```

## 주의사항

1. **Update 호출**: 매 프레임 Update() 메서드 호출 필요
2. **메모리 관리**: 만료된 RestTime 적절한 정리
3. **스레드 안전성**: 멀티스레드 환경에서 사용 시 동기화 필요
4. **성능 모니터링**: RestTime 개수가 많아지면 성능 영향 고려 