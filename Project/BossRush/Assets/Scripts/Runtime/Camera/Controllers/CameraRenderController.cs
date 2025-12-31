using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    /// <summary>
    /// 카메라 렌더링을 제어하는 컨트롤러
    /// Culling Mask, 렌더링 설정 등을 관리합니다.
    /// </summary>
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

        /// <summary>
        /// Culling Mask를 기본값으로 설정합니다.
        /// </summary>
        public void SetCullingMaskToDefault()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Render) MainCamera가 null입니다.");
                return;
            }

            _mainCamera.cullingMask = _defaultCullingMask;
        }

        /// <summary>
        /// Culling Mask를 모든 레이어로 설정합니다.
        /// </summary>
        public void SetCullingMaskToEverything()
        {
            if (_mainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Render) MainCamera가 null입니다.");
                return;
            }

            _mainCamera.cullingMask = int.MaxValue;
        }

        /// <summary>
        /// Culling Mask를 원래 값으로 복원합니다.
        /// </summary>
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