using System.Collections.Generic;

using UnityEngine;

namespace TeamSuneat
{
    public class NearestCharacterDetector : ICharacterDetector
    {
        public override Character DoDetectCharacter(Vector3 detectPosition)
        {
            float minDistance = float.MaxValue;
            float distanceBetweenTarget;

            Character candidate;
            Character targetCharacter = null;

            for (int i = 0; i < CharacterManager.Instance.MonsterCount; i++)
            {
                candidate = CharacterManager.Instance.FindMonster(i);

                if (candidate.SID == OwnerSID)
                {
                    continue;
                }

                if (candidate.MyVital.CurrentHealth == 0)
                {
                    continue;
                }

                if (!LayerEx.IsInMask(candidate.Layer, TargetMask))
                {
                    continue;
                }

                distanceBetweenTarget = UnityEngine.Vector2.Distance(detectPosition, candidate.position);

                if (MaxDistance > 0 && distanceBetweenTarget > MaxDistance)
                {
                    continue;
                }

                if (distanceBetweenTarget < minDistance)
                {
                    minDistance = distanceBetweenTarget;

                    targetCharacter = candidate;
                }
            }

            return targetCharacter;
        }

        public override Character[] DoDetectCharacters(Vector3 detectPosition)
        {
            float distanceBetweenTarget;
            Character candidate;
            List<Character> targetCharacters = new List<Character>();

            for (int i = 0; i < CharacterManager.Instance.MonsterCount; i++)
            {
                candidate = CharacterManager.Instance.FindMonster(i);

                if (OwnerSID != null && candidate.SID == OwnerSID)
                {
                    continue;
                }

                if (candidate.CurrentHealth == 0)
                {
                    continue;
                }

                if (!LayerEx.IsInMask(candidate.Layer, TargetMask))
                {
                    continue;
                }

                distanceBetweenTarget = UnityEngine.Vector2.Distance(detectPosition, candidate.position);

                if (MaxDistance > 0 && distanceBetweenTarget > MaxDistance)
                {
                    continue;
                }

                targetCharacters.Add(candidate);
            }

            NearestCharacterComparer comparer = new NearestCharacterComparer();

            comparer.Position = detectPosition;

            targetCharacters.Sort(comparer);

            return targetCharacters.ToArray();
        }
    }
}