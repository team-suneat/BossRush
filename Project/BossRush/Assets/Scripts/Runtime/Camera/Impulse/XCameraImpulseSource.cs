using TeamSuneat.CameraSystem.Core;
using TeamSuneat.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Impulse
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class XCameraImpulseSource : XBehaviour
    {
        public GameImpulseType Type;
        public CinemachineImpulseSource Source;
        public GameImpulseType PresetName;

        private CameraImpulseAsset _asset;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Source = GetComponent<CinemachineImpulseSource>();
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"Impulse Source({Type})");
        }

        private void Awake()
        {
            LoadPreset();

            if (Source != null && _asset != null)
            {
                Source.DefaultVelocity = _asset.DefaultVelocity;
            }
        }

        private void LoadPreset()
        {
            CameraImpulseAsset asset = ScriptableDataManager.Instance.GetCameraImpulseAsset(PresetName);
            if (asset.IsValid())
            {
                _asset = asset;
            }
            else
            {
                Log.Warning(LogTags.Camera, "CameraImpulseAsset을 찾을 수 없거나 Preset이 설정되지 않았습니다: {0}", PresetName);
            }
        }

        public void Shake()
        {
            if (_asset == null)
            {
                LoadPreset();
            }

            if (_asset != null)
            {
                CameraManager.Instance.Shake(Source, _asset);
            }
        }
    }
}