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

        protected override string info
        {
            get
            {
                return "강제 속도 적용 여부 확인 (사용 안 함)";
            }
        }
    }
}