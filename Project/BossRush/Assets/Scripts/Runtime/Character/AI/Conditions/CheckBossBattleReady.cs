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

        protected override string info
        {
            get
            {
                return "보스 전투 준비 상태 확인 (사용 안 함)";
            }
        }
    }
}