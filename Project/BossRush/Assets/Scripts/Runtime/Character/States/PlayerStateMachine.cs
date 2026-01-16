using System.Collections.Generic;

namespace TeamSuneat
{
    public class PlayerStateMachine : CharacterStateMachine
    {
        private CharacterPhysics _physics;
        private CharacterAnimator _animator;

        protected override void Awake()
        {
            base.Awake();

            _physics = GetComponent<CharacterPhysics>();
            _animator = GetComponentInChildren<CharacterAnimator>();
        }

        public override void InitializeStates()
        {
            _states = new Dictionary<CharacterState, ICharacterState>
            {
                // 이동 상태
                { CharacterState.Idle, new IdleState(this, _physics, _character) },
                { CharacterState.Walk, new WalkState(this, _physics, _character) },
                { CharacterState.Jumping, new JumpState(this, _physics, _character) },
                { CharacterState.Falling, new FallingState(this, _physics, _character) },
                { CharacterState.Dash, new DashState(this, _physics, _animator, _character) },
                { CharacterState.Attack, new PlayerAttackState(this, _physics, _animator, _character) },
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
            if (cmd.IsJumpPressed)
            {
                // 아래 방향 키가 눌려있지 않으면 일반 점프
                if (!cmd.IsDownInputPressed)
                {
                    RequestJump();
                }
            }

            // 대시 입력 감지
            if (cmd.IsDashPressed)
            {
                RequestDash();
            }

            // 공격 입력 감지 (즉시 처리)
            if (cmd.IsAttackPressed)
            {
                RequestAttack();
            }

            // 공격 입력 버퍼 소비
            TryConsumeAttackBuffer();

            // 패리 입력 감지
            if (cmd.IsParryPressed)
            {
                RequestParry();
            }
        }

        //

        // 공격 버퍼 소비
        private void TryConsumeAttackBuffer()
        {
            if (_character == null) return;

            var cmd = _character.Command;

            if (!cmd.IsAttackBuffered)
                return;

            // 공격 시작 가능한 상태 명시적 정의
            // 초기 구현: Idle, Walk, Jumping, Falling만 허용
            if (CurrentState == CharacterState.Idle ||
                CurrentState == CharacterState.Walk ||
                CurrentState == CharacterState.Jumping ||
                CurrentState == CharacterState.Falling)
            {
                Log.Info(LogTags.Input_Command, "[공격 입력 버퍼] 버퍼 소비 시도. 현재 상태: {0}", CurrentState);
                cmd.ConsumeAttackBuffer();
                RequestAttack();
            }
            else
            {
                Log.Info(LogTags.Input_Command, "[공격 입력 버퍼] 버퍼 소비 실패. 공격 불가능한 상태: {0}", CurrentState);
            }
        }

        private void RequestAttack()
        {
            if (_states.TryGetValue(CurrentState, out ICharacterState currentState))
            {
                if (CurrentState != CharacterState.Attack)
                {
                    ChangeState(CharacterState.Attack);
                }
            }
        }

        protected override void RequestDash()
        {
            if (_character != null && _character.MyVital != null)
            {
                if (!_character.MyVital.TryUsePulse())
                {
                    return;
                }
            }

            base.RequestDash();
        }

        private void RequestParry()
        {
            if (_character != null && _character.MyVital != null)
            {
                if (!_character.MyVital.TryUsePulse())
                {
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

        //

        public void EnableComboInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is PlayerAttackState attack)
                {
                    attack.EnableComboQueue();
                }
            }
        }

        public void DisableComboInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is PlayerAttackState attack)
                {
                    attack.DisableComboQueue();
                }
            }
        }

        public void EnableFlipInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is PlayerAttackState attack)
                {
                    attack.EnableFlip();
                }
            }
        }

        public void DisableFlipInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is PlayerAttackState attack)
                {
                    attack.DisableFlip();
                }
            }
        }
    }
}