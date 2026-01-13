using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    [Description("공격 재사용 대기시간인지 확인합니다.")]
    public class CheckForAttackCooldown : ConditionTask<Character>
    {
        public int order;

        protected override bool OnCheck()
        {
            if (agent.Attack != null)
            {
                if (agent.Attack.CheckAttackCooldown(order))
                {
                    return true;
                }
            }

            return false;
        }
    }
}