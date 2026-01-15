using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("명령을 초기화합니다.")]
    public class ActionResetCommand : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }
            agent.SetHorizontalInput(0f);
            agent.SetVerticalInput(0f);
            EndAction(true);
        }

        protected override string info
        {
            get
            {
                return "명령 초기화";
            }
        }
    }
}