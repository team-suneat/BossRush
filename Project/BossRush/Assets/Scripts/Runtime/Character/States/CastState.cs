using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class CastState : ICharacterState
    {
        private CharacterStateMachine _stateMachine;
        private CharacterPhysics _physics;
        private readonly Character _character;
        private CharacterAnimator _animator;

        public CastState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
        {
            _stateMachine = stateMachine;
            _physics = physics;
            _animator = animator;
            _character = character;
        }

        public void OnEnter()
        {
            SkillName currentSkill = FindActiveInputCastSkillName();
            if (currentSkill != SkillName.None)
            {
                ActivateSkill(currentSkill);
            }
        }

        public void OnUpdate()
        {
            // 시전 애니메이션 중에는 입력 처리 없음
        }

        public void OnFixedUpdate()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
            }

            // 캐릭터가 살아있지 않으면 업데이트 스킵
            if (!_character.IsAlive)
            {
                return;
            }

            // 애니메이션 종료 시 자동으로 Idle/Walk로 전환
            if (!_animator.IsCasting)
            {
                CharacterCommand cmd = _character.Command;
                if (_physics.IsGrounded)
                {
                    // 착지 시
                    if (Mathf.Abs(cmd.HorizontalInput) > 0.01f)
                    {
                        _stateMachine.TransitionToState(CharacterState.Walk);
                    }
                    else
                    {
                        _stateMachine.TransitionToState(CharacterState.Idle);
                    }
                }
                else
                {
                    // 공중일 때: 속도에 따라 Jumping 또는 Falling로 전환
                    if (_physics.RigidbodyVelocity.y > 0f)
                    {
                        _stateMachine.TransitionToState(CharacterState.Jumping);
                    }
                    else
                    {
                        _stateMachine.TransitionToState(CharacterState.Falling);
                    }
                }
                return;
            }
        }

        public void OnExit()
        {
        }

        public void OnJumpRequested()
        {
            // 시전 중에는 점프 무시
        }

        public void OnDashRequested(Vector2 direction)
        {
            // 시전 중에는 대시 무시
        }

        public bool CanTransitionTo(CharacterState targetState)
        {
            // 시전에서 전환 가능한 상태
            return targetState == CharacterState.Idle ||
                   targetState == CharacterState.Walk ||
                   targetState == CharacterState.Jumping ||
                   targetState == CharacterState.Falling;
        }

        //

        private SkillName FindActiveInputCastSkillName()
        {
            if (_character is PlayerCharacter player)
            {
                var profile = GameApp.GetSelectedProfile();
                if (profile?.Charm == null)
                {
                    return SkillName.None;
                }

                // 캐시된 액티브+인풋 캐스트 트리거 스킬을 가진 부적 사용
                CharmName activeCharmName = profile.Charm.ActiveInputCastCharmName;
                if (activeCharmName != CharmName.None)
                {
                    CharmAssetData charmData = ScriptableDataManager.Instance?.FindCharmClone(activeCharmName);
                    if (charmData != null && charmData.SkillName != SkillName.None)
                    {
                        return charmData.SkillName;
                    }
                }
            }

            Log.Warning(LogTags.Skill, "액티브+인풋 캐스트 트리거 스킬을 가진 부적을 찾을 수 없습니다.");
            return SkillName.None;
        }

        private void ActivateSkill(SkillName skillName)
        {
            if (_character is PlayerCharacter player && player.Skill != null)
            {
                if (_animator.IsCasting)
                {
                    return;
                }

                if (player.Skill.TryActivate(skillName))
                {
                    player.Skill.Activate(skillName);
                }
            }
        }
    }
}