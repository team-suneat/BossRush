using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class ActionDash : ActionTask<Character>
    {
        // private CommandInfo command = new CommandInfo();

        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // command.Reset();
            // command.dash = true;
            //
            // agent.commandSystem.AddCurrentCommandInfo(command);
            // agent.commandSystem.ExecuteCommand();
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "대시 (사용 안 함)";
            }
        }
    }
}