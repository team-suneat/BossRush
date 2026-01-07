using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class ActionAttackWithDelay : ActionTask<Character>
    {
        // private CommandInfo command = new CommandInfo();
        public float DelayTime;
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // agent.StartXCoroutine(CoroutineEx.NextTimer(DelayTime, () =>
            // {
            //     command.Reset();
            //     command.attackDown = true;
            //
            //     agent.commandSystem.AddCurrentCommandInfo(command);
            //     agent.commandSystem.ExecuteCommand();
            // }));
            //
            // EndAction();
        }
    }
}