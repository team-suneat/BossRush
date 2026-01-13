using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    [Description("타겟이 공격가능한 영역 안에 있는지 확인합니다.")]
    public class CheckTargetInAttackableArea : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            if (agent.Attack != null)
            {
                if (agent.Attack.CheckTargetInAttackableArea())
                {
                    return true;
                }
            }

            return false;
        }
    }
}