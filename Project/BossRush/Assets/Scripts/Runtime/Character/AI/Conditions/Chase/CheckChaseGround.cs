using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class CheckChaseGround : ConditionTask<BossCharacter>
    {
        protected override bool OnCheck()
        {
            if (agent.Chase != null)
            {
                if (false == agent.Chase.TryChaseInGround())
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        protected override string info
        {
            get
            {
                return "지상 추적 가능 여부 확인";
            }
        }
    }
}