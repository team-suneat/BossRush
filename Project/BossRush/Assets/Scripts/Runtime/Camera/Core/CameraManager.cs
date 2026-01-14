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
        private const float DEFAULT_BLEND_TIME = 0f;

        [SerializeField] private CameraAsset _cameraAsset;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private Camera _uiCamera;
        [SerializeField] private CinemachineBrain _brainCamera;

        [Title("#Controllers")]
        [SerializeField] private CameraBoundingController _boundingController;
        [SerializeField] private CameraShakeController _shakeController;
        [SerializeField] private CameraZoomController _zoomController;
        [SerializeField] private CameraRenderController _renderController;
        [SerializeField] private CameraCinemachineController _cinemachineController;
        [SerializeField] private CameraSettingsController _settingsController;
        [SerializeField] private CameraFollowController _followController;

        public CameraAsset CameraAsset => _cameraAsset;
        public Camera MainCamera => _mainCamera;
        public Camera UICamera => _uiCamera;
        public CinemachineBrain BrainCamera => _brainCamera;
        public CameraBoundingController BoundingController => _boundingController;
        public CameraShakeController ShakeController => _shakeController;
        public CameraZoomController ZoomController => _zoomController;
        public CameraRenderController RenderController => _renderController;
        public CameraCinemachineController CinemachineController => _cinemachineController;
        public CameraSettingsController SettingsController => _settingsController;
        public CameraFollowController FollowController => _followController;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _mainCamera ??= this.FindComponent<Camera>("MainCamera");
            _uiCamera ??= this.FindComponent<Camera>("UICamera");
            _brainCamera ??= GetComponentInChildren<CinemachineBrain>();
            _boundingController ??= GetComponent<CameraBoundingController>();
            _shakeController ??= GetComponent<CameraShakeController>();
            _zoomController ??= GetComponent<CameraZoomController>();
            _renderController ??= GetComponent<CameraRenderController>();
            _cinemachineController ??= GetComponent<CameraCinemachineController>();
            _settingsController ??= GetComponent<CameraSettingsController>();
            _followController ??= GetComponent<CameraFollowController>();
        }

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public CinemachineBrain GetBrainCamera()
        {
            return _brainCamera;
        }

        public Transform GetMainCameraPoint()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "MainCamera가 null입니다. 카메라 설정을 확인하세요.");
                return null;
            }
            return _mainCamera.transform;
        }

        public void Initialize()
        {
            Setup(_cameraAsset);
        }

        public void Setup(CameraAsset cameraAsset)
        {
            _cameraAsset = cameraAsset;
            _settingsController?.Setup(cameraAsset);
        }

        public float GetDefaultBlendTime()
        {
            return _settingsController?.GetDefaultBlendTime() ?? DEFAULT_BLEND_TIME;
        }

        public void SetCullingMaskToDefault()
        {
            _renderController?.SetCullingMaskToDefault();
        }

        public void SetCullingMaskToEverything()
        {
            _renderController?.SetCullingMaskToEverything();
        }

        public void SetLookaheadTime(float time)
        {
            _cinemachineController?.SetLookaheadTime(time);
        }

        public void SetSoftZoneWidth(float width)
        {
            _cinemachineController?.SetSoftZoneWidth(width);
        }

        public void ResetAllCinemachineParameters()
        {
            _cinemachineController?.ResetAllParameters();
        }

        public void ResetAllCameraSettings()
        {
            _shakeController?.ResetShakeSettings();
            _zoomController?.ResetZoomSettings();
            _boundingController?.ClearBounding();
            _renderController?.ResetCullingMask();
            _cinemachineController?.ResetAllParameters();
            _settingsController?.ResetToDefaultSettings();
        }

        #region Camera Zoom (위임 패턴)

        public void Zoom(Transform target, float zoomSize)
        {
            _zoomController?.Zoom(target, zoomSize);
        }

        public void ZoomDefault(Transform target)
        {
            _zoomController?.ZoomDefault(target);
        }

        #endregion Camera Zoom (위임 패턴)

        #region Camera Shake (위임 패턴)

        public void Shake(CinemachineImpulseSource impulseSource, CameraImpulseAsset preset)
        {
            _shakeController?.Shake(impulseSource, preset);
        }

        public void ShakeAtPosition(Vector3 position, CameraImpulseAsset preset)
        {
            _shakeController?.ShakeAtPosition(position, preset);
        }

        #endregion Camera Shake (위임 패턴)

        #region Camera Follow (위임 패턴)

        public void SetFollowTarget(Transform target)
        {
            _followController?.SetFollowTarget(target);
        }

        public void StopFollow()
        {
            _followController?.StopFollow();
        }

        public bool IsFollowing()
        {
            return _followController?.IsFollowing() ?? false;
        }

        public Transform GetCurrentFollowTarget()
        {
            return _followController?.GetCurrentFollowTarget();
        }

        #endregion Camera Follow (위임 패턴)

        #region Camera Bounding

        public void SetStageBoundingShape2D(Collider2D boundingShape)
        {
            if (_boundingController == null)
            {
                _ = StartCoroutine(SetStageBoundingShape2DDelayed(boundingShape));
                return;
            }

            _boundingController.SetStageBoundingShape2D(boundingShape, true);
        }

        private IEnumerator SetStageBoundingShape2DDelayed(Collider2D boundingShape)
        {
            int maxWaitFrames = 10;
            int currentFrame = 0;

            while (_boundingController == null && currentFrame < maxWaitFrames)
            {
                yield return null;
                currentFrame++;
            }

            if (_boundingController != null)
            {
                _boundingController.SetStageBoundingShape2D(boundingShape, true);
                Log.Info(LogTags.Camera, "지연 초기화로 바운딩이 설정되었습니다: {0}", boundingShape?.name);
            }
            else
            {
                Log.Error(LogTags.Camera, "BoundingController 초기화에 실패했습니다. 바운딩을 설정할 수 없습니다.");
            }
        }

        public void SetCustomBoundingShape2D(Collider2D boundingShape)
        {
            _boundingController?.SetStageBoundingShape2D(boundingShape);
        }

        public void ResetBoundingShape2D()
        {
            _boundingController?.ClearBounding();
        }

        #endregion Camera Bounding
    }
}