using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Command")]
    [Description("공격 명령을 실행합니다.")]
    public class ActionExecuteAttackCommand : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            if (agent == null)
            {
                EndAction(false);
                return;
            }
            agent.RequestAttack();
            EndAction(true);
        }

        protected override string info
        {
            get
            {
                return "공격 명령 실행";
            }
        }
    }
}