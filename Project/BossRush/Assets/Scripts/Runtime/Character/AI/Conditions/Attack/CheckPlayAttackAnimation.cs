using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attack")]
    [Description("공격 애니메이션을 재생할 수 있는지 확인합니다.")]
    public class CheckPlayAttackAnimation : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            if (agent.CharacterAnimator != null)
            {
                return agent.CharacterAnimator.IsAttacking;
            }

            return false;
        }

        protected override string info
        {
            get
            {
                return "공격 애니메이션 재생 중 확인";
            }
        }
    }
}