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

        internal static PlayerCharacter SpawnPlayerCharacter(Vector3 spawnPosition, Transform parent)
        {
            PlayerCharacter player = SpawnPrefab<PlayerCharacter>("PlayerCharacter", parent);
            if (player != null)
            {
                player.transform.localPosition = Vector3.zero;
                player.transform.localRotation = Quaternion.identity;
                player.transform.localScale = Vector3.one;
                player.transform.position = spawnPosition;
            }

            return player;
        }
    }
}