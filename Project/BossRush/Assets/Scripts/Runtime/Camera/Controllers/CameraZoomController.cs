using TeamSuneat.CameraSystem.Implementations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    /// <summary>
    /// 카메라 줌을 조율하는 컨트롤러
    /// 줌 실행, 설정 조정, 상태 관리를 담당합니다.
    /// </summary>
    public class CameraZoomController : XBehaviour
    {
        [Title("줌 설정")]
        [InfoBox("카메라 줌을 조율합니다.")]
        [SerializeField] private float _defaultZoomSize = 5f;

        [SerializeField] private float _minZoomSize = 1f;
        [SerializeField] private float _maxZoomSize = 20f;
        [SerializeField] private float _zoomSpeed = 2f;

        // 줌 관련 상태 (AutoGetComponents에서 사용)
        [SerializeField] private bool _isZooming = false;

        [SerializeField] private float _currentZoomSize = 5f;
        [SerializeField] private Transform _currentZoomTarget = null;

        [SerializeField] private CameraZoom _cameraZoom;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _cameraZoom = GetComponentInChildren<CameraZoom>();
        }

        /// <summary>
        /// 줌을 실행합니다.
        /// </summary>
        public void Zoom(Transform target, float zoomSize)
        {
            if (target == null)
            {
                Log.Warning(LogTags.Camera, "(Zoom) 줌 타겟이 null입니다.");
                return;
            }

            // 줌 크기 제한
            float clampedZoomSize = Mathf.Clamp(zoomSize, _minZoomSize, _maxZoomSize);

            _currentZoomTarget = target;
            _currentZoomSize = clampedZoomSize;
            _isZooming = true;

            // 실제 줌 실행 (구현 클래스에 위임)
            ExecuteZoom(target, clampedZoomSize);

            Log.Info(LogTags.Camera, "(Zoom) 줌이 실행되었습니다. 타겟: {0}, 크기: {1}", target.name, clampedZoomSize);
        }

        /// <summary>
        /// 기본 줌으로 복원합니다.
        /// </summary>
        public void ZoomDefault(Transform target)
        {
            if (target == null)
            {
                Log.Warning(LogTags.Camera, "(Zoom) 줌 타겟이 null입니다.");
                return;
            }

            Zoom(target, _defaultZoomSize);

            Log.Info(LogTags.Camera, "(Zoom) 기본 줌으로 복원되었습니다. 타겟: {0}", target.name);
        }

        /// <summary>
        /// 줌을 중지합니다.
        /// </summary>
        public void StopZoom()
        {
            _isZooming = false;
            _currentZoomTarget = null;
            _currentZoomSize = _defaultZoomSize;

            Log.Info(LogTags.Camera, "(Zoom) 줌이 중지되었습니다.");
        }

        /// <summary>
        /// 줌 크기를 설정합니다.
        /// </summary>
        public void SetZoomSize(float zoomSize)
        {
            _currentZoomSize = Mathf.Clamp(zoomSize, _minZoomSize, _maxZoomSize);

            Log.Info(LogTags.Camera, "(Zoom) 줌 크기가 설정되었습니다: {0}", _currentZoomSize);
        }

        /// <summary>
        /// 줌 속도를 설정합니다.
        /// </summary>
        public void SetZoomSpeed(float speed)
        {
            _zoomSpeed = Mathf.Max(0.1f, speed);

            Log.Info(LogTags.Camera, "(Zoom) 줌 속도가 설정되었습니다: {0}", _zoomSpeed);
        }

        /// <summary>
        /// 최소 줌 크기를 설정합니다.
        /// </summary>
        public void SetMinZoomSize(float minSize)
        {
            _minZoomSize = Mathf.Max(0.1f, minSize);

            Log.Info(LogTags.Camera, "(Zoom) 최소 줌 크기가 설정되었습니다: {0}", _minZoomSize);
        }

        /// <summary>
        /// 최대 줌 크기를 설정합니다.
        /// </summary>
        public void SetMaxZoomSize(float maxSize)
        {
            _maxZoomSize = Mathf.Max(_minZoomSize, maxSize);

            Log.Info(LogTags.Camera, "(Zoom) 최대 줌 크기가 설정되었습니다: {0}", _maxZoomSize);
        }

        /// <summary>
        /// 현재 줌 상태를 반환합니다.
        /// </summary>
        public bool CheckZooming()
        {
            return _isZooming;
        }

        /// <summary>
        /// 현재 줌 크기를 반환합니다.
        /// </summary>
        public float GetCurrentZoomSize()
        {
            return _currentZoomSize;
        }

        /// <summary>
        /// 현재 줌 타겟을 반환합니다.
        /// </summary>
        public Transform GetCurrentZoomTarget()
        {
            return _currentZoomTarget;
        }

        /// <summary>
        /// 줌 설정을 초기화합니다.
        /// </summary>
        public void ResetZoomSettings()
        {
            _defaultZoomSize = 5f;
            _minZoomSize = 1f;
            _maxZoomSize = 20f;
            _zoomSpeed = 2f;
            _isZooming = false;
            _currentZoomSize = _defaultZoomSize;
            _currentZoomTarget = null;

            Log.Info(LogTags.Camera, "(Zoom) 줌 설정이 초기화되었습니다.");
        }

        /// <summary>
        /// 실제 줌을 실행합니다. (구현 클래스에 위임)
        /// </summary>
        private void ExecuteZoom(Transform target, float zoomSize)
        {
            // 활성화된 가상 카메라의 CameraZoom 구현 클래스 찾기

            if (_cameraZoom != null)
            {
                _cameraZoom.Zoom(target, zoomSize);
            }
            else
            {
                Log.Warning(LogTags.Camera, "(Zoom) CameraZoom 구현 클래스를 찾을 수 없습니다.");
            }

            Log.Info(LogTags.Camera, "(Zoom) 줌 실행됨 - 타겟: {0}, 크기: {1}", target.name, zoomSize);
        }

        /// <summary>
        /// 줌 설정 정보를 반환합니다.
        /// </summary>
        public ZoomSettingsInfo GetZoomSettingsInfo()
        {
            return new ZoomSettingsInfo
            {
                DefaultZoomSize = _defaultZoomSize,
                MinZoomSize = _minZoomSize,
                MaxZoomSize = _maxZoomSize,
                ZoomSpeed = _zoomSpeed,
                IsZooming = _isZooming,
                CurrentZoomSize = _currentZoomSize,
                CurrentZoomTarget = _currentZoomTarget
            };
        }
    }

    /// <summary>
    /// 줌 설정 정보를 담는 구조체
    /// </summary>
    [System.Serializable]
    public struct ZoomSettingsInfo
    {
        public float DefaultZoomSize;
        public float MinZoomSize;
        public float MaxZoomSize;
        public float ZoomSpeed;
        public bool IsZooming;
        public float CurrentZoomSize;
        public Transform CurrentZoomTarget;
    }
}