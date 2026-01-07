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
    }
}