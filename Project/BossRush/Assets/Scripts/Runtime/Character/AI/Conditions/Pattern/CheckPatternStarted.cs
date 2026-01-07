using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class CheckPatternStarted : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.patternSystem != null)
            // {
            //     if (agent.patternSystem.IsStartPattern)
            //     {
            //         Log.Spare("CheckPatternStarted: {0}.", true.ToBoolString());
            //
            //         return true;
            //     }
            //     else
            //     {
            //         Log.Spare("CheckPatternStarted: {0}.", false.ToBoolString());
            //     }
            // }
            //
            // return false;
        }
    }
}