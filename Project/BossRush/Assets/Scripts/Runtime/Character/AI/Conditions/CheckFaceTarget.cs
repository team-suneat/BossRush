using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Face")]
    [Color("FFFF00")]
    [Description("목표를 바라보고 있는지 확인합니다.")]
    public class CheckFaceTarget : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            if (agent != null && agent.TargetCharacter != null)
            {
                bool isTargetRight = agent.TargetCharacter.position.x > agent.position.x;
                if (agent.IsFacingRight == isTargetRight)
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
                return "타겟을 바라보고 있는지 확인";
            }
        }
    }
}