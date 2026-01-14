using Sirenix.OdinInspector;
using TeamSuneat;
using Unity.Cinemachine;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "CameraImpulseAsset", menuName = "TeamSuneat/Scriptable/CameraImpulse")]
    public class CameraImpulseAsset : XScriptableObject
    {
        public bool IsChangingAsset;

        [EnableIf("IsChangingAsset")]
        public GameImpulseType Type;

        [Title("#Impulse Source Setting")]
        public float ImpactTime;
        public float ImpactForce;
        public Vector3 DefaultVelocity = new Vector3(0f, -1f, 0f);
        public CinemachineImpulseDefinition.ImpulseShapes ImpulseShape;

        [ShowIf("ImpulseShape", CinemachineImpulseDefinition.ImpulseShapes.Custom)]
        [SuffixLabel("For Custom Shape")]
        public AnimationCurve ImpulseCurve;

        [Title("#Impulse Listener Setting")]
        public float ListenerAmplitude = 1f;
        public float ListenerFrequency = 1f;
        public float ListenerDuration = 0.2f;

        [Title("#Trauma System")]
        [Range(0f, 1f)]
        [Tooltip("Trauma 시스템에 기여하는 값 (0~1). 이 값이 trauma에 누적됩니다.")]
        public float TraumaContribution = 0.1f;

        public int TID => BitConvert.Enum32ToInt(Type);

#if UNITY_EDITOR

        public override void Validate()
        {
            base.Validate();

            if (!IsChangingAsset)
            {
                EnumEx.ConvertTo(ref Type, NameString);
            }
        }

        public override void Refresh()
        {
            if (Type != 0)
            {
                NameString = Type.ToString();
            }

            IsChangingAsset = false;
            base.Refresh();
        }

        public override void Rename()
        {
            Rename("CameraImpulse");
        }

#endif
    }
}