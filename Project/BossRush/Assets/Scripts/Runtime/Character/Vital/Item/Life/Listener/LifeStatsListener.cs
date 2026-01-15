using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// Life의 데미지 이벤트를 구독하여 통계를 기록하는 Listener입니다.
    /// </summary>
    public sealed class LifeStatsListener : XBehaviour
    {
        private Life _life;

        private void Awake()
        {
            _life = GetComponentInParent<Life>();
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
            if (_life == null)
            {
                return;
            }

            _life.OnDamage += HandleDamageStats;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (_life == null)
            {
                return;
            }

            _life.OnDamage -= HandleDamageStats;
        }

        private void HandleDamageStats(DamageResult damage)
        {
            if (damage == null)
            {
                return;
            }

            if (damage.Attacker == null || !damage.Attacker.IsPlayer)
            {
                return;
            }

            GameSetting.Instance.Statistics.AddDamage(damage, Time.time);
        }
    }
}