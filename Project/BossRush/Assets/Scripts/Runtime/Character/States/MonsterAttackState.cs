using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackState : BaseAttackState
    {
        // 공격 순서 관리
        private List<int> _attackOrder = new List<int>();
        private int _currentAttackIndex = 0;
        private bool _hasAttackOrder = false;

        public MonsterAttackState(CharacterStateMachine stateMachine, CharacterPhysics physics, CharacterAnimator animator, Character character)
            : base(stateMachine, physics, animator, character)
        {
        }

        public void SetAttackOrder(List<int> attackOrder)
        {
            if (attackOrder == null || attackOrder.Count == 0)
            {
                Log.Warning(LogTags.CharacterState, "공격 순서가 비어있습니다. 기본 공격(0)을 사용합니다.");
                _attackOrder = new List<int> { 0 };
            }
            else
            {
                _attackOrder = new List<int>(attackOrder);
            }

            _hasAttackOrder = true;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _currentAttackIndex = 0;

            // 공격 순서가 설정되지 않았을 경우 기본 공격 사용
            if (!_hasAttackOrder || _attackOrder.Count == 0)
            {
                Log.Warning(LogTags.CharacterState, "공격 순서가 설정되지 않았습니다. 기본 공격(0)을 사용합니다.");
                _attackOrder = new List<int> { 0 };
                _hasAttackOrder = true;
            }

            // 첫 번째 공격 재생
            int firstAttackIndex = _attackOrder[0];
            PlayAttackAnimation(firstAttackIndex);
        }

        public override void OnUpdate()
        {
            if (_physics == null || _animator == null || _character == null)
            {
                return;
            }

            // 현재 공격이 완료되었는지 확인
            if (!_animator.IsAttacking)
            {
                // 현재 공격의 쿨타임 시작
                if (_currentAttackIndex >= 0 && _currentAttackIndex < _attackOrder.Count)
                {
                    int currentAttackOrder = _attackOrder[_currentAttackIndex];
                    if (_character.Attack != null)
                    {
                        _character.Attack.StartAttackCooldown(currentAttackOrder);
                    }
                }

                // 다음 공격 순서로 진행
                _currentAttackIndex++;

                if (_currentAttackIndex < _attackOrder.Count)
                {
                    // 다음 공격 재생
                    int nextAttackIndex = _attackOrder[_currentAttackIndex];
                    PlayAttackAnimation(nextAttackIndex);
                }
                else
                {
                    // 모든 공격 순서 완료 - 다음 상태로 전환
                    TransitionToNextState();
                }
            }
        }

        public override void OnFixedUpdate()
        {
        }

        public override void OnExit()
        {
            base.OnExit();

            _currentAttackIndex = 0;
            // 공격 순서는 유지 (다음 공격 시 재사용 가능)
        }

        protected override Character.FacingDirections? GetAttackDirection()
        {
            // 몬스터: 타겟 방향을 바라봄
            if (_character != null && _character.TargetCharacter != null)
            {
                Vector3 targetPosition = _character.TargetCharacter.transform.position;
                Vector3 myPosition = _character.transform.position;

                if (targetPosition.x > myPosition.x)
                {
                    return Character.FacingDirections.Right;
                }
                else
                {
                    return Character.FacingDirections.Left;
                }
            }

            return null;
        }
    }
}