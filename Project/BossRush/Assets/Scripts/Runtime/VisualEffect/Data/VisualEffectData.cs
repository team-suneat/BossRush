using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public enum PointTypes
    {
        None,
        OwnerCharacter,
        OwnerProjectile,
    }

    [System.Serializable]
    public struct VisualEffectData
    {
        public GameObject Prefab;

        public PointTypes SpawnPointType;

        [DisableIf("SpawnPointType", PointTypes.None)]
        public Vector3 SpawnOffset;

        public PointTypes OriginPointType;

        [DisableIf("OriginPointType", PointTypes.None)]
        public Vector3 OriginOffset;

        public PointTypes TargetPointType;

        [DisableIf("TargetPointType", PointTypes.None)]
        public Vector3 TargetOffset;
    }
}