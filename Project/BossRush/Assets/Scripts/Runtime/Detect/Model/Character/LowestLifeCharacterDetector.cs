using UnityEngine;

namespace TeamSuneat
{
    public class LowestHealthCharacterDetector : ICharacterDetector
    {
        public override Character DoDetectCharacter(Vector3 detectPosition)
        {
            float minHealth = int.MaxValue;
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

                if (!candidate.MyVital.IsAlive)
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

                if (candidate.CurrentHealth < minHealth)
                {
                    minHealth = candidate.CurrentHealth;
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