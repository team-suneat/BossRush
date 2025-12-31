# 카메라 시스템 아키텍처

## 개요

카메라 시스템은 **Controller → Implementation 패턴**을 기반으로 설계되었습니다. 각 기능별로 역할이 명확히 분리되어 유지보수성과 확장성을 높였습니다.

## 전체 구조

```
CameraManager (Core)
├── CameraBoundingController → BoundingShape2D (Implementation)
├── CameraShakeController → CameraShake (Implementation)
├── CameraZoomController → CameraZoom (Implementation)
├── CameraRenderController → Camera 렌더링 관리
├── CameraCinemachineController → Cinemachine 설정 관리
├── CameraSettingsController → CameraAsset 설정 관리
├── CameraLookController → CameraLook (Implementation)
└── CameraFollowController → CameraFollow (Implementation)
```

## 폴더 구조

```
Assets/Scripts/Camera/
├── Core/                    # 핵심 매니저 클래스
│   └── CameraManager.cs
├── Controllers/             # 컨트롤러 클래스들
│   ├── CameraBoundingController.cs
│   ├── CameraShakeController.cs
│   ├── CameraZoomController.cs
│   ├── CameraRenderController.cs
│   ├── CameraCinemachineController.cs
│   ├── CameraSettingsController.cs
│   ├── CameraLookController.cs
│   └── CameraFollowController.cs
├── Implementations/         # 구현 클래스들
│   ├── CameraBounding.cs
│   ├── CameraShake.cs
│   ├── CameraZoom.cs
│   ├── CameraLook.cs
│   └── CameraFollow.cs
├── VirtualCameras/         # 가상 카메라 클래스들
│   └── TSVirtualCamera.cs
├── Effects/               # 카메라 효과들
├── Object/                # 카메라 관련 오브젝트들
├── Impurse/               # 임펄스 관련
└── Parallax/              # 패럴랙스 효과
```

## 네임스페이스 구조

```csharp
namespace IntemStudio.CameraSystem
{
    namespace Core          // CameraManager
    namespace Controllers   // 모든 컨트롤러 클래스들
    namespace Implementations // 모든 구현 클래스들
    namespace VirtualCameras // 가상 카메라 클래스들
}
```

## 컴포넌트 역할

### Core (핵심)
- **CameraManager**: 전체 카메라 시스템의 중앙 관리자
  - Singleton 패턴으로 전역 접근 제공
  - 각 컨트롤러에 기능 위임
  - 외부 API 제공

### Controllers (컨트롤러)
각 컨트롤러는 특정 기능을 담당하며, 설정 관리와 API 제공을 담당합니다.

- **CameraBoundingController**: 카메라 이동 범위 제한
- **CameraShakeController**: 카메라 진동 효과
- **CameraZoomController**: 카메라 줌 기능
- **CameraRenderController**: 카메라 렌더링 설정
- **CameraCinemachineController**: Cinemachine 파라미터 관리
- **CameraSettingsController**: 카메라 설정 관리
- **CameraLookController**: 카메라 시점 조절
- **CameraFollowController**: 카메라 추적 기능

### Implementations (구현)
실제 기능을 수행하는 클래스들로, 가상 카메라에 컴포넌트로 추가됩니다.

- **CameraBounding**: 바운딩 영역 관리
- **CameraShake**: 쉐이크 효과 실행
- **CameraZoom**: 줌 기능 실행
- **CameraLook**: 시점 조절 실행
- **CameraFollow**: 추적 기능 실행

## 데이터 흐름

```
외부 호출 → CameraManager → Controller → Implementation → Cinemachine 컴포넌트
```

### 예시: 카메라 줌 실행
1. `CameraManager.Instance.Zoom(target, size)` 호출
2. `CameraZoomController.Zoom()` 실행
3. 활성화된 가상 카메라의 `CameraZoom` 구현 클래스 찾기
4. `CameraZoom.Zoom()` 실행
5. Cinemachine Virtual Camera의 렌즈 설정 변경

## 의존성 관계

```
CameraManager
    ↓ (참조)
Controllers
    ↓ (참조)
Implementations (가상 카메라에 위치)
    ↓ (참조)
Cinemachine Components
```

## 설계 원칙

1. **단일 책임 원칙**: 각 클래스는 하나의 기능만 담당
2. **의존성 역전**: 구현 클래스가 Manager를 참조하지 않음
3. **일관성**: 모든 기능이 동일한 패턴을 따름
4. **확장성**: 새로운 기능 추가 시 일관된 구조 유지
5. **테스트 용이성**: 각 컴포넌트를 독립적으로 테스트 가능

## 성능 고려사항

- **지연 초기화**: 필요할 때만 컴포넌트 검색
- **캐싱**: 자주 사용되는 컴포넌트 참조 캐싱
- **에러 처리**: null 체크 및 안전한 접근
- **로깅**: 디버그 정보 제공

## 확장 방법

새로운 카메라 기능을 추가할 때:

1. **Implementation 클래스** 생성 (가상 카메라에 추가)
2. **Controller 클래스** 생성 (CameraManager에 추가)
3. **CameraManager에 래퍼 함수** 추가
4. **네임스페이스** 적용

이 구조를 통해 일관성 있고 유지보수 가능한 카메라 시스템을 구축할 수 있습니다.
