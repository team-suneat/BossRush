using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("패리 명령을 실행합니다.")]
    public class ActionExecuteParryCommand : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }
            agent.RequestParry();
            EndAction(true);
        }

        protected override string info
        {
            get
            {
                return "패리 명령 실행";
            }
        }
    }
}
