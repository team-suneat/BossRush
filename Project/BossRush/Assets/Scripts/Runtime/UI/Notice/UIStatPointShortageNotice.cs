using TeamSuneat;

namespace TeamSuneat.UserInterface
{
    // 능력치 포인트 부족 알림 Notice 컴포넌트
    public class UIStatPointShortageNotice : UINoticeBase
    {
        public override void AutoNaming()
        {
            SetGameObjectName("UIStatPointShortageNotice");
        }

        public void SetStringKey()
        {
            base.SetStringKey("Notice_StatPoint_Shortage");
        }
    }
}