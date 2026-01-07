using UnityEngine;

namespace TeamSuneat
{
    public class PlayerInput : MonoBehaviour
    {
        // 수평 이동 입력 (-1.0 ~ 1.0)
        public float HorizontalInput { get; private set; }

        // 수직 이동 입력 (-1.0 ~ 1.0)
        public float VerticalInput { get; private set; }

        // 점프 입력 (한 프레임만 true)
        public bool IsJumpPressed { get; private set; }

        // 점프 키 해제 (한 프레임만 true)
        public bool IsJumpReleased { get; private set; }

        // 아래 방향 키 입력 상태
        public bool IsDownInputPressed { get; private set; }

        // 대시 입력 (한 프레임만 true)
        public bool IsDashPressed { get; private set; }

        // 공격 입력 (한 프레임만 true)
        public bool IsAttackPressed { get; private set; }

        public void LogicUpdate()
        {
            if (TSInputManager.Instance == null || !TSInputManager.Instance.IsInitialized)
            {
                return;
            }

            // TSInputManager의 이동 입력 사용
            HorizontalInput = TSInputManager.Instance.PrimaryMovement.x;
            VerticalInput = TSInputManager.Instance.PrimaryMovement.y;

            // TSInputManager의 버튼 상태 사용
            IsJumpPressed = TSInputManager.Instance.CheckButtonState(ActionNames.Jump, ButtonStates.ButtonDown);
            IsJumpReleased = TSInputManager.Instance.CheckButtonState(ActionNames.Jump, ButtonStates.ButtonUp);
            IsDownInputPressed = TSInputManager.Instance.CheckButtonState(ActionNames.MoveDown, ButtonStates.ButtonPressed);
            IsDashPressed = TSInputManager.Instance.CheckButtonState(ActionNames.Dash, ButtonStates.ButtonDown);
            IsAttackPressed = TSInputManager.Instance.CheckButtonState(ActionNames.Attack, ButtonStates.ButtonDown);
        }
    }
}