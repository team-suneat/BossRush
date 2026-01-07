using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Physics")]
    [Description("PhysicsController")]
    public class CheckForceVelocity : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // return agent.CheckAppliedAnyForceVelocity();
        }
    }
}