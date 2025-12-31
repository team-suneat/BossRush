using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // HUD 경험치 버튼 컴포넌트 - 버튼 클릭 시 경험치를 획득
    public class UIExperienceButton : UIButton
    {
        [Title("#UIExperienceButton")]
        [SerializeField] private int _earnAmount = 100;

        public override void AutoNaming()
        {
            SetGameObjectName($"HUDExperienceButton");
        }

        protected override void OnButtonClick()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Level == null)
            {
                Log.Warning(LogTags.UI, "프로필 또는 레벨 시스템을 찾을 수 없습니다.");
                return;
            }

            profile.Level.AddExperience(_earnAmount);
        }
    }
}

