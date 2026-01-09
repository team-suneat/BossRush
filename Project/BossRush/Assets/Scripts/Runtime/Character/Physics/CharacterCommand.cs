using UnityEngine;

namespace TeamSuneat
{
    public struct CharacterCommand
    {
        // 이동 입력
        public float HorizontalInput;
        public float VerticalInput;
        public bool IsDownInputPressed;

        // 한 프레임만 true인 입력
        public bool IsJumpPressed;
        public bool IsJumpReleased;
        public bool IsDashPressed;
        public bool IsAttackPressed;
        public bool IsParryPressed;

        // 이전 프레임 상태 (한 프레임만 true 처리용)
        private bool _wasJumpPressed;
        private bool _wasDashPressed;
        private bool _wasAttackPressed;
        private bool _wasParryPressed;

        public void SetHorizontalInput(float value)
        {
            HorizontalInput = value;
        }

        public void SetVerticalInput(float value)
        {
            VerticalInput = value;
        }

        public void SetDownInputPressed(bool value)
        {
            IsDownInputPressed = value;
        }

        public void SetJumpPressed(bool value)
        {
            if (value && !_wasJumpPressed)
            {
                IsJumpPressed = true;
            }
            else if (!value)
            {
                IsJumpPressed = false;
            }
            _wasJumpPressed = value;
        }

        public void SetJumpReleased(bool value)
        {
            IsJumpReleased = value;
        }

        public void SetDashPressed(bool value)
        {
            if (value && !_wasDashPressed)
            {
                IsDashPressed = true;
            }
            else if (!value)
            {
                IsDashPressed = false;
            }
            _wasDashPressed = value;
        }

        public void SetAttackPressed(bool value)
        {
            if (value && !_wasAttackPressed)
            {
                IsAttackPressed = true;
            }
            else if (!value)
            {
                IsAttackPressed = false;
            }
            _wasAttackPressed = value;
        }

        public void SetParryPressed(bool value)
        {
            if (value && !_wasParryPressed)
            {
                IsParryPressed = true;
            }
            else if (!value)
            {
                IsParryPressed = false;
            }
            _wasParryPressed = value;
        }

        public void ResetFrame()
        {
            // 한 프레임만 true인 값들 초기화
            IsJumpPressed = false;
            IsJumpReleased = false;
            IsDashPressed = false;
            IsAttackPressed = false;
            IsParryPressed = false;

            // 이전 프레임 상태도 리셋 (다음 프레임에서 새로운 입력 감지 가능하도록)
            _wasJumpPressed = false;
            _wasDashPressed = false;
            _wasAttackPressed = false;
            _wasParryPressed = false;
        }

        public void Reset()
        {
            HorizontalInput = 0f;
            VerticalInput = 0f;
            IsDownInputPressed = false;
            IsJumpPressed = false;
            IsJumpReleased = false;
            IsDashPressed = false;
            IsAttackPressed = false;
            IsParryPressed = false;
            _wasJumpPressed = false;
            _wasDashPressed = false;
            _wasAttackPressed = false;
            _wasParryPressed = false;
        }
    }
}
