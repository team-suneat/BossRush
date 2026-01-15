using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Chase")]
    public class CheckChaseAir : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent.chaseSystem != null)
            // {
            //     return agent.chaseSystem.TryChaseAir();
            // }
            //
            // return false;
        }

        protected override string info
        {
            get
            {
                return "공중 추적 가능 여부 확인 (사용 안 함)";
            }
        }
    }
}