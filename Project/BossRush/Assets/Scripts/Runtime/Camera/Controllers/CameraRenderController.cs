using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    public class CameraRenderController : XBehaviour
    {
        [Title("렌더링 설정")]
        [InfoBox("카메라 렌더링 설정을 제어합니다.")]
        [SerializeField] private LayerMask _defaultCullingMask = -1;

        // 카메라 컴포넌트 캐싱 (AutoGetComponents에서 사용)
        [SerializeField] private Camera _mainCamera;

        [SerializeField] private LayerMask _originalCullingMask;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // 직접 컴포넌트 참조
            _mainCamera = this.FindComponent<Camera>("MainCamera");

            // 기본 Culling Mask 저장
            if (_mainCamera != null)
            {
                _originalCullingMask = _mainCamera.cullingMask;
                _defaultCullingMask = _originalCullingMask;
            }
        }

        public void SetCullingMaskToDefault()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Render) MainCamera가 null입니다.");
                return;
            }

            _mainCamera.cullingMask = _defaultCullingMask;
        }

        public void SetCullingMaskToEverything()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Render) MainCamera가 null입니다.");
                return;
            }

            _mainCamera.cullingMask = int.MaxValue;
        }

        public void ResetCullingMask()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Render) MainCamera가 null입니다.");
                return;
            }

            _mainCamera.cullingMask = _originalCullingMask;
        }
    }
}