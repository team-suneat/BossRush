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
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.commandSystem != null)
            // {
            //     agent.commandSystem.ResetCommand();
            // }
            //
            // agent.SetDirectionalInput(0, 0);
            //
            // EndAction();
        }
    }
}