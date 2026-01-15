using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Animation")]
    public class CheckPlayingAnimation : ConditionTask<Character>
    {
        public CharacterState StateName;

        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null)
            // {
            //     return agent.characterAnimator.CheckPlayingAnimation(AnimationName);
            // }
            //
            // return false;
        }

        protected override string info
        {
            get
            {
                return $"애니메이션 재생 중 확인 (사용 안 함): {StateName}";
            }
        }
    }
}