using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    [Description("전역 공격 재사용 대기시간인지 확인합니다.")]
    public class CheckForGlobalAttackCooldown : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            if (agent.Attack != null)
            {
                if (agent.Attack.CheckGlobalCooldown())
                {
                    return true;
                }
            }

            return false;
        }

        protected override string info
        {
            get
            {
                return "전역 공격 재사용 대기시간 확인";
            }
        }
    }
}
