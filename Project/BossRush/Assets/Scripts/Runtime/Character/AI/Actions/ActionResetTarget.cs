using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class ActionResetTarget : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            agent.ResetTarget();
            EndAction();

            // agent.detectSystem.StartDetect();
        }

        protected override string info
        {
            get
            {
                return "캐릭터의 목표 캐릭터 초기화";
            }
        }
    }
}