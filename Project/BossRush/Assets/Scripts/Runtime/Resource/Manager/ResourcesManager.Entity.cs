using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        #region Entity

        public static AttackEntity SpawnAttackEntity(HitmarkNames hitmarkName, Transform parent)
        {
            string prefabName = string.Format("AttackEntity({0})", hitmarkName.ToString());
            GameObject spawnedObject = SpawnPrefab(prefabName, parent);
            if (spawnedObject != null)
            {
                spawnedObject.transform.localScale = Vector3.one;
                spawnedObject.transform.localPosition = Vector3.zero;
                spawnedObject.transform.localRotation = Quaternion.identity;

                return spawnedObject.GetComponent<AttackEntity>();
            }

            return null;
        }

        public static BuffEntity SpawnBuffEntity(BuffName buffName, Transform parent)
        {
            GameObject spawnedObject = SpawnPrefab("BuffEntity", parent);
            if (spawnedObject != null)
            {
                spawnedObject.transform.localScale = Vector3.one;
                spawnedObject.transform.localPosition = Vector3.zero;
                spawnedObject.transform.localRotation = Quaternion.identity;

                return spawnedObject.GetComponent<BuffEntity>();
            }

            return null;
        }

        #endregion Entity
    }
}