using NodeCanvas.Framework;

using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    public class CheckDashPossible : ConditionTask<Character>
    {
        public string WarningMessage;

        protected override bool OnCheck()
        {
            // 더 이상 사용되지 않음 - 항상 false 반환
            return false;

            // WarningMessage = null;
            //
            // if (false == agent.characterAnimator.TryDash())
            // {
            //     WarningMessage = "대시를 할 수 있는 애니메이션 상태가 아닙니다.";
            //     return false;
            // }
            //
            // if (agent.attackSystem.CheckDashAttackCooldown())
            // {
            //     WarningMessage = "대시의 재사용 대기 중입니다.";
            //     return false;
            // }
            //
            // if (false == agent.IsFlying)
            // {
            //     if (false == agent.collisionController.IsGrounded)
            //     {
            //         WarningMessage = "지상 캐릭터는 공중에서 대시가 불가능합니다.";
            //         return false;
            //     }
            // }
            //
            // AttackEntity entity = agent.attackSystem.GetAttackEntity(CharacterAttackTypes.DashAttack);
            // if (entity != null)
            // {
            //     if (false == entity.CheckTargetInArea())
            //     {
            //         WarningMessage = "타겟이 대시 가능한 영역 안에 있지 않습니다.";
            //         return false;
            //     }
            // }
            //
            // return true;
        }
    }
}