using UnityEngine;

namespace TeamSuneat
{
    public partial class PlayerCharacterAnimator
    {
        // 캐스트 상태 플래그 (플레이어 전용)
        private bool _isCasting;

        public override bool IsCasting => _isCasting;

        protected new void SetCasting(bool value)
        {
            if (_isCasting != value)
            {
                _isCasting = value;
                if (_animator != null)
                {
                    _animator.UpdateAnimatorBool(ANIMATOR_IS_CASTING_PARAMETER_ID, value, AnimatorParameters);
                }
            }
        }
    }
}
