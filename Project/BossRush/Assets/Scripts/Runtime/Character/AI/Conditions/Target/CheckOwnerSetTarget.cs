using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Target")]
    public class CheckOwnerSetTarget : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            return agent.TargetCharacter != null;
        }

        protected override string info
        {
            get
            {
                return "타겟 설정 여부 확인";
            }
        }
    }
}