using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Patrol")]
    public class ActionSetTargetPatrolPosition : ActionTask<Character>
    {
        protected override void OnExecute()
        {
            // 더 이상 사용되지 않음 - 아무것도 하지 않음
            EndAction();

            // if (agent.monsterPatrolSystem != null)
            // {
            //     if (agent.IsFlying)
            //     {
            //         agent.monsterPatrolSystem.SetPatrolPositionInAir();
            //     }
            //     else if (false == agent.IsFlying && agent.collisionController.IsGrounded)
            //     {
            //         agent.monsterPatrolSystem.SetPatrolPositionInGround();
            //     }
            // }
            //
            // EndAction();
        }

        protected override string info
        {
            get
            {
                return "순찰 위치 설정 (사용 안 함)";
            }
        }
    }
}