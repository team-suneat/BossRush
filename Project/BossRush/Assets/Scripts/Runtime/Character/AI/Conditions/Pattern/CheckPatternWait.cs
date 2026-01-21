using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class CheckPatternWait : ConditionTask<BossCharacter>
    {
        protected override bool OnCheck()
        {
            if (agent.Pattern != null)
            {
                return agent.Pattern.IsWaitPattern;
            }
            else
            {
                return false;
            }
        }

        protected override string info
        {
            get
            {
                return "패턴 대기 상태 확인";
            }
        }
    }
}