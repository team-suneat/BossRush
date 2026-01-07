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

            // if (agent == null)
            // {
            //     result = "Character를 찾을 수 없습니다.";
            //
            //     return false;
            // }
            //
            // if (agent.CharacterAnimator == null)
            // {
            //     result = "Character의 Animator를 찾을 수 없습니다.";
            //     return false;
            // }
            //
            // if (agent.CharacterAnimator.CheckPlayingAllAnimations())
            // {
            //     result = "다른 애니메이션이 재생중입니다.";
            //
            //     return true;
            // }
            //
            // result = null;
            //
            // return false;
        }
    }
}