using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerMovementStateMachine : CharacterStateMachine
    {
        private CharacterPhysics _physics;
        private PlayerInput _input;

        protected override void Awake()
        {
            base.Awake();
            _physics = GetComponent<CharacterPhysics>();
            _input = GetComponent<PlayerInput>();
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
                { CharacterState.Dash, new DashState(this, _physics, _input) },

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
                Vector2 dashDirection = CalculateDashDirection();
                RequestDash(dashDirection);
            }
        }

        private Vector2 CalculateDashDirection()
        {
            if (_input == null) return Vector2.right;

            // 대시 방향 계산
            if (Mathf.Abs(_input.HorizontalInput) > 0.01f)
            {
                return _input.HorizontalInput > 0 ? Vector2.right : Vector2.left;
            }

            // 입력이 없는 경우: CharacterModel의 스케일 x 값 기준
            if (_character != null && _character.CharacterModel != null)
            {
                Transform modelTransform = _character.CharacterModel.transform;
                return modelTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
            }

            // 기본값: 오른쪽
            return Vector2.right;
        }

        protected override void ExecuteDash(Vector2 direction)
        {
            // 실제 물리 시스템에 대시 요청
            _physics?.RequestDash(direction);
        }
    }
}