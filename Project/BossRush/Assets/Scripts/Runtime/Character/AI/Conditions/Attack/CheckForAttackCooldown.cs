using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attak")]
    [Description("공격 재사용 대기시간인지 확인합니다.")]
    public class CheckForAttackCooldown : ConditionTask<Character>
    {
        public int order;

        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.attackSystem != null)
            // {
            //     return agent.attackSystem.CheckAttackCooldown(order);
            // }
            //
            // return false;
        }
    }
}