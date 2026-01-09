using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace TeamSuneat
{
    [Category("@TeamSuneat")]
    [Description("CharacterAnimator, AttackSystem")]
    public class CheckAttackPossible : ConditionTask<Character>
    {
        protected override bool OnCheck()
        {
            // CharacterAnimator가 공격 중이 아닌지 확인
            if (agent.CharacterAnimator != null)
            {
                if (agent.CharacterAnimator.IsAttacking)
                {
                    return false;
                }
            }

            // StateMachine이 공격 상태가 아닌지 확인
            if (agent.StateMachine != null)
            {
                if (agent.StateMachine.CurrentState == CharacterState.Attack)
                {
                    return false;
                }
            }

            // AttackSystem이 있는지 확인
            if (agent.Attack == null)
            {
                return false;
            }

            // 조건 상태(Dead, Stunned 등)가 아닌지 확인
            if (agent.StateMachine != null)
            {
                CharacterState currentState = agent.StateMachine.CurrentState;
                if (currentState == CharacterState.Dead || 
                    currentState == CharacterState.Stunned ||
                    currentState == CharacterState.ControlledMovement)
                {
                    return false;
                }
            }

            return true;
        }
    }
}