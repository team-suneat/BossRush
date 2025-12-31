# CameraManager API 레퍼런스

## 개요

`CameraManager`는 카메라 시스템의 중앙 관리자로, 모든 카메라 관련 기능에 대한 통합 API를 제공합니다. Singleton 패턴으로 구현되어 전역에서 접근 가능합니다.

## 기본 사용법

```csharp
// CameraManager 인스턴스 접근
CameraManager cameraManager = CameraManager.Instance;

// 카메라 기능 사용
cameraManager.Zoom(target, 5f);
cameraManager.Shake(impulseSource);
```

## 핵심 API

### 카메라 참조

#### `GetMainCameraPoint()`
메인 카메라의 Transform을 반환합니다.

```csharp
public Transform GetMainCameraPoint()
```

**반환값**: `Transform` - 메인 카메라의 Transform (null 가능)

**사용 예제**:
```csharp
Transform cameraTransform = CameraManager.Instance.GetMainCameraPoint();
if (cameraTransform != null)
{
    // 카메라 위치 사용
}
```

#### `GetVirtualPlayerCharacterCamera()`
플레이어 캐릭터용 가상 카메라를 반환합니다.

```csharp
public TSVirtualCamera GetVirtualPlayerCharacterCamera()
```

**반환값**: `TSVirtualCamera` - 플레이어 캐릭터용 가상 카메라

#### `GetVirtualEventCamera()`
이벤트용 가상 카메라를 반환합니다.

```csharp
public TSVirtualCamera GetVirtualEventCamera()
```

**반환값**: `TSVirtualCamera` - 이벤트용 가상 카메라

#### `GetBrainCamera()`
Cinemachine Brain 카메라를 반환합니다.

```csharp
public CinemachineBrain GetBrainCamera()
```

**반환값**: `CinemachineBrain` - Cinemachine Brain 컴포넌트

### 카메라 추적 (Follow)

#### `SetFollowTarget(Transform target)`
특정 타겟을 따라가도록 설정합니다.

```csharp
public void SetFollowTarget(Transform target)
```

**매개변수**:
- `target`: `Transform` - 따라갈 타겟

**사용 예제**:
```csharp
CameraManager.Instance.SetFollowTarget(playerTransform);
```

#### `SetFollowPlayer()`
플레이어 캐릭터를 따라가도록 설정합니다.

```csharp
public void SetFollowPlayer()
```

**사용 예제**:
```csharp
CameraManager.Instance.SetFollowPlayer();
```

#### `StopFollow()`
카메라 추적을 중지합니다.

```csharp
public void StopFollow()
```

#### `IsFollowing()`
현재 추적 중인지 확인합니다.

```csharp
public bool IsFollowing()
```

**반환값**: `bool` - 추적 중이면 true

#### `GetCurrentFollowTarget()`
현재 추적 중인 타겟을 반환합니다.

```csharp
public Transform GetCurrentFollowTarget()
```

**반환값**: `Transform` - 현재 추적 타겟 (null 가능)

### 카메라 바운딩 (Bounding)

#### `SetStageBoundingShape2D(Collider2D boundingShape)`
스테이지 바운딩 영역을 설정합니다.

```csharp
public void SetStageBoundingShape2D(Collider2D boundingShape)
```

**매개변수**:
- `boundingShape`: `Collider2D` - 바운딩 영역 콜라이더

#### `SetCustomBoundingShape2D(Collider2D boundingShape)`
커스텀 바운딩 영역을 설정합니다.

```csharp
public void SetCustomBoundingShape2D(Collider2D boundingShape)
```

#### `ResetBoundingShape2D()`
바운딩 영역을 초기화합니다.

```csharp
public void ResetBoundingShape2D()
```

### 카메라 렌더링 (Render)

#### `SetCullingMaskToDefault()`
컬링 마스크를 기본값으로 설정합니다.

```csharp
public void SetCullingMaskToDefault()
```

#### `SetCullingMaskToEverything()`
컬링 마스크를 모든 레이어로 설정합니다.

```csharp
public void SetCullingMaskToEverything()
```

### 카메라 줌 (Zoom)

#### `Zoom(Transform target, float zoomSize)`
특정 타겟으로 줌을 실행합니다.

```csharp
public void Zoom(Transform target, float zoomSize)
```

**매개변수**:
- `target`: `Transform` - 줌할 타겟
- `zoomSize`: `float` - 줌 크기

#### `ZoomDefault(Transform target)`
기본 줌으로 복원합니다.

```csharp
public void ZoomDefault(Transform target)
```

### 카메라 쉐이크 (Shake)

#### `Shake(CinemachineImpulseSource impulseSource)`
기본 쉐이크를 실행합니다.

```csharp
public void Shake(CinemachineImpulseSource impulseSource)
```

**매개변수**:
- `impulseSource`: `CinemachineImpulseSource` - 임펄스 소스

#### `Shake(CinemachineImpulseSource impulseSource, ImpulsePreset preset)`
프리셋을 사용하여 쉐이크를 실행합니다.

```csharp
public void Shake(CinemachineImpulseSource impulseSource, ImpulsePreset preset)
```

### 카메라 시점 조절 (Look)

#### `SetCameraLookEnabled(bool enabled)`
카메라 시점 조절을 활성화/비활성화합니다.

```csharp
public void SetCameraLookEnabled(bool enabled)
```

#### `SetCameraLookInputThreshold(float threshold)`
입력 임계값을 설정합니다.

```csharp
public void SetCameraLookInputThreshold(float threshold)
```

#### `SetCameraLookUpOffset(Vector3 offset)`
위쪽 시점 오프셋을 설정합니다.

```csharp
public void SetCameraLookUpOffset(Vector3 offset)
```

#### `SetCameraLookDownOffset(Vector3 offset)`
아래쪽 시점 오프셋을 설정합니다.

```csharp
public void SetCameraLookDownOffset(Vector3 offset)
```

#### `SetCameraLookTransitionSpeed(float speed)`
오프셋 전환 속도를 설정합니다.

```csharp
public void SetCameraLookTransitionSpeed(float speed)
```

#### `ResetCameraLookToDefault()`
카메라 시점 조절 설정을 기본값으로 리셋합니다.

```csharp
public void ResetCameraLookToDefault()
```

#### `IsCameraLookEnabled()`
현재 카메라 시점 조절 상태를 반환합니다.

```csharp
public bool IsCameraLookEnabled()
```

**반환값**: `bool` - 활성화되어 있으면 true

### Cinemachine 설정

#### `SetXDamping(float damping)`
X축 댐핑을 설정합니다.

```csharp
public void SetXDamping(float damping)
```

#### `SetLookaheadTime(float time)`
룩어헤드 시간을 설정합니다.

```csharp
public void SetLookaheadTime(float time)
```

#### `SetSoftZoneWidth(float width)`
소프트 존 너비를 설정합니다.

```csharp
public void SetSoftZoneWidth(float width)
```

#### `ResetAllCinemachineParameters()`
모든 Cinemachine 파라미터를 초기화합니다.

```csharp
public void ResetAllCinemachineParameters()
```

### 설정 관리

#### `Setup(CameraAsset cameraAsset)`
카메라 에셋을 설정합니다.

```csharp
public void Setup(CameraAsset cameraAsset)
```

**매개변수**:
- `cameraAsset`: `CameraAsset` - 카메라 설정 에셋

#### `GetDefaultBlendTime()`
기본 블렌드 시간을 반환합니다.

```csharp
public float GetDefaultBlendTime()
```

**반환값**: `float` - 기본 블렌드 시간

## 에러 처리

모든 API는 안전한 null 체크를 수행하며, 문제 발생 시 로그를 출력합니다:

```csharp
// 안전한 사용
if (CameraManager.Instance != null)
{
    CameraManager.Instance.Zoom(target, 5f);
}
```

## 성능 고려사항

- **지연 초기화**: 컴포넌트는 필요할 때만 검색됩니다
- **캐싱**: 자주 사용되는 참조는 캐싱됩니다
- **에러 처리**: null 체크로 안전성을 보장합니다

## 주의사항

1. **초기화 순서**: CameraManager가 초기화된 후 사용하세요
2. **Null 체크**: Instance가 null일 수 있으므로 체크하세요
3. **스레드 안전성**: 메인 스레드에서만 사용하세요
