using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    [Description("CharacterAnimator, AttackSystem")]
    public class CheckAttackPossible : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null)
            // {
            //     if (false == agent.characterAnimator.TryAttack())
            //     {
            //         return false;
            //     }
            // }
            //
            // if (agent.attackSystem != null)
            // {
            //     if (false == agent.attackSystem.CheckAttackCooldown())
            //     {
            //         return agent.attackSystem.CheckAttackable();
            //     }
            // }
            //
            // return false;
        }
    }
}