using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attak")]
    [Description("타겟이 공격영역내에 있는지 확인합니다.")]
    public class CheckTargetInAttackableArea : ConditionTask<Character>
    {
        public int attackOrder = 0;

        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.attackSystem != null)
            // {
            //     return agent.attackSystem.CheckTargetInAttackableArea(attackOrder);
            // }
            //
            // return false;
        }
    }
}