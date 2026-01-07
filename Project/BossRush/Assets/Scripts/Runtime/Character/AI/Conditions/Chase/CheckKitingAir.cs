using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class CheckKitingAir : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.chaseSystem != null)
            // {
            //     return agent.chaseSystem.TryKitingAir();
            // }
            //
            // return false;
        }
    }
}