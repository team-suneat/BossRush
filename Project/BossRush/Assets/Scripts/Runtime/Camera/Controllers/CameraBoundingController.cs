using TeamSuneat.CameraSystem.VirtualCameras;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    public class CameraBoundingController : XBehaviour
    {
        // 상수 정의
        private const string CAMERA_TYPE_PLAYER = "Player";

        [Title("바운딩 제어 설정")]
        [InfoBox("현재 스테이지의 바운딩 콜라이더를 자동으로 감지하고 적용합니다.")]
        [SerializeField] private bool _autoDetectBounding = true;

        private VirtualCamera _virtualCamera;
        private Collider2D _defaultBoundingShape;
        private Collider2D _currentBoundingShape;

        private void Awake()
        {
            _virtualCamera = GetComponentInChildren<VirtualCamera>();
        }

        public void SetStageBoundingShape2D(Collider2D boundingShape, bool isDefault = false)
        {
            if (boundingShape == null)
            {
                Log.Warning(LogTags.Camera, "(Bounding) 바운딩 콜라이더가 null입니다.");
                return;
            }

            _currentBoundingShape = boundingShape;

            if (isDefault)
            {
                _defaultBoundingShape = boundingShape;
            }

            ApplyBoundingToPlayerCamera();

            Log.Info(LogTags.Camera, "(Bounding) 바운딩 콜라이더가 설정되었습니다: {0}", boundingShape.name);
        }

        private void ApplyBoundingToPlayerCamera()
        {
            if (_currentBoundingShape == null)
            {
                return;
            }

            ApplyBoundingToCamera(_virtualCamera, CAMERA_TYPE_PLAYER);
        }

        private void ApplyBoundingToCamera(VirtualCamera virtualCamera, string cameraType)
        {
            if (virtualCamera?.Confiner == null)
            {
                Log.Warning(LogTags.Camera, "(Bounding) {0} 카메라의 Confiner가 null입니다.", cameraType);
                return;
            }

            virtualCamera.Confiner.BoundingShape2D = _currentBoundingShape;
            virtualCamera.Confiner.InvalidateBoundingShapeCache();

            Log.Info(LogTags.Camera, "(Bounding) {0} 카메라에 바운딩이 적용되었습니다.", cameraType);
        }

        public void ClearBounding()
        {
            _currentBoundingShape = null;

            ClearPlayerBounding();
        }

        private void ClearPlayerBounding()
        {
            if (_virtualCamera?.Confiner == null)
            {
                return;
            }

            var confiner = _virtualCamera.Confiner;
            var currentCollider = confiner.BoundingShape2D;

            if (_defaultBoundingShape == null)
            {
                if (currentCollider != null)
                {
                    Log.Info(LogTags.Camera, "(Bounding) 플레이어 바운딩을 제거합니다: {0}", currentCollider.name);
                    confiner.BoundingShape2D = null;
                }
            }
            else
            {
                Log.Info(LogTags.Camera, "(Bounding) 플레이어 바운딩을 초기화합니다: {0} >> {1}",
                    currentCollider?.name ?? "null", _defaultBoundingShape.name);
                confiner.BoundingShape2D = _defaultBoundingShape;
            }
        }
    }
}