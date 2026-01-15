using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Pattern")]
    public class CheckPatternWait : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.patternSystem != null)
            // {
            //     return agent.patternSystem.IsWaitPattern;
            // }
            // else
            // {
            //     return false;
            // }
        }

        protected override string info
        {
            get
            {
                return "패턴 대기 상태 확인 (사용 안 함)";
            }
        }
    }
}