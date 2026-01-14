using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator
    {
        // 상태 플래그를 구조체로 그룹화
        private struct AnimationStateFlags
        {
            public bool IsAttacking;
            public bool IsDashing;
            public bool IsDamaging;
            public bool IsParrying;
            public bool IsBlockDeathAnimation;

            public void Reset()
            {
                IsAttacking = false;
                IsDashing = false;
                IsDamaging = false;
                IsParrying = false;
                IsBlockDeathAnimation = false;
            }
        }

        private AnimationStateFlags _stateFlags;

        // 읽기 전용 프로퍼티로 노출
        public bool IsAttacking => _stateFlags.IsAttacking;
        public bool IsDashing => _stateFlags.IsDashing;
        public bool IsDamaging => _stateFlags.IsDamaging;
        public bool IsParrying => _stateFlags.IsParrying;
        public bool IsBlockDeathAnimation => _stateFlags.IsBlockDeathAnimation;

        // 내부 상태 변경 메서드 (필요 시 부가 로직 추가 가능)
        protected void SetAttacking(bool value)
        {
            if (_stateFlags.IsAttacking != value)
            {
                _stateFlags.IsAttacking = value;
                if (_animator != null)
                {
                    _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, value, AnimatorParameters);
                }
            }
        }

        protected void SetDashing(bool value)
        {
            if (_stateFlags.IsDashing != value)
            {
                _stateFlags.IsDashing = value;
            }
        }

        protected void SetDamaging(bool value)
        {
            if (_stateFlags.IsDamaging != value)
            {
                _stateFlags.IsDamaging = value;
                if (_animator != null)
                {
                    _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, value, AnimatorParameters);
                }
            }
        }

        protected void SetParrying(bool value)
        {
            if (_stateFlags.IsParrying != value)
            {
                _stateFlags.IsParrying = value;
                if (_animator != null)
                {
                    _animator.UpdateAnimatorBool(ANIMATOR_IS_PARRYING_PARAMETER_ID, value, AnimatorParameters);
                }
            }
        }

        protected void SetBlockDeathAnimation(bool value)
        {
            _stateFlags.IsBlockDeathAnimation = value;
        }
    }
}