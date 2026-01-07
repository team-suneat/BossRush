using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("공격 명령을 실행합니다.")]
    public class ActionExecuteAttackCommand : ActionTask<Character>
    {
        // private CommandInfo command = new CommandInfo();

        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.commandSystem != null)
            // {
            //     command.Reset();
            //
            //     command.attackDown = true;
            //
            //     agent.commandSystem.AddCurrentCommandInfo(command);
            //
            //     agent.commandSystem.ExecuteCommand();
            // }
            //
            // EndAction();
        }
    }
}