using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Target")]
    public class CheckTargetAlive : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            if (agent.TargetCharacter != null)
            {
                return agent.TargetCharacter.IsAlive;
            }

            return false;
        }

        protected override string info
        {
            get
            {
                return "타겟 생존 확인";
            }
        }
    }
}