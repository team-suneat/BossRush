using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIToggleButton : UISelectButtonBase
    {
        [FoldoutGroup("#Event")]
        public UnityEvent<bool> OnToggleChanged;

        protected override bool TryHandleClick()
        {
            if (!CheckClickable() || _currentState == ButtonState.Locked)
            {
                return false;
            }

            ToggleSelection();
            return true;
        }

        public void ToggleSelection()
        {
            if (_currentState == ButtonState.UnlockedSelected)
            {
                SetState(ButtonState.UnlockedUnselected);
                OnToggleChanged?.Invoke(false);
            }
            else if (_currentState == ButtonState.UnlockedUnselected)
            {
                SetState(ButtonState.UnlockedSelected);
                OnToggleChanged?.Invoke(true);
            }
        }
    }
}
