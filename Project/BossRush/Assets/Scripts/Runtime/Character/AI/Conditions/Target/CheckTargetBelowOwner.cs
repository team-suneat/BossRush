using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Target")]
    public class CheckTargetBelowOwner : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.TargetVital != null)
            // {
            //     UnityEngine.Vector3 ownerPosition = agent.position;
            //
            //     UnityEngine.Vector3 targetPosition = agent.TargetVital.position;
            //
            //     if (agent.chaseSystem != null)
            //     {
            //         if (agent.chaseSystem.Type == ChaseSystem.Types.Kiting)
            //         {
            //             return ownerPosition.y > targetPosition.y + agent.chaseSystem.KitingMinHeight;
            //         }
            //     }
            //
            //     return ownerPosition.y > targetPosition.y;
            // }
            //
            // return false;
        }
    }
}