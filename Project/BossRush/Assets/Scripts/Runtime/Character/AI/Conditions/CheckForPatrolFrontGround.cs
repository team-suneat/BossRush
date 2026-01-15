using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckForPatrolFrontGround : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // bool facingRight = agent.monsterPatrolSystem.TargetPosition.x > agent.transform.position.x;
            // if (agent.collisionController.IsFrontLeftGround && !facingRight)
            //     return true;
            //
            // if (agent.collisionController.IsFrontRightGround && facingRight)
            //     return true;
            //
            // return false;
        }

        protected override string info
        {
            get
            {
                return "순찰 전방 지면 확인 (사용 안 함)";
            }
        }
    }
}