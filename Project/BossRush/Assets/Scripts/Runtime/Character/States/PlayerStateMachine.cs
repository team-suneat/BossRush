using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerStateMachine : CharacterStateMachine
    {
        private CharacterPhysics _physics;
        private PlayerInput _input;
        private CharacterAnimator _animator;

        protected override void Awake()
        {
            base.Awake();

            _physics = GetComponent<CharacterPhysics>();
            _input = GetComponent<PlayerInput>();
            _animator = GetComponentInChildren<CharacterAnimator>();
        }

        protected override void InitializeStates()
        {
            _states = new Dictionary<CharacterState, ICharacterState>
            {
                // 이동 상태
                { CharacterState.Idle, new IdleState(this, _physics, _input) },
                { CharacterState.Walk, new WalkState(this, _physics, _input) },
                { CharacterState.Jumping, new JumpState(this, _physics, _input) },
                { CharacterState.Falling, new FallingState(this, _physics, _input) },
                { CharacterState.Dash, new DashState(this, _physics,_animator, _input) },
                { CharacterState.Attack, new AttackState(this, _physics, _animator, _input) },
                { CharacterState.Parry, new ParryState(this, _physics, _animator, _input) },

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
            if (_input == null) return;

            // 점프 입력 감지
            if (_input.IsJumpPressed)
            {
                // 아래 방향 키가 눌려있지 않으면 일반 점프
                if (!_input.IsDownInputPressed)
                {
                    RequestJump();
                }
            }

            // 대시 입력 감지
            if (_input.IsDashPressed)
            {
                RequestDash();
            }

            // 공격 입력 감지
            if (_input.IsAttackPressed)
            {
                RequestAttack();
            }

            // 패리 입력 감지
            if (_input.IsParryPressed)
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
                if (!_character.MyVital.CanParry)
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

        public void EnableComboInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is AttackState attack)
                {
                    attack.EnableComboQueue();
                }
            }
        }

        public void DisableComboInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is AttackState attack)
                {
                    attack.DisableComboQueue();
                }
            }
        }

        public void EnableFlipInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is AttackState attack)
                {
                    attack.EnableFlip();
                }
            }
        }

        public void DisableFlipInput()
        {
            if (_states.TryGetValue(CharacterState.Attack, out ICharacterState attackState))
            {
                if (attackState is AttackState attack)
                {
                    attack.DisableFlip();
                }
            }
        }
    }
}