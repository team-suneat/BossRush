using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// Life의 이벤트를 구독하여 UI(HP 바, 플로팅 텍스트)를 업데이트하는 Listener입니다.
    /// </summary>
    public sealed class LifeUIListener : XBehaviour
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

            _life.OnLifeValueChanged += HandleLifeChanged;
            _life.OnLifeHealRequested += HandleHealText;
            _life.OnDamage += HandleDamageText;
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            if (_life == null)
            {
                return;
            }

            _life.OnLifeValueChanged -= HandleLifeChanged;
            _life.OnLifeHealRequested -= HandleHealText;
            _life.OnDamage -= HandleDamageText;
        }

        private void HandleLifeChanged(int current, int max)
        {
            // HP 바 업데이트는 UIPlayerGauge가 이미 OnValueChanged를 구독 중이므로
            // 여기서는 추가 처리가 필요한 경우에만 구현
        }

        private void HandleHealText(int healValue)
        {
            if (_life == null || _life.DamageTextPoint == null)
            {
                return;
            }

            _life.SpawnFloatyText(healValue.ToString(), _life.DamageTextPoint, UIFloatyMoveNames.HealLife);
        }

        private void HandleDamageText(DamageResult damage)
        {
            if (damage == null)
            {
                return;
            }

            if (damage.TargetCharacter == null)
            {
                return;
            }

            if (_life == null || _life.DamageTextPoint == null)
            {
                return;
            }

            string content = string.Empty;
            UIFloatyMoveNames moveType = UIFloatyMoveNames.None;

            if (damage.DamageValue > 0)
            {
                moveType = UIFloatyText.ConvertToName(damage, _life.Type);
                content = damage.DamageValueToInt.ToString();
            }

            _life.SpawnFloatyText(content, _life.DamageTextPoint, moveType);
        }
    }
}