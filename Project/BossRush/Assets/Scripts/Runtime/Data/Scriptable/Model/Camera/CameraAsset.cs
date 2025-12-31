using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "Camera", menuName = "TeamSuneat/Scriptable/Camera")]
    public class CameraAsset : XScriptableObject
    {
        [Title("#Character")]
        public float OrthographicSize;

        [Range(0f, 1f)]
        public float LookaheadTime;

        [Range(0f, 30f)]
        public float LookaheadSmooting;

        [Range(0f, 20f)]
        public float XDamping;

        [Range(0f, 20f)]
        public float YDamping;

        [Title("#Soft Zone")]
        [Range(0f, 2f)]
        public float SoftZoneWidth;

        [Range(0f, 2f)]
        public float SoftZoneHeight;

        [Title("#Blend")]
        [Range(0f, 2f)]
        public float DefaultBlendTime;
    }
}