using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    public class UIPopupManager : XBehaviour
    {
        [SerializeField] private bool _isLockCancelPopup;
        public UIPopup CenterPopup { get; private set; }

        public bool IsLockPopup { get; set; }

        public bool BlockSpawnPopup { get; set; }

        public void LogicUpdate()
        {
        }

        public void ResetValues()
        {
            BlockSpawnPopup = false;
            IsLockPopup = false;
        }

        public void LockPopup()
        {
            IsLockPopup = true;
            Log.Info(LogTags.UI_Popup, "팝업을 잠가 더 이상 표시되지 않습니다.");
        }

        public void UnlockPopup(float waitTime = 0)
        {
            if (IsLockPopup)
            {
                if (waitTime > 0)
                {
                    Log.Info(LogTags.UI_Popup, "{0}초 후 팝업 잠금을 해제합니다.", waitTime);
                }

                _ = CoroutineNextRealTimer(waitTime, () =>
                {
                    IsLockPopup = false;
                    Log.Info(LogTags.UI_Popup, "팝업 잠금이 해제되었습니다.");
                });
            }
            else
            {
                Log.Progress(LogTags.UI_Popup, "팝업이 이미 닫혔습니다. 중복 해제는 허용되지 않습니다.");
            }
        }

        public void LockCancelPopup()
        {
            _isLockCancelPopup = true;
        }

        public void UnlockCancelPopup()
        {
            _isLockCancelPopup = false;
        }

        internal void CloseAllPopups()
        {
        }

        internal UIPopup SpawnCenterPopup(UIPopupNames popupName, UnityAction<bool> despawnCallback)
        {
            if (CenterPopup == null)
            {
                UIPopup popUp = ResourcesManager.SpawnPopup(popupName);
                if (popUp != null)
                {
                    CenterPopup = popUp;

                    CenterPopup.RegisterCloseCallback(despawnCallback);
                    CenterPopup.RegisterCloseCallback(OnCloseCenterPopup);
                    CenterPopup.Open();

                    Log.Info(LogTags.UI_Popup, "가운데 팝업을 생성합니다. {0}", CenterPopup.GetHierarchyName());
                }
            }
            else
            {
                Log.Warning(LogTags.UI_Popup, "가운데 팝업을 생성할 수 없습니다. 이미 생성되어있습니다. {0}", CenterPopup.GetHierarchyName());
            }

            return CenterPopup;
        }

        private void OnCloseCenterPopup(bool popupResult)
        {
            CenterPopup = null;
        }
    }
}