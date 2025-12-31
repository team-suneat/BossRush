using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat.UserInterface
{
    // 레벨업 버튼 - 경험치가 충분할 때 활성화되고 레벨업을 수행
    public class UICharacterLevelUpButton : UIButton
    {
        [FoldoutGroup("#UIButton-CharacterLevelUp"), SerializeField]
        public UnityEvent OnLevelUpSuccess;

        private VProfile GetProfile()
        {
            return GameApp.GetSelectedProfile();
        }

        public void Refresh()
        {
            VProfile profile = GetProfile();
            if (profile == null)
            {
                SetFrameImageColor(GameColors.IndianRed);
                return;
            }

            bool canLevelUp = CanLevelUp(profile);
            Color buttonColor = canLevelUp ? GameColors.SteelBlue : GameColors.IndianRed;
            SetFrameImageColor(buttonColor);
        }

        protected override void OnButtonClick()
        {
            base.OnButtonClick();
            TryPerformLevelUp();
        }

        protected override void OnButtonHold()
        {
            base.OnButtonHold();
            TryPerformLevelUp();
        }

        private void TryPerformLevelUp()
        {
            VProfile profile = GetProfile();
            if (profile == null)
            {
                return;
            }

            if (TryLevelUp(profile))
            {
                AddStatPoint(profile);
                Refresh();
                OnLevelUpSuccess?.Invoke();
            }
        }

        private bool CanLevelUp(VProfile profile)
        {
            if (profile == null)
            {
                return false;
            }

            return profile.Level.CanLevelUp();
        }

        private bool TryLevelUp(VProfile profile)
        {
            if (!profile.Level.CanLevelUpOrNotify())
            {
                Log.Warning(LogTags.UI_Page, "경험치가 부족하여 레벨업할 수 없습니다.");
                return false;
            }

            int addedLevel = profile.Level.LevelUp();
            if (addedLevel <= 0)
            {
                return false;
            }

            return true;
        }

        private void AddStatPoint(VProfile profile)
        {
            ExperienceConfigAsset experienceConfigAsset = ScriptableDataManager.Instance.GetExperienceConfigAsset();
            if (experienceConfigAsset == null)
            {
                return;
            }

            int statPointPerLevel = experienceConfigAsset.StatPointPerLevel;
            profile.Growth.AddStatPoint(statPointPerLevel);
        }
    }
}