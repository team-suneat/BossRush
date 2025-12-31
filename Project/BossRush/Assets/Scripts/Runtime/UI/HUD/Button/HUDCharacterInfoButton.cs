namespace TeamSuneat.UserInterface
{
    // HUD 캐릭터 정보 버튼 - 클릭 시 캐릭터 정보 팝업 열기
    public class HUDCharacterInfoButton : UIButton
    {
        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            OpenCharacterInfoPopup();
        }

        private void OpenCharacterInfoPopup()
        {
            if (CharacterManager.Instance == null || CharacterManager.Instance.Player == null)
            {
                Log.Warning(LogTags.UI_Popup, "CharacterManager가 없어 캐릭터 정보 팝업을 열 수 없습니다.");
                return;
            }

            if (UIManager.Instance == null)
            {
                Log.Warning(LogTags.UI_Popup, "UIManager가 없어 캐릭터 정보 팝업을 열 수 없습니다.");
                return;
            }

            if (UIManager.Instance.PopupManager.BlockSpawnPopup)
            {
                Log.Info(LogTags.UI_Popup, "팝업 스폰이 차단되어 캐릭터 정보 팝업을 열 수 없습니다.");
                return;
            }

            if (UIManager.Instance.PopupManager.CenterPopup != null)
            {
                Log.Info(LogTags.UI_Popup, "이미 열린 팝업이 있어 캐릭터 정보 팝업을 열 수 없습니다.");
                return;
            }

            UIPopup popup = UIManager.Instance.PopupManager.SpawnCenterPopup(UIPopupNames.CharacterInfo, null);
            if (popup != null)
            {
                Log.Info(LogTags.UI_Popup, "캐릭터 정보 팝업을 엽니다.");
            }
            else
            {
                Log.Error(LogTags.UI_Popup, "캐릭터 정보 팝업 프리팹을 찾을 수 없습니다.");
            }
        }
    }
}