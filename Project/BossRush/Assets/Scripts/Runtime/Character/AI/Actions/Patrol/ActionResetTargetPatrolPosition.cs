using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Patrol")]
    public class ActionResetTargetPatrolPosition : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.monsterPatrolSystem != null)
            // {
            //     agent.monsterPatrolSystem.ResetValues();
            // }
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "순찰 위치 리셋 (사용 안 함)";
            }
        }
    }
}