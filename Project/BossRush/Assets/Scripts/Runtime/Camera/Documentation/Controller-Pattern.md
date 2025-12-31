# Controller → Implementation 패턴

## 개요

카메라 시스템은 **Controller → Implementation 패턴**을 사용하여 역할을 명확히 분리했습니다. 이 패턴은 설정 관리와 실제 기능 실행을 분리하여 코드의 유지보수성과 확장성을 높입니다.

## 패턴 구조

```
CameraManager
    ↓ (위임)
Controller (설정 관리, API 제공)
    ↓ (위임)
Implementation (실제 기능 실행)
    ↓ (제어)
Cinemachine Components
```

## 역할 분리

### Controller (컨트롤러)
**역할**: 설정 관리, API 제공, 상태 관리

**특징**:
- CameraManager에 컴포넌트로 추가
- 외부 API 제공
- 설정값 관리
- 상태 추적
- 에러 처리

**예시**:
```csharp
public class CameraZoomController : XBehaviour
{
    [SerializeField] private float _defaultZoomSize = 5f;
    [SerializeField] private bool _isZooming = false;
    
    public void Zoom(Transform target, float zoomSize)
    {
        // 설정 검증
        float clampedZoomSize = Mathf.Clamp(zoomSize, _minZoomSize, _maxZoomSize);
        
        // 상태 업데이트
        _isZooming = true;
        _currentZoomSize = clampedZoomSize;
        
        // 구현 클래스에 위임
        ExecuteZoom(target, clampedZoomSize);
    }
    
    private void ExecuteZoom(Transform target, float zoomSize)
    {
        // 가상 카메라의 구현 클래스 찾기
        var activeVirtualCamera = CameraManager.Instance?.GetVirtualPlayerCharacterCamera();
        var cameraZoom = activeVirtualCamera?.GetComponent<Implementations.CameraZoom>();
        cameraZoom?.Zoom(target, zoomSize);
    }
}
```

### Implementation (구현)
**역할**: 실제 기능 실행

**특징**:
- 가상 카메라에 컴포넌트로 추가
- 단순한 기능 실행
- Manager 참조 없음
- 하나의 가상 카메라에 종속

**예시**:
```csharp
public class CameraZoom : XBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    
    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }
    
    public void Zoom(Transform target, float zoomSize)
    {
        if (_virtualCamera != null && target != null)
        {
            _virtualCamera.transform.position = target.position + new Vector3(0, 0, -10);
            _virtualCamera.m_Lens.OrthographicSize = zoomSize;
        }
    }
}
```

## 패턴의 장점

### 1. 역할 분리
- **Controller**: 복잡한 로직, 설정 관리
- **Implementation**: 단순한 기능 실행

### 2. 의존성 관리
- **Implementation**이 **Manager**를 참조하지 않음
- 순환 참조 방지
- 테스트 용이성 향상

### 3. 일관성
- 모든 기능이 동일한 패턴을 따름
- 새로운 기능 추가 시 일관된 구조

### 4. 확장성
- 새로운 기능 추가 시 명확한 구조
- 기존 코드 영향 최소화

## 구현 가이드

### 새로운 카메라 기능 추가 시

#### 1단계: Implementation 클래스 생성
```csharp
// Assets/Scripts/Camera/Implementations/CameraNewFeature.cs
namespace IntemStudio.CameraSystem.Implementations
{
    public class CameraNewFeature : XBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        
        private void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
        
        public void ExecuteFeature()
        {
            // 실제 기능 구현
        }
    }
}
```

#### 2단계: Controller 클래스 생성
```csharp
// Assets/Scripts/Camera/Controllers/CameraNewFeatureController.cs
namespace IntemStudio.CameraSystem.Controllers
{
    public class CameraNewFeatureController : XBehaviour
    {
        [SerializeField] private bool _isEnabled = true;
        
        public void ExecuteFeature()
        {
            if (!_isEnabled) return;
            
            // 활성화된 가상 카메라의 구현 클래스 찾기
            var activeVirtualCamera = CameraManager.Instance?.GetVirtualPlayerCharacterCamera();
            var implementation = activeVirtualCamera?.GetComponent<Implementations.CameraNewFeature>();
            implementation?.ExecuteFeature();
        }
    }
}
```

#### 3단계: CameraManager에 추가
```csharp
// CameraManager.cs
public class CameraManager : XStaticBehaiour<CameraManager>
{
    public CameraNewFeatureController NewFeatureController;
    
    public override void AutoGetComponents()
    {
        // ...
        NewFeatureController = GetComponent<CameraNewFeatureController>();
    }
    
    public void ExecuteNewFeature()
    {
        NewFeatureController?.ExecuteFeature();
    }
}
```

## 패턴 규칙

### Controller 규칙
1. **설정 관리**: Inspector에서 설정 가능한 값들 관리
2. **상태 추적**: 현재 상태를 추적하고 관리
3. **에러 처리**: null 체크 및 예외 처리
4. **로깅**: 디버그 정보 제공
5. **API 제공**: 외부에서 사용할 수 있는 메서드 제공

### Implementation 규칙
1. **단순성**: 복잡한 로직 없이 단순한 기능만 실행
2. **독립성**: Manager나 다른 시스템 참조 금지
3. **종속성**: 하나의 가상 카메라에만 종속
4. **효율성**: 최소한의 연산으로 기능 수행

## 주의사항

### Controller 주의사항
- **과도한 로직 금지**: 복잡한 로직은 Implementation에 위임
- **상태 동기화**: Implementation과 상태 동기화
- **성능 고려**: 자주 호출되는 메서드는 성능 최적화

### Implementation 주의사항
- **Manager 참조 금지**: 순환 참조 방지
- **단일 책임**: 하나의 기능만 담당
- **가상 카메라 종속**: 다른 가상 카메라 참조 금지

## 디버깅

### Controller 디버깅
```csharp
[Title("디버그 설정")]
public bool ShowDebugInfo = false;

private void ExecuteFeature()
{
    if (ShowDebugInfo)
    {
        Log.Info(LogTags.Camera, "기능 실행됨");
    }
}
```

### Implementation 디버깅
```csharp
public void ExecuteFeature()
{
    if (_virtualCamera == null)
    {
        Log.Warning(LogTags.Camera, "VirtualCamera가 null입니다.");
        return;
    }
    
    // 기능 실행
}
```

이 패턴을 통해 일관성 있고 유지보수 가능한 카메라 시스템을 구축할 수 있습니다.
