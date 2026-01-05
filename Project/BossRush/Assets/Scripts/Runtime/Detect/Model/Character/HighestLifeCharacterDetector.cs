
using UnityEngine;

namespace TeamSuneat
{
    public class HighestLifeCharacterDetector : ICharacterDetector
    {
        public override Character DoDetectCharacter(Vector3 detectPosition)
        {
            float maxLife = 0;
            float distanceBetweenTarget;

            Character candidate;
            Character targetCharacter = null;

            for (int i = 0; i < CharacterManager.Instance.MonsterCount; i++)
            {
                candidate = CharacterManager.Instance.FindMonster(i);

                if (OwnerSID != null && candidate.SID == OwnerSID)
                {
                    continue;
                }

                if (candidate.CurrentLife == 0)
                {
                    continue;
                }

                if (!LayerEx.IsInMask(candidate.Layer, TargetMask))
                {
                    continue;
                }

                distanceBetweenTarget = Vector2.Distance(detectPosition, candidate.position);

                if (MaxDistance > 0 && distanceBetweenTarget > MaxDistance)
                {
                    continue;
                }

                if (candidate.CurrentLife > maxLife)
                {
                    maxLife = candidate.CurrentLife;
                    targetCharacter = candidate;
                }
            }

            return targetCharacter;
        }

        public override Character[] DoDetectCharacters(Vector3 detectPosition)
        {
            return null;
        }
    }
}