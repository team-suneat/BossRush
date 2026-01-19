using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIImmediateButton : UISelectButtonBase
    {
        [FoldoutGroup("#Event")]
        public UnityEvent OnImmediateClick;

        protected override bool TryHandleClick()
        {
            if (!CheckClickable() || _currentState == ButtonState.Locked)
            {
                return false;
            }

            OnImmediateClick?.Invoke();
            return true;
        }
    }
}
