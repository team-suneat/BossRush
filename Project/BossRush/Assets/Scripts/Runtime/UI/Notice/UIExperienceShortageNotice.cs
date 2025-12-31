namespace TeamSuneat.UserInterface
{
    // 경험치 부족 알림 Notice 컴포넌트
    public class UIExperienceShortageNotice : UINoticeBase
    {
        public override void AutoNaming()
        {
            SetGameObjectName("UIExperienceShortageNotice");
        }

        public void SetStringKey()
        {
            base.SetStringKey("Notice_Experience_Shortage");
        }
    }
}