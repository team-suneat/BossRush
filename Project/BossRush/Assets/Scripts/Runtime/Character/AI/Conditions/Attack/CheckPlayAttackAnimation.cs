using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Attak")]
    [Description("공격 애니메이션을 재생할 수 있는지 확인합니다.")]
    public class CheckPlayAttackAnimation : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null)
            // {
            //     return agent.characterAnimator.TryAttack();
            // }
            //
            // return false;
        }
    }
}