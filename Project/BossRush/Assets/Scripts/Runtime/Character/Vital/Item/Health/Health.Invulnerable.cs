using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Health : VitalResource
    {
        #region Invulnerable

        public void SetTemporarilyInvulnerable(Component source)
        {
            if (!TemporarilyInvulnerable.Contains(source))
            {
                TemporarilyInvulnerable.Add(source);
                LogInfo($"임시 무적 상태를 {"부여".ToSelectString()}합니다. {source.GetHierarchyName()}");
            }

            if (GameSetting.Instance.Play.ShowInvulnerableRenderer)
            {
                Vital.Owner.CharacterRenderer.ShowOutline();
            }
        }

        public void ResetTemporarilyInvulnerable(Component source)
        {
            if (TemporarilyInvulnerable.Remove(source))
            {
                LogInfo($"임시 무적 상태를 {"해제".ToDisableString()}합니다. {source.GetHierarchyName()}");
                if (TemporarilyInvulnerable.Count == 0)
                {
                    if (GameSetting.Instance.Play.ShowInvulnerableRenderer)
                    {
                        Vital.Owner.CharacterRenderer.HideOutline();
                    }
                }
            }
        }

        public void ClearTemporarilyInvulnerable()
        {
            LogInfo($"임시 무적 상태를 모두 {"해제".ToDisableString()}합니다: {TemporarilyInvulnerable.Count}");
            TemporarilyInvulnerable.Clear();
        }

        private void EnablePostDamageInvulnerability(float invincibilityDuration)
        {
            if (invincibilityDuration > 0)
            {
                PostDamageInvulnerable = true;
                CoroutineNextTimer(invincibilityDuration, DisablePostDamageInvulnerability);
            }
        }

        private void DisablePostDamageInvulnerability()
        {
            PostDamageInvulnerable = false;
        }

        #endregion Invulnerable
    }
}