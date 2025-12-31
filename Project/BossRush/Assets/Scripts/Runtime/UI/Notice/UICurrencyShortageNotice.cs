using TeamSuneat.Data;

namespace TeamSuneat.UserInterface
{
    // 재화 부족 알림 Notice 컴포넌트
    public class UICurrencyShortageNotice : UINoticeBase
    {
        public override void AutoNaming()
        {
            SetGameObjectName("UICurrencyShortageNotice");
        }

        public void SetContent(CurrencyNames currencyName)
        {
            string currencyNameString = currencyName.GetLocalizedString();
            string format = JsonDataManager.FindStringClone("Format_Currency_Shortage");

            if (!string.IsNullOrEmpty(format) && !string.IsNullOrEmpty(currencyNameString))
            {
                base.SetContent(string.Format(format, currencyNameString));
            }
            else
            {
                Log.Warning(LogTags.UI_Notice, "재화 부족 알림 표시 실패: format:[{0}], currencyNameString:[{1}]", format, currencyNameString);
            }
        }
    }
}