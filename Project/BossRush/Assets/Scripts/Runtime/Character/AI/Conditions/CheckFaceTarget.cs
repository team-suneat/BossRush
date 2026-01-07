using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat/Face")]
    [Color("FFFF00")]
    [Description("목표를 바라보고 있는지 확인합니다.")]
    public class CheckFaceTarget : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // if (agent != null && agent.TargetVital != null)
            // {
            //     if (UnityEngine.Mathf.Approximately(agent.TargetVital.position.x, agent.position.x))
            //     {
            //         return true;
            //     }
            //     else
            //     {
            //         bool facingRight = agent.TargetVital.position.x > agent.position.x;
            //
            //         return agent.FacingRight == facingRight;
            //     }
            // }
            //
            // return false;
        }
    }
}