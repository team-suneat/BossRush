using UnityEngine;

namespace TeamSuneat
{
    public class StunnedState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private Character _character;
        private float _stunDuration;
        private float _stunTimer;
        private VFXObject _stunVFX;

        public StunnedState(CharacterStateMachine stateMachine, Character character)
        {
            _stateMachine = stateMachine;
            _character = character;
        }

        public void OnEnter()
        {
            // 스턴 애니메이션 재생
            _ = (_character?.CharacterAnimator?.PlayStunAnimation());

            // 스턴 VFX 생성
            if (_character != null)
            {
                _stunVFX = VFXManager.Spawn("fx_character_stun", _character.HeadPoint, true);

                // 스턴 FloatyText 생성
                ResourcesManager.SpawnFloatyText("기절!", UIFloatyMoveNames.Content, _character.HeadPoint);
            }
        }

        public void OnUpdate()
        {
            // 기절 시간 감소
            _stunTimer -= Time.deltaTime;

            // 기절 시간이 끝나면 Idle로 전환
            if (_stunTimer <= 0f)
            {
                _stateMachine?.TransitionToState(CharacterState.Idle);
            }
        }

        public void OnFixedUpdate()
        {
            // Stunned 상태에서는 물리 속도 제어
            if (_character?.Physics != null)
            {
                _character.Physics.ResetVelocity();
            }
        }

        public void OnExit()
        {
            // Stunned 상태 종료 시 처리
            _stunTimer = 0f;
            _stunDuration = 0f;

            // 스턴 애니메이션 상태 해제
            _character?.CharacterAnimator?.SetStunned(false);

            // 스턴 VFX 제거
            if (_stunVFX != null)
            {
                _stunVFX.ForceDespawn();
                _stunVFX = null;
            }
        }

        // 기절 시간 설정
        public void SetStunDuration(float duration)
        {
            _stunDuration = duration;
            _stunTimer = duration;
        }

        public void OnJumpRequested()
        {
            // Stunned 상태에서는 이동 불가
        }

        public void OnDashRequested(Vector2 direction)
        {
            // Stunned 상태에서는 이동 불가
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // Stunned에서 전환 가능한 상태
            return targetState is CharacterState.Idle or
                   CharacterState.Dead;
        }
    }
}