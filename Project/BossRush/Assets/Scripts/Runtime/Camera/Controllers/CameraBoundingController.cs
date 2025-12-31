using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    /// <summary>
    /// 카메라 바운딩 영역을 제어하는 컨트롤러 클래스
    /// 스테이지별로 다른 바운딩 콜라이더를 자동으로 적용하고 관리합니다.
    /// </summary>
    public class CameraBoundingController : XBehaviour
    {
        [Title("바운딩 제어 설정")]
        [InfoBox("현재 스테이지의 바운딩 콜라이더를 자동으로 감지하고 적용합니다.")]
        public bool AutoDetectBounding = true;

        public bool ApplyToPlayerCamera = true;
        public bool ApplyToEventCamera = true;

        public VirtualCamera VirtualPlayerCharacterCamera;
        public VirtualCamera VirtualEventCamera;

        private Collider2D _defaultBoundingShape;
        private Collider2D _currentBoundingShape;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // 직접 컴포넌트 참조
            VirtualPlayerCharacterCamera = this.FindComponent<VirtualCamera>("#Cinemachine/Cinemachine Virtual PlayerCharacter Camera");
            VirtualEventCamera = this.FindComponent<VirtualCamera>("#Cinemachine/Cinemachine Virtual Event Camera");
        }

        /// <summary>
        /// 스테이지의 바운딩 콜라이더를 설정합니다.
        /// </summary>
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

            ApplyBoundingToAllCameras();

            Log.Info(LogTags.Camera, "(Bounding) 바운딩 콜라이더가 설정되었습니다: {0}", boundingShape.name);
        }

        /// <summary> 모든 카메라에 바운딩을 적용합니다. </summary>
        private void ApplyBoundingToAllCameras()
        {
            if (_currentBoundingShape == null)
            {
                return;
            }

            // 플레이어 카메라에 적용
            if (ApplyToPlayerCamera)
            {
                ApplyBoundingToCamera(VirtualPlayerCharacterCamera, "Player");
            }

            // 이벤트 카메라에 적용
            if (ApplyToEventCamera)
            {
                ApplyBoundingToCamera(VirtualEventCamera, "Event");
            }
        }

        /// <summary> 특정 가상 카메라에 바운딩을 적용합니다. </summary>
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

        /// <summary> 바운딩을 제거합니다. </summary>
        public void ClearBounding()
        {
            _currentBoundingShape = null;

            ClearPlayerBounding();
            ClearEventBounding();
        }

        private void ClearPlayerBounding()
        {
            if (VirtualPlayerCharacterCamera == null) return;
            if (VirtualPlayerCharacterCamera.Confiner == null) return;

            if (_defaultBoundingShape == null)
            {
                if (VirtualPlayerCharacterCamera.Confiner.BoundingShape2D != null)
                {
                    Collider2D collider = VirtualPlayerCharacterCamera.Confiner.BoundingShape2D;
                    Log.Info(LogTags.Camera, "(Bounding) 플레이어 바운딩을 제거합니다: {0}", collider.name);

                    VirtualPlayerCharacterCamera.Confiner.BoundingShape2D = null;
                }
            }
            else
            {
                Collider2D collider = VirtualPlayerCharacterCamera.Confiner.BoundingShape2D;
                Log.Info(LogTags.Camera, "(Bounding) 플레이어 바운딩을 초기화합니다: {0} >> {1}", collider.name, _defaultBoundingShape);

                VirtualPlayerCharacterCamera.Confiner.BoundingShape2D = _defaultBoundingShape;
            }
        }

        private void ClearEventBounding()
        {
            if (VirtualEventCamera == null) return;
            if (VirtualEventCamera.Confiner == null) return;

            if (_defaultBoundingShape == null)
            {
                if (VirtualEventCamera.Confiner.BoundingShape2D != null)
                {
                    Collider2D collider = VirtualEventCamera.Confiner.BoundingShape2D;
                    Log.Info(LogTags.Camera, "(Bounding) 이벤트 바운딩을 제거합니다: {0}", collider.name);

                    VirtualEventCamera.Confiner.BoundingShape2D = null;
                }
            }
            else
            {
                Collider2D collider = VirtualEventCamera.Confiner.BoundingShape2D;
                Log.Info(LogTags.Camera, "(Bounding) 이벤트 바운딩을 초기화합니다: {0} >> {1}", collider.name, _defaultBoundingShape);

                VirtualEventCamera.Confiner.BoundingShape2D = _defaultBoundingShape;
            }
        }
    }
}