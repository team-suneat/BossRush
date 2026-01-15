using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class ActionPatrol : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // agent.monsterPatrolSystem.DoPatrol();
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "순찰 (사용 안 함)";
            }
        }
    }
}