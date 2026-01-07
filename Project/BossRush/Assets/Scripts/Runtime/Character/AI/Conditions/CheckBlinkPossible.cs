using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckBlinkPossible : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null && agent.blinkSystem != null)
            // {
            //     if (false == agent.characterAnimator.TryBlink())
            //     {
            //         return false;
            //     }
            //
            //     if (false == agent.blinkSystem.TryBlink())
            //     {
            //         return false;
            //     }
            //
            //     return true;
            // }
            //
            // return false;
        }
    }
}