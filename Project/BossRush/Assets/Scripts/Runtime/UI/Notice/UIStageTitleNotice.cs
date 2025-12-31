using TeamSuneat.Data;

namespace TeamSuneat.UserInterface
{
    // 스테이지 타이틀 Notice 컴포넌트
    public class UIStageTitleNotice : UINoticeBase
    {
        public void SetContent(StageNames stageName)
        {
            SetContent(stageName.GetLocalizedString());
        }
    }
}