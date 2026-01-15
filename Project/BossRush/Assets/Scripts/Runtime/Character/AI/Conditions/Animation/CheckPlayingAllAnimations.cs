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
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

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

            result = null;

            return false;
        }

        protected override string info
        {
            get
            {
                return "모든 애니메이션 재생 중 확인 (사용 안 함)";
            }
        }
    }
}