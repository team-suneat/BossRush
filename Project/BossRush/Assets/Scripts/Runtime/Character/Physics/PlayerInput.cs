using UnityEngine;

namespace TeamSuneat
{
    public class PlayerInput : MonoBehaviour
    {
        private PlayerCharacter _character;

        // 수평 이동 입력 (-1.0 ~ 1.0)
        public float HorizontalInput => _character != null ? _character.Command.HorizontalInput : 0f;

        // 수직 이동 입력 (-1.0 ~ 1.0)
        public float VerticalInput => _character != null ? _character.Command.VerticalInput : 0f;

        // 점프 입력 (한 프레임만 true)
        public bool IsJumpPressed => _character != null ? _character.Command.IsJumpPressed : false;

        // 점프 키 해제 (한 프레임만 true)
        public bool IsJumpReleased => _character != null ? _character.Command.IsJumpReleased : false;

        // 아래 방향 키 입력 상태
        public bool IsDownInputPressed => _character != null ? _character.Command.IsDownInputPressed : false;

        // 대시 입력 (한 프레임만 true)
        public bool IsDashPressed => _character != null ? _character.Command.IsDashPressed : false;

        // 공격 입력 (한 프레임만 true)
        public bool IsAttackPressed => _character != null ? _character.Command.IsAttackPressed : false;

        // 패리 입력 (한 프레임만 true)
        public bool IsParryPressed => _character != null ? _character.Command.IsParryPressed : false;

        private void Awake()
        {
            _character = GetComponent<PlayerCharacter>();
        }

        public void LogicUpdate()
        {
            if (_character == null || TSInputManager.Instance == null || !TSInputManager.Instance.IsInitialized)
            {
                return;
            }

            // TSInputManager 값 읽고 Character 메서드로 전달
            float h = TSInputManager.Instance.PrimaryMovement.x;
            float v = TSInputManager.Instance.PrimaryMovement.y;
            _character.SetHorizontalInput(h);
            _character.SetVerticalInput(v);
            _character.SetDownInputPressed(TSInputManager.Instance.CheckButtonState(ActionNames.MoveDown, ButtonStates.ButtonPressed));

            if (TSInputManager.Instance.CheckButtonState(ActionNames.Jump, ButtonStates.ButtonDown))
            {
                _character.RequestJump(true);
            }
            if (TSInputManager.Instance.CheckButtonState(ActionNames.Jump, ButtonStates.ButtonUp))
            {
                _character.RequestJumpRelease(true);
            }
            if (TSInputManager.Instance.CheckButtonState(ActionNames.Dash, ButtonStates.ButtonDown))
            {
                _character.RequestDash();
            }
            if (TSInputManager.Instance.CheckButtonState(ActionNames.Attack, ButtonStates.ButtonDown))
            {
                _character.RequestAttack();
            }
            if (TSInputManager.Instance.CheckButtonState(ActionNames.Parry, ButtonStates.ButtonDown))
            {
                _character.RequestParry();
            }
        }
    }
}