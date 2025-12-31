using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static AttackEntity SpawnAttackEntity(HitmarkNames hitmarkName, Transform transform)
        {
            return SpawnPrefab<AttackEntity>($"AttackEntity({hitmarkName})", transform);
        }

        internal static BuffEntity SpawnBuffEntity(Transform transform)
        {
            return null;
        }

        internal static BuffEntity SpawnBuffEntity(BuffNames buffName, Transform transform)
        {
            return null;
        }

        internal static PassiveEntity SpawnPassiveEntity(PassiveNames passiveName, Transform transform)
        {
            return null;
        }
    }
}