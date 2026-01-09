using TeamSuneat.CameraSystem.VirtualCameras;
using TeamSuneat.Data;
using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Controllers
{
    public class CameraSettingsController : XBehaviour
    {
        [Title("카메라 설정")]
        [InfoBox("카메라 설정을 관리합니다.")]
        [SerializeField] private CameraAsset _currentAsset;

        // 카메라 컴포넌트 캐싱 (AutoGetComponents에서 사용)
        [SerializeField] private CinemachineBrain _brainCamera;

        [SerializeField] private VirtualCamera _virtualPlayerCamera;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            // 직접 컴포넌트 참조
            _brainCamera = GetComponentInChildren<CinemachineBrain>();
            _virtualPlayerCamera = this.FindComponent<VirtualCamera>("#Cinemachine/Cinemachine Virtual PlayerCharacter Camera");
        }

        public void Setup(CameraAsset cameraAsset)
        {
            if (cameraAsset == null)
            {
                Log.Warning(LogTags.Camera, "(Setting) CameraAsset이 null입니다.");
                return;
            }

            _currentAsset = cameraAsset;

            // 가상 카메라 설정
            if (_virtualPlayerCamera != null)
            {
                _virtualPlayerCamera.Setup(cameraAsset);
            }

            // 브레인 카메라 설정
            if (_brainCamera != null)
            {
                _brainCamera.DefaultBlend.Time = cameraAsset.DefaultBlendTime;
            }

            Log.Info(LogTags.Camera, "(Setting) 카메라 에셋이 설정되었습니다: {0}", cameraAsset.name);
        }

        public float GetDefaultBlendTime()
        {
            if (_brainCamera != null)
            {
                return _brainCamera.DefaultBlend.BlendTime;
            }

            return 0f;
        }

        public void ResetToDefaultSettings()
        {
            if (_currentAsset != null)
            {
                Setup(_currentAsset);

                Log.Info(LogTags.Camera, "(Setting) 카메라 설정이 기본값으로 복원되었습니다.");
            }
            else
            {
                Log.Warning(LogTags.Camera, "(Setting) 현재 카메라 에셋이 null입니다. 복원할 수 없습니다.");
            }
        }

        public bool ValidateSettings()
        {
            bool isValid = true;

            if (_brainCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Setting) BrainCamera가 null입니다.");
                isValid = false;
            }

            if (_virtualPlayerCamera == null)
            {
                Log.Warning(LogTags.Camera, "(Setting) VirtualPlayerCamera가 null입니다.");
                isValid = false;
            }

            if (_currentAsset == null)
            {
                Log.Warning(LogTags.Camera, "(Setting) 현재 카메라 에셋이 null입니다.");
                isValid = false;
            }

            Log.Info(LogTags.Camera, "(Setting) 카메라 설정 검증 결과: {0}", isValid ? "유효함" : "무효함");

            return isValid;
        }
    }
}