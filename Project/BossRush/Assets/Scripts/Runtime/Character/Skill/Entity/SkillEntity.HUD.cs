using Lean.Pool;
using TeamSuneat.Data;
using TeamSuneat.UserInterface;


namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        public void SetHUD()
        {
            if (CharacterInfo == null)
            {
                return;
            }
            if (Owner == null || !Owner.IsPlayer)
            {
                return;
            }
            if (HudCommandItem != null)
            {
                LogWarning("이미 연결된 스킬 HUD가 있습니다.");
                return;
            }

            VSkill _skillInfo = CharacterInfo.Skill.Find(Name);
            if (_skillInfo == null)
            {
                LogWarning("해당 캐릭터의 스킬 정보를 찾을 수 없습니다.");
                return;
            }

            if (_skillInfo.ActionName == ActionNames.None)
            {
                LogProgress("스킬 HUD를 설정할 수 없습니다. 할당된 액션이 없습니다.");
                return;
            }

            if (UIManager.Instance == null)
            {
                return;
            }

            HudCommandItem = UIManager.Instance.HUDPlayerCharacter.FindCommandItem(_skillInfo.ActionName);
            if (HudCommandItem != null)
            {
                HudCommandItem.Initialize();
            }

            LogProgress("스킬 HUD를 설정합니다.");
        }

        private void ResetHUD()
        {
            if (HudCommandItem == null) return;

            HudCommandItem.Deactivate();
            HudCommandItem = null;
        }

        private void StartCooldownOfHUD()
        {
            if (HudCommandItem == null) return;

            HudCommandItem.SetCooldowning(true);
        }

        private void StopCooldownOfHUD()
        {
            if (HudCommandItem == null) return;

            HudCommandItem.SetCooldowning(false);
        }

        public void SetCooldownOfHUD()
        {
            if (HudCommandItem == null) return;

            float cooldownRate = ElapsedTimeOnCast.SafeDivide(CooldownTime);
            SetCooldownOfHUD(cooldownRate, CooldownTime - ElapsedTimeOnCast);
        }

        private void SetCooldownOfHUD(float cooldownRate, float lastTime)
        {
            if (HudCommandItem == null) return;

            HudCommandItem.SetCooldownFillAmount(cooldownRate);
            HudCommandItem.SetCooldownText(lastTime);
        }

        private void ResetCooldownOfHUD()
        {
            if (HudCommandItem == null) return;

            HudCommandItem.SetCooldownFillAmount(1);
            HudCommandItem.ResetCooldownText();
        }

        private void RefreshAllowOfHUD()
        {
            if (HudCommandItem == null) return;

            HudCommandItem.RefreshAllowState();
        }

        private void SetCountOfHUD()
        {
            if (HudCommandItem == null) return;

            if (_skillMaxCount > 0)
            {
                HudCommandItem.SetSkillCount(_skillCount, _skillMaxCount);
                HudCommandItem.RefreshAllowState();
            }
        }
    }
}