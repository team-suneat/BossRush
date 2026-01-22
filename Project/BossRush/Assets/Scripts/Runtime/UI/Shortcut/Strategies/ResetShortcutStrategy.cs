using Rewired;

using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 단축키 초기화 전략
    /// </summary>
    public class ResetShortcutStrategy : BaseShortcutStrategy
    {
        public override void ApplyShortcuts(UIShortcutElement[] shortcutElements, ref int activateCount)
        {
            Log.Progress(LogTags.UI_Shortcut, "단축키 초기화 전략 적용 시작. 단축키 요소 수: {0}", shortcutElements.Length);
            
            // 모든 단축키를 비활성화하고 초기화
            for (int i = 0; i < shortcutElements.Length; i++)
            {
                shortcutElements[i].SetActive(false);                
                shortcutElements[i].Refresh(ActionNames.None);
            }
            
            activateCount = 0;
            Log.Progress(LogTags.UI_Shortcut, "단축키 초기화 전략 적용 완료. 모든 단축키 비활성화");
        }

        public override ShortcutData GetShortcutData(ControllerType controllerType)
        {
            return new ShortcutData(new ActionNames[0], new string[0]);
        }
    }
}
