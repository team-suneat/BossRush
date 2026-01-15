using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckPatrolPossible : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // return agent.monsterPatrolSystem.TryPatrol();
        }

        protected override string info
        {
            get
            {
                return "순찰 가능 여부 확인 (사용 안 함)";
            }
        }
    }
}