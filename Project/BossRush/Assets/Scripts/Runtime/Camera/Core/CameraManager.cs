using System.Collections;
using TeamSuneat.CameraSystem.Controllers;
using TeamSuneat.Data;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Core
{
    public class CameraManager : XStaticBehaviour<CameraManager>
    {
        // 상수 정의
        private const float DEFAULT_BLEND_TIME = 0f;

        public CameraAsset CameraAsset;
        public Camera MainCamera;
        public Camera UICamera;

        // 직접 컴포넌트 참조 (Controller 대체)
        public CinemachineBrain BrainCamera;

        [Title("#Controllers")]
        public CameraBoundingController BoundingController;
        public CameraShakeController ShakeController;
        public CameraZoomController ZoomController;
        public CameraRenderController RenderController;
        public CameraCinemachineController CinemachineController;
        public CameraSettingsController SettingsController;
        public CameraFollowController FollowController;

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
            FollowController = GetComponent<CameraFollowController>();
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public CinemachineBrain GetBrainCamera()
        {
            return BrainCamera;
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

        public void Initialize()
        {
            Setup(CameraAsset);
        }

        public void Setup(CameraAsset cameraAsset)
        {
            SettingsController?.Setup(cameraAsset);
        }

        public float GetDefaultBlendTime()
        {
            return SettingsController?.GetDefaultBlendTime() ?? DEFAULT_BLEND_TIME;
        }

        public void SetCullingMaskToDefault()
        {
            RenderController?.SetCullingMaskToDefault();
        }

        public void SetCullingMaskToEverything()
        {
            RenderController?.SetCullingMaskToEverything();
        }

        public void SetLookaheadTime(float time)
        {
            CinemachineController?.SetLookaheadTime(time);
        }

        public void SetSoftZoneWidth(float width)
        {
            CinemachineController?.SetSoftZoneWidth(width);
        }

        public void ResetAllCinemachineParameters()
        {
            CinemachineController?.ResetAllParameters();
        }

        public void ResetAllCameraSettings()
        {
            ShakeController?.ResetShakeSettings();
            ZoomController?.ResetZoomSettings();
            BoundingController?.ClearBounding();
            RenderController?.ResetCullingMask();
            CinemachineController?.ResetAllParameters();
            SettingsController?.ResetToDefaultSettings();
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

        #endregion Camera Shake (위임 패턴)

        #region Camera Follow (위임 패턴)

        public void SetFollowTarget(Transform target)
        {
            FollowController?.SetFollowTarget(target);
        }

        public void StopFollow()
        {
            FollowController?.StopFollow();
        }

        public bool IsFollowing()
        {
            return FollowController?.IsFollowing() ?? false;
        }

        public Transform GetCurrentFollowTarget()
        {
            return FollowController?.GetCurrentFollowTarget();
        }

        #endregion Camera Follow (위임 패턴)

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
    }
}