using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Camera Impurse Preset", menuName = "TeamSuneat/Preset/Camera Impurse")]
    public class ImpulsePreset : XScriptableObject
    {
        [Title("#Impulse Source Setting")]
        public float ImpactTime;

        public float ImpactForce;
        public Vector3 DefaultVelocity = new Vector3(0f, -1f, 0f);
        public CinemachineImpulseDefinition.ImpulseShapes ImpulseShape;

        [EnableIf("ImpulseShape", CinemachineImpulseDefinition.ImpulseShapes.Custom)]
        [SuffixLabel("For Custom Shape")]
        public AnimationCurve ImpurseCurve;

        [Title("#Impulse Listener Setting")]
        public float ListenerAmplitude = 1f;

        public float ListenerFrequency = 1f;
        public float ListenerDuration = 0.2f;

        public override void OnLoadData()
        {
            base.OnLoadData();
        }
    }
}