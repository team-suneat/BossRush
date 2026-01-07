using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class ActionAttackToTarget : ActionTask<Character>
    {
        // private CommandInfo command = new CommandInfo();

        protected override void OnExecute()
        {
            // command.Reset();
            // command.attackDown = true;
            // 
            // if (agent.commandSystem != null)
            // {
            //     agent.commandSystem.AddCurrentCommandInfo(command);
            //     agent.commandSystem.ExecuteCommand();
            // }

            EndAction();
        }
    }
}