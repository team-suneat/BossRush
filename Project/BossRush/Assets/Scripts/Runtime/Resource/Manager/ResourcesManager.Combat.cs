using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static MonsterCharacter SpawnMonsterCharacter(CharacterNames characterName, Transform parent)
        {
            string prefabName = characterName.ToString();
            MonsterCharacter monster = SpawnPrefab<MonsterCharacter>(prefabName, parent);
            if (monster != null)
            {
                monster.ResetLocalTransform();
            }

            return monster;
        }

        internal static MonsterCharacter SpawnMonsterCharacter(GameObject prefab, Transform parent)
        {
            MonsterCharacter monster = Instantiate<MonsterCharacter>(prefab, parent);
            if (monster != null)
            {
                monster.ResetLocalTransform();
            }

            return monster;
        }

        internal static PlayerCharacter SpawnPlayerCharacter(GameObject prefab, Vector3 spawnPosition, Transform parent)
        {
            PlayerCharacter player = Instantiate<PlayerCharacter>(prefab, parent);
            if (player != null)
            {
                player.ResetLocalTransform();
                player.transform.position = spawnPosition;
            }

            return player;
        }
    }
}