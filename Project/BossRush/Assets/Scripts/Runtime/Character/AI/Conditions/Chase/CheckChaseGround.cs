using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class CheckChaseGround : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.chaseSystem != null)
            // {
            //     if (false == agent.chaseSystem.TryChaseInGround())
            //     {
            //         return false;
            //     }
            //
            //     if (false == agent.characterAnimator.TryMove())
            //     {
            //         return false;
            //     }
            //
            //     return true;
            // }
            //
            // return false;
        }
    }
}