using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Boss")]
    public class CheckBossBattleReady : ConditionTask<BossCharacter>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null)
            // {
            //     return !agent.characterAnimator.CheckPlayingAnimation(PlayingAnimationStates.BattleReady);
            // }
            //
            // return false;
        }
    }
}