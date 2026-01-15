using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("점프 명령을 실행합니다.")]
    public class ActionExecuteJumpCommand : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }
            agent.RequestJump(true);
            EndAction(true);
        }

        protected override string info
        {
            get
            {
                return "점프 명령 실행";
            }
        }
    }
}
