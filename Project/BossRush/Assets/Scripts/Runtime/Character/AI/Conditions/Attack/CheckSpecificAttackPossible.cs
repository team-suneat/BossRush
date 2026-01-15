using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    [Description("CharacterAnimator, AttackSystem")]
    public class CheckSpecificAttackPossible : ConditionTask<Character>
    {
        public int AttackOrder;

        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (false == agent.characterAnimator.TryAttack())
            // {
            //     return false;
            // }
            //
            // if (agent.attackSystem.CheckAttackCooldown(AttackOrder))
            // {
            //     return false;
            // }
            //
            // return true;
        }

        protected override string info
        {
            get
            {
                return $"특정 공격 가능 여부 확인 (사용 안 함): 순서 {AttackOrder}";
            }
        }
    }
}