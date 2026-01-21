using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Animation")]
    public class CheckPlayingAllAnimations : ConditionTask<Character>
    {
        public string result;

        protected override bool OnCheck()
        {
            if (agent == null)
            {
                result = "Character를 찾을 수 없습니다.";
                return false;
            }

            if (agent.CharacterAnimator == null)
            {
                result = "Character의 Animator를 찾을 수 없습니다.";
                return false;
            }

            if (!agent.CharacterAnimator.IsPlayingAttackAnimation())
            {
                result = "공격 애니메이션 재생 중이 아닙니다.";
                return false;
            }

            result = "공격 애니메이션 재생 중입니다.";
            return true;
        }

        protected override string info
        {
            get
            {
                return "공격 애니메이션 재생 중인지 확인";
            }
        }
    }
}