using System.Collections.Generic;

namespace TeamSuneat
{
    public class MonsterStateMachine : CharacterStateMachine
    {
        private CharacterPhysics _physics;
        private CharacterAnimator _animator;

        protected override void Awake()
        {
            base.Awake();
            _physics = GetComponent<CharacterPhysics>();
            _animator = GetComponentInChildren<CharacterAnimator>();
        }

        protected override void InitializeStates()
        {
            _states = new Dictionary<CharacterState, ICharacterState>
            {
                // 이동 상태
                { CharacterState.Idle, new IdleState(this, _physics, _character) },
                { CharacterState.Walk, new WalkState(this, _physics, _character) },
                { CharacterState.Jumping, new JumpState(this, _physics, _character) },
                { CharacterState.Falling, new FallingState(this, _physics, _character) },
                { CharacterState.Dash, new DashState(this, _physics, _animator, _character) },
                { CharacterState.Attack, new AttackState(this, _physics, _animator, _character) },
                { CharacterState.Parry, new ParryState(this, _physics, _animator, _character) },

                // 조건 상태
                { CharacterState.Dead, new DeadState(this, _character) },
                { CharacterState.Stunned, new StunnedState(this, _character) },
                { CharacterState.ControlledMovement, new ControlledMovementState(this, _character) }
            };

            // 초기 상태 설정
            ChangeState(CharacterState.Idle);
        }

        protected override void HandleInput()
        {
            if (_character == null) return;

            CharacterCommand cmd = _character.Command;

            // 점프 입력 감지
            if (cmd.IsJumpPressed && !cmd.IsDownInputPressed)
            {
                RequestJump();
            }

            // 대시 입력 감지
            if (cmd.IsDashPressed)
            {
                RequestDash();
            }

            // 공격 입력 감지
            if (cmd.IsAttackPressed)
            {
                RequestAttack();
            }

            // 패리 입력 감지
            if (cmd.IsParryPressed)
            {
                RequestParry();
            }
        }

        public virtual void RequestAttack()
        {
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                if (CurrentState != CharacterState.Attack)
                {
                    ChangeState(CharacterState.Attack);
                }
            }
        }

        public virtual void RequestParry()
        {
            // 패링 가능 여부 검사 (온전한 한 칸이 있는지 확인)
            if (_character != null && _character.MyVital != null && _character.MyVital.Pulse != null)
            {
                if (!_character.MyVital.CanUsePulse)
                {
                    // 패링 게이지가 부족하여 상태 변경 불가
                    return;
                }
            }

            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                if (CurrentState != CharacterState.Parry)
                {
                    ChangeState(CharacterState.Parry);
                }
            }
        }
    }
}