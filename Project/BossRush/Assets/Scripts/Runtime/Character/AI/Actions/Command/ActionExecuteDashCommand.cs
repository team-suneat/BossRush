using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("대시 명령을 실행합니다.")]
    public class ActionExecuteDashCommand : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }
            agent.RequestDash();
            EndAction(true);
        }

        protected override string info
        {
            get
            {
                return "대시 명령 실행";
            }
        }
    }
}
