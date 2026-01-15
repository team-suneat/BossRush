using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class ActionChaseGround : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.chaseSystem != null)
            // {
            //     agent.chaseSystem.ChaseInGround();
            // }
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "지상 추적 (사용 안 함)";
            }
        }
    }
}