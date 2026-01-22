using Rewired;


namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 단축키 전략의 기본 구현 클래스
    /// </summary>
    public abstract class BaseShortcutStrategy : IShortcutStrategy
    {
        public virtual void ApplyShortcuts(UIShortcutElement[] shortcutElements, ref int activateCount)
        {
            ControllerType controllerType = TSInputManager.Instance.CurrentControllerType;
            ShortcutData shortcutData = GetShortcutData(controllerType);

            Log.Progress(LogTags.UI_Shortcut, "단축키 전략 적용 시작. 컨트롤러 타입: {0}, 액션 수: {1}", controllerType, shortcutData.Actions.Length);

            if (shortcutData.Actions.IsValid())
            {
                for (int i = 0; i < shortcutElements.Length; i++)
                {
                    var action = shortcutData.Actions.Length > i ? shortcutData.Actions[i] : ActionNames.None;
                    var textKeys = shortcutData.TextKeys.Length > i ? shortcutData.TextKeys[i] : string.Empty;

                    if (action != ActionNames.None)
                    {
                        SetShortcut(shortcutElements[i], action, textKeys);
                        activateCount++;
                        Log.Progress(LogTags.UI_Shortcut, "단축키 설정 완료. 인덱스: {0}, 액션: {1}, 텍스트: {2}", i, action, textKeys);
                    }
                    else
                    {
                        ResetShortcut(shortcutElements[i]);
                    }
                }

                // 위치 오프셋 적용
                if (shortcutData.PositionOffset.HasValue && shortcutData.Actions.Length > 1)
                {
                    shortcutElements[1].anchoredPosition3D += shortcutData.PositionOffset.Value;
                    Log.Progress(LogTags.UI_Shortcut, "단축키 위치 오프셋 적용. 오프셋: {0}", shortcutData.PositionOffset.Value);
                }
            }
        }

        public abstract ShortcutData GetShortcutData(ControllerType controllerType);

        /// <summary>
        /// 단일 단축키를 설정하고 활성화하는 헬퍼 메서드
        /// </summary>
        protected virtual void SetShortcut(UIShortcutElement shortcutElement, ActionNames actionName, string stringKey)
        {
            if (shortcutElement != null)
            {
                shortcutElement.SetActive(true);
                shortcutElement.Refresh(actionName, stringKey);
            }
        }

        protected void ResetShortcut(UIShortcutElement shortcutElement)
        {
            if (shortcutElement != null)
            {
                shortcutElement.SetActive(false);
                shortcutElement.Refresh(ActionNames.None);
            }
        }
    }
}