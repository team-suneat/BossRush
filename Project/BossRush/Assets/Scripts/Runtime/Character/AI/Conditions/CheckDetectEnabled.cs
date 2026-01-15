using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckDetectEnabled : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // return agent.detectSystem.CheckActive();
        }

        protected override string info
        {
            get
            {
                return "탐지 활성화 여부 확인 (사용 안 함)";
            }
        }
    }
}