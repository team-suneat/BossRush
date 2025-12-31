using TeamSuneat.Data;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.CameraSystem.Impulse
{
    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class CameraImpulseSource : XBehaviour
    {
        public string Label;
        public CinemachineImpulseSource Source;
        public ImpulsePreset Preset;
        public float ForceDuration;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Source = GetComponent<CinemachineImpulseSource>();
        }

        public override void AutoNaming()
        {
            if (string.IsNullOrEmpty(Label))
            {
                SetGameObjectName("Impulse Source");
            }
            else
            {
                SetGameObjectName($"Impulse Source({Label})");
            }
        }

        private void Awake()
        {
            if (Source != null && Preset != null)
            {
                Source.DefaultVelocity = Preset.DefaultVelocity;
            }
        }

        public void Shake()
        {
            CameraManager.Instance.Shake(Source, Preset, ForceDuration);
        }
    }
}