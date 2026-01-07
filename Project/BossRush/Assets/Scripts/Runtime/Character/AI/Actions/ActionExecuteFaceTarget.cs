using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("타겟을 바라보는 명령을 실행합니다.")]
    public class ActionExecuteFaceTarget : ActionTask<Character>
    {
        // private CommandInfo command = new CommandInfo();

        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.TryFlip())
            // {
            //     if (agent.TargetVital != null)
            //     {
            //         agent.RefreshFlipWithTarget();
            //     }
            //     else
            //     {
            //         agent.RefreshFlip();
            //     }
            // }
            //
            // EndAction();
        }
    }
}