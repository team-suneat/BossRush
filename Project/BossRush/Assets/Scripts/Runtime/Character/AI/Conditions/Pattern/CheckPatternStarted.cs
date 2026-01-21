using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class CheckPatternStarted : ConditionTask<BossCharacter>
    {
        protected override bool OnCheck()
        {
             if (agent.Pattern != null)
             {
                 if (agent.Pattern.IsStartPattern)
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
                return "패턴 시작 여부 확인";
            }
        }
    }
}