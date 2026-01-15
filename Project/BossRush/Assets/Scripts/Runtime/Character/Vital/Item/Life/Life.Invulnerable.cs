using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Life : VitalResource
    {
        /// <summary>
        /// 무적 상태 관리
        /// 
        /// CheckInvulnerable()에서 다음 조건 중 하나라도 true이면 무적 상태로 판단합니다:
        /// - TemporarilyInvulnerable: 임시 무적 리스트에 항목이 있는 경우
        /// - _invulnerable: 일반 무적 플래그
        /// - _immuneToDamage: 영구 피해 면역 플래그
        /// - PostDamageInvulnerable: 피해 후 무적 상태
        /// </summary>
        #region Invulnerable

        private Coroutine _postDamageInvulnerabilityCoroutine;

        public void SetTemporarilyInvulnerable(Component source)
        {
            if (source == null)
            {
                LogWarning("임시 무적 상태를 부여할 소스가 null입니다.");
                return;
            }

            if (!_temporarilyInvulnerable.Contains(source))
            {
                _temporarilyInvulnerable.Add(source);
                LogInfo($"임시 무적 상태를 {"부여".ToSelectString()}합니다. {source.GetHierarchyName()}");
            }

            if (GameSetting.Instance.Play.ShowInvulnerableRenderer)
            {
                if (Vital == null || Vital.Owner == null || Vital.Owner.CharacterRenderer == null)
                {
                    return;
                }

                Vital.Owner.CharacterRenderer.ShowOutline();
            }
        }

        public void ResetTemporarilyInvulnerable(Component source)
        {
            if (source == null)
            {
                LogWarning("임시 무적 상태를 해제할 소스가 null입니다.");
                return;
            }

            if (_temporarilyInvulnerable.Remove(source))
            {
                LogInfo($"임시 무적 상태를 {"해제".ToDisableString()}합니다. {source.GetHierarchyName()}");
                if (_temporarilyInvulnerable.Count == 0)
                {
                    if (GameSetting.Instance.Play.ShowInvulnerableRenderer)
                    {
                        if (Vital == null || Vital.Owner == null || Vital.Owner.CharacterRenderer == null)
                        {
                            return;
                        }

                        Vital.Owner.CharacterRenderer.HideOutline();
                    }
                }
            }
        }

        public void ClearTemporarilyInvulnerable()
        {
            LogInfo($"임시 무적 상태를 모두 {"해제".ToDisableString()}합니다: {_temporarilyInvulnerable.Count}");
            _temporarilyInvulnerable.Clear();
        }

        private void EnablePostDamageInvulnerability(float invincibilityDuration)
        {
            if (invincibilityDuration > 0)
            {
                StopXCoroutine(ref _postDamageInvulnerabilityCoroutine);

                PostDamageInvulnerable = true;
                _postDamageInvulnerabilityCoroutine = CoroutineNextTimer(invincibilityDuration, () =>
                {
                    DisablePostDamageInvulnerability();
                    _postDamageInvulnerabilityCoroutine = null;
                });
            }
        }

        private void DisablePostDamageInvulnerability()
        {
            PostDamageInvulnerable = false;
        }

        #endregion Invulnerable
    }
}