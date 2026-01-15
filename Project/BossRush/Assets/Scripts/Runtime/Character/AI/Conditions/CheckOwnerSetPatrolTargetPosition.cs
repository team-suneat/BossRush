using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckOwnerSetPatrolTargetPosition : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.monsterPatrolSystem != null)
            // {
            //     return agent.monsterPatrolSystem.IsVaildTargetPosition();
            // }
            //
            // return false;
        }

        protected override string info
        {
            get
            {
                return "순찰 목표 위치 설정 여부 확인 (사용 안 함)";
            }
        }
    }
}