using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckFlashPossible : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.characterAnimator != null && agent.monsterFlashSystem != null)
            // {
            //     if (false == agent.characterAnimator.TryFlash())
            //     {
            //         return false;
            //     }
            //
            //     if (false == agent.monsterFlashSystem.TryFlash())
            //     {
            //         return false;
            //     }
            //
            //     if (agent.patternSystem != null)
            //     {
            //         if (agent.patternSystem.CurrentPatternStep.StepName >= PatternStepNames.Flash)
            //         {
            //             return false;
            //         }
            //     }
            //
            //     return true;
            // }
            //
            // return false;
        }

        protected override string info
        {
            get
            {
                return "플래시 가능 여부 확인 (사용 안 함)";
            }
        }
    }
}