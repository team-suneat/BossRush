using UnityEngine;

namespace TeamSuneat
{
    public class PlayerAttackState : BaseAttackState
    {
        // 콤보 관련 필드
        private int _currentComboIndex = 0;
        private int _maxComboCount = 3;
        private bool _canQueueCombo = false;
        private bool _hasQueuedCombo = false;
        private bool _isAirAttack = false;

        public PlayerAttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
            : base(stateMachine, physics, animator, character)
        {
        }

        public void SetMaxComboCount(int maxComboCount)
        {
            _maxComboCount = Mathf.Max(1, maxComboCount);
        }

        public void EnableComboQueue()
        {
            _canQueueCombo = true;
        }

        public void DisableComboQueue()
        {
            _canQueueCombo = false;

            if (_hasQueuedCombo)
            {
                _hasQueuedCombo = false;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;

            // 공중/지상 상태 확인
            if (_physics != null && !_physics.IsGrounded)
            {
                // 공중 공격: 단일 콤보만 지원
                _isAirAttack = true;
                _maxComboCount = 1;
                PlayAirAttackAnimation();
            }
            else
            {
                // 지상 공격: 기존 콤보 시스템 사용
                _isAirAttack = false;
                _maxComboCount = 3;  // 기본값으로 초기화
                PlayAttackAnimation(_currentComboIndex);

                // 이동 잠금
                _animator?.LockMovement();
            }
        }

        public override void OnUpdate()
        {
            // 캐릭터가 살아있지 않으면 업데이트 스킵
            if (!_character.IsAlive)
            {
                return;
            }

            // 공중 공격 중에는 콤보 입력 무시 (단일 콤보만 지원)
            if (_isAirAttack)
            {
                return;
            }

            if (_character != null)
            {
                var cmd = _character.Command;
                if (cmd.IsAttackPressed)
                {
                    if (_canQueueCombo)
                    {
                        if (_currentComboIndex < _maxComboCount - 1)
                        {
                            _hasQueuedCombo = true;
                        }
                    }
                }
            }
        }

        public override void OnFixedUpdate()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
            }

            if (_hasQueuedCombo)
            {
                _currentComboIndex++;
                _hasQueuedCombo = false;
                _canQueueCombo = false;
                PlayAttackAnimation(_currentComboIndex);
            }

            TransitionToNextState();
        }

        public override void OnExit()
        {
            base.OnExit();

            _currentComboIndex = 0;
            _canQueueCombo = false;
            _hasQueuedCombo = false;
            _isAirAttack = false;
        }

        public override void OnJumpRequested()
        {
            // 공중 공격은 캔슬 불가
            if (_isAirAttack)
            {
                return;
            }

            // 지상 공격이면 바로 점프로 전환
            if (_physics != null && _physics.IsGrounded)
            {
                // 공격 애니메이션 중지
                _animator?.StopAttacking();

                // 점프 상태로 전환
                _stateMachine.ChangeState(CharacterState.Jumping);
            }
        }

        protected override Character.FacingDirections? GetAttackDirection()
        {
            // 플레이어: 입력 방향에 맞게 캐릭터 반전
            if (_character != null)
            {
                var cmd = _character.Command;
                if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
                {
                    return cmd.HorizontalInput > 0
                        ? Character.FacingDirections.Right
                        : Character.FacingDirections.Left;
                }
            }

            return null;
        }
    }
}
