using System.Collections;
using TeamSuneat.CameraSystem.Controllers;
using TeamSuneat.Data;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat
{
    public class CameraManager : XStaticBehaviour<CameraManager>
    {
        // 상수 정의
        private const float DEFAULT_GLOBAL_SHAKE_FORCE = 1f;

        private const float DEFAULT_BLEND_TIME = 0f;

        public CameraAsset CameraAsset;
        public Camera MainCamera;
        public Camera UICamera;        

        // 직접 컴포넌트 참조 (Controller 대체)
        public CinemachineBrain BrainCamera;

        [Title("카메라 컨트롤러들")]        
        public CameraBoundingController BoundingController;
        public CameraShakeController ShakeController;
        public CameraZoomController ZoomController;
        public CameraRenderController RenderController;
        public CameraCinemachineController CinemachineController;
        public CameraSettingsController SettingsController;
        public TurnBasedCameraController TurnBasedController;

        // _globalShakeForce는 이제 ShakeController에서 관리됩니다.

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // 기본 카메라 컴포넌트들
            MainCamera = this.FindComponent<Camera>("MainCamera");
            UICamera = this.FindComponent<Camera>("UICamera");            

            // 직접 컴포넌트 참조
            BrainCamera = GetComponentInChildren<CinemachineBrain>();

            // 카메라 컨트롤러들
            BoundingController = GetComponent<CameraBoundingController>();
            ShakeController = GetComponent<CameraShakeController>();
            ZoomController = GetComponent<CameraZoomController>();
            RenderController = GetComponent<CameraRenderController>();
            CinemachineController = GetComponent<CameraCinemachineController>();
            SettingsController = GetComponent<CameraSettingsController>();
            TurnBasedController = GetComponent<TurnBasedCameraController>();
        }

        protected override void Awake()
        {
            base.Awake();

            Initialize();
        }

        public CinemachineBrain GetCinemachineBrain()
        {
            return BrainCamera;
        }

        // 래퍼 함수들 (Controller 대체용)

        /// <summary>
        /// 브레인 카메라를 반환합니다.
        /// </summary>
        public CinemachineBrain GetBrainCamera()
        {
            return BrainCamera;
        }

        // 팔로우 기능 래퍼 함수들

        /// <summary>
        /// 고정 카메라를 사용하므로 팔로우 기능을 비활성화합니다.
        /// </summary>
        public void SetFollowTarget(Transform target)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 팔로우 기능을 비활성화합니다.
        /// </summary>
        public void SetFollowPlayer()
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 팔로우 기능을 비활성화합니다.
        /// </summary>
        public void StopFollow()
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 팔로우를 사용하지 않습니다.
        /// </summary>
        public bool CheckFollowing()
        {
            return false;
        }

        /// <summary>
        /// 팔로우 타겟이 없습니다.
        /// </summary>
        public Transform GetCurrentFollowTarget()
        {
            return null;
        }

        public Transform GetMainCameraPoint()
        {
            if (MainCamera == null)
            {
                Log.Warning(LogTags.Camera, "MainCamera가 null입니다. 카메라 설정을 확인하세요.");
                return null;
            }
            return MainCamera.transform;
        }

        // 새로운 컨트롤러 API들

        /// <summary>
        /// 렌더링 설정을 기본값으로 복원합니다.
        /// </summary>
        public void SetCullingMaskToDefault()
        {
            RenderController?.SetCullingMaskToDefault();
        }

        /// <summary>
        /// 렌더링 설정을 모든 레이어로 설정합니다.
        /// </summary>
        public void SetCullingMaskToEverything()
        {
            RenderController?.SetCullingMaskToEverything();
        }

        /// <summary>
        /// Cinemachine X축 댐핑을 설정합니다.
        /// </summary>
        public void SetXDamping(float damping)
        {
            CinemachineController?.SetXDamping(damping);
        }

        /// <summary>
        /// Cinemachine 룩어헤드 시간을 설정합니다.
        /// </summary>
        public void SetLookaheadTime(float time)
        {
            CinemachineController?.SetLookaheadTime(time);
        }

        /// <summary>
        /// Cinemachine 소프트존 너비를 설정합니다.
        /// </summary>
        public void SetSoftZoneWidth(float width)
        {
            CinemachineController?.SetSoftZoneWidth(width);
        }

        /// <summary>
        /// 모든 Cinemachine 파라미터를 기본값으로 복원합니다.
        /// </summary>
        public void ResetAllCinemachineParameters()
        {
            CinemachineController?.ResetAllParameters();
        }

        /// <summary>
        /// 모든 카메라 설정을 기본값으로 복원합니다.
        /// </summary>
        public void ResetAllCameraSettings()
        {
            ShakeController?.ResetShakeSettings();
            ZoomController?.ResetZoomSettings();
            BoundingController?.ClearBounding();
            RenderController?.ResetCullingMask();
            CinemachineController?.ResetAllParameters();
            SettingsController?.ResetToDefaultSettings();
        }

        public void Initialize()
        {
            Setup(CameraAsset);
        }

        /// <summary>
        /// 카메라 에셋을 설정합니다. (SettingsController에 위임)
        /// </summary>
        public void Setup(CameraAsset cameraAsset)
        {
            SettingsController?.Setup(cameraAsset);
        }

        /// <summary>
        /// 기본 블렌드 시간을 반환합니다. (SettingsController에 위임)
        /// </summary>
        public float GetDefaultBlendTime()
        {
            return SettingsController?.GetDefaultBlendTime() ?? DEFAULT_BLEND_TIME;
        }

        #region Camera Zoom (위임 패턴)

        public void Zoom(Transform target, float zoomSize)
        {
            ZoomController?.Zoom(target, zoomSize);
        }

        public void ZoomDefault(Transform target)
        {
            ZoomController?.ZoomDefault(target);
        }

        #endregion Camera Zoom (위임 패턴)

        #region Camera Shake (위임 패턴)

        public void Shake(CinemachineImpulseSource impulseSource)
        {
            ShakeController?.Shake(impulseSource);
        }

        public void Shake(CinemachineImpulseSource impulseSource, ImpulsePreset preset)
        {
            ShakeController?.Shake(impulseSource, preset);
        }

        /// <summary>
        /// 고급 쉐이크 기능 (ShakeController에 위임)
        /// </summary>
        public void Shake2(CinemachineImpulseSource impulseSource, ImpulsePreset preset, float impactTime)
        {
            // TODO: ShakeController에 고급 쉐이크 기능 추가 후 위임
            ShakeController?.Shake(impulseSource, preset);
        }

        /// <summary>
        /// 지속 시간이 있는 쉐이크 (ShakeController에 위임)
        /// </summary>
        public void Shake(CinemachineImpulseSource impulseSource, ImpulsePreset preset, float impactTime)
        {
            // TODO: ShakeController에 지속 시간 쉐이크 기능 추가 후 위임
            ShakeController?.Shake(impulseSource, preset);
        }

        #endregion Camera Shake (위임 패턴)

        #region Camera Look (위임 패턴)

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void SetCameraLookEnabled(bool enabled)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void SetCameraLookInputThreshold(float threshold)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void SetCameraLookUpOffset(Vector3 offset)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void SetCameraLookDownOffset(Vector3 offset)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void SetCameraLookTransitionSpeed(float speed)
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 고정 카메라를 사용하므로 시점 조절을 비활성화합니다.
        /// </summary>
        public void ResetCameraLookToDefault()
        {
            Log.Info(LogTags.Camera, "고정 카메라를 사용합니다.");
        }

        /// <summary>
        /// 시점 조절을 사용하지 않습니다.
        /// </summary>
        public bool CheckCameraLookEnabled()
        {
            return false;
        }

        #endregion Camera Look (위임 패턴)

        #region Camera Bounding

        public void SetStageBoundingShape2D(Collider2D boundingShape)
        {
            // BoundingController가 아직 초기화되지 않았다면 지연 초기화
            if (BoundingController == null)
            {
                _ = StartCoroutine(SetStageBoundingShape2DDelayed(boundingShape));
                return;
            }

            BoundingController.SetStageBoundingShape2D(boundingShape, true);
        }

        /// <summary>
        /// BoundingController가 준비될 때까지 대기한 후 바운딩을 설정합니다.
        /// </summary>
        private IEnumerator SetStageBoundingShape2DDelayed(Collider2D boundingShape)
        {
            // BoundingController가 준비될 때까지 최대 10프레임 대기
            int maxWaitFrames = 10;
            int currentFrame = 0;

            while (BoundingController == null && currentFrame < maxWaitFrames)
            {
                yield return null;
                currentFrame++;
            }

            if (BoundingController != null)
            {
                BoundingController.SetStageBoundingShape2D(boundingShape, true);
                Log.Info(LogTags.Camera, "지연 초기화로 바운딩이 설정되었습니다: {0}", boundingShape?.name);
            }
            else
            {
                Log.Error(LogTags.Camera, "BoundingController 초기화에 실패했습니다. 바운딩을 설정할 수 없습니다.");
            }
        }

        public void SetCustomBoundingShape2D(Collider2D boundingShape)
        {
            BoundingController?.SetStageBoundingShape2D(boundingShape);
        }

        public void ResetBoundingShape2D()
        {
            BoundingController?.ClearBounding();
        }

        #endregion Camera Bounding

        #region 게임용 카메라 기능

        /// <summary>
        /// 웨이브 시작 시 카메라 효과
        /// </summary>
        public void OnWaveStart()
        {
            TurnBasedController?.OnWaveStart();
        }

        /// <summary>
        /// 보스 전투 시 카메라 효과
        /// </summary>
        public void OnBossFight()
        {
            TurnBasedController?.OnBossFight();
        }

        /// <summary>
        /// 보상 단계 시 카메라 효과
        /// </summary>
        public void OnRewardPhase()
        {
            TurnBasedController?.OnRewardPhase();
        }

        /// <summary>
        /// 기본 카메라 위치로 복귀
        /// </summary>
        public void ReturnToDefaultCamera()
        {
            TurnBasedController?.ReturnToDefault();
        }

        /// <summary>
        /// 카메라를 지정된 위치로 설정
        /// </summary>
        public void SetCameraPosition(Vector3 position)
        {
            TurnBasedController?.SetCameraPosition(position);
        }

        /// <summary>
        /// 카메라를 부드럽게 전환
        /// </summary>
        public void TransitionToPosition(Vector3 position, float duration = -1f)
        {
            TurnBasedController?.TransitionToPosition(position, duration);
        }

        #endregion 게임용 카메라 기능
    }
}