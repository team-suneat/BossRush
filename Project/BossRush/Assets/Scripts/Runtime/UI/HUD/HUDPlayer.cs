using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary> 플레이어 캐릭터의 허드 UI를 관리하는 메인 클래스입니다. </summary>
    public class HUDPlayer : XBehaviour
    {
        [FoldoutGroup("#HUDPlayer")]
        [SerializeField] private Image _characterPortrait;

        [FoldoutGroup("#HUDPlayer")]
        [SerializeField] private UIHealthHearts _healthHearts;

        [FoldoutGroup("#HUDPlayer")]
        [SerializeField] private UIManaGauge _manaGauge;

        [FoldoutGroup("#HUDPlayer")]
        [SerializeField] private UIPulseBars _pulseBars;

        private PlayerCharacter _playerCharacter;
        private Vital _vital;
        private Mana _mana;
        private Pulse _pulse;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _characterPortrait ??= this.FindComponent<UnityEngine.UI.Image>("CharacterPortrait");
            _healthHearts ??= GetComponentInChildren<UIHealthHearts>();
            _manaGauge ??= GetComponentInChildren<UIManaGauge>();
            _pulseBars ??= GetComponentInChildren<UIPulseBars>();
        }

        /// <summary> 플레이어 캐릭터를 바인딩합니다. </summary>
        public void Bind(PlayerCharacter playerCharacter)
        {
            Unbind();

            if (playerCharacter == null)
            {
                return;
            }

            Vital vital = playerCharacter.MyVital;
            if (vital == null)
            {
                return;
            }

            _playerCharacter = playerCharacter;
            _vital = vital;
            _mana = vital.Mana;
            _pulse = vital.Pulse;

            SetupHealth();
            SetupMana();
            SetupPulse();
        }

        /// <summary> 바인딩을 해제합니다. </summary>
        public void Unbind()
        {
            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged -= OnLifeChanged;
                }

                if (_pulse != null)
                {
                    _pulse.OnGaugeProgressChanged -= OnPulseGaugeProgressChanged;
                    _pulse.OnValueChanged -= OnPulseValueChanged;
                }

                if (_mana != null)
                {
                    _mana.OnGaugeProgressChanged -= OnManaGaugeProgressChanged;
                    _mana.OnValueChanged -= OnManaValueChanged;
                }
            }

            _playerCharacter = null;
            _vital = null;
            _mana = null;
            _pulse = null;
        }

        /// <summary> 체력 UI를 설정합니다. </summary>
        private void SetupHealth()
        {
            if (_vital.Life != null)
            {
                _vital.Life.OnValueChanged += OnLifeChanged;
                OnLifeChanged(_vital.Life.Current, _vital.Life.Max);
            }
        }

        /// <summary> 마나 UI를 설정합니다. </summary>
        private void SetupMana()
        {
            if (_mana != null)
            {
                _mana.OnGaugeProgressChanged += OnManaGaugeProgressChanged;
                _mana.OnValueChanged += OnManaValueChanged;

                OnManaGaugeProgressChanged(_mana.GaugeProgress);
                OnManaValueChanged(_mana.Current, _mana.Max);
            }
        }

        /// <summary> 펄스 UI를 설정합니다. </summary>
        private void SetupPulse()
        {
            if (_pulse != null)
            {
                _pulse.OnGaugeProgressChanged += OnPulseGaugeProgressChanged;
                _pulse.OnValueChanged += OnPulseValueChanged;

                OnPulseGaugeProgressChanged(_pulse.GaugeProgress);
                OnPulseValueChanged(_pulse.Current, _pulse.Max);
            }
        }

        private void OnLifeChanged(int current, int max)
        {
            _healthHearts?.SetHealth(current, max);
        }

        private void OnManaGaugeProgressChanged(float progress)
        {
            _manaGauge?.SetGaugeProgress(progress);
        }

        private void OnManaValueChanged(int current, int max)
        {
            if (_mana != null)
            {
                _manaGauge?.SetFullManaCount(current, max);
            }
        }

        private void OnPulseGaugeProgressChanged(float progress)
        {
            _pulseBars?.SetGaugeProgress(progress);
        }

        private void OnPulseValueChanged(int current, int max)
        {
            if (_pulse != null)
            {
                _pulseBars?.SetFullPulseCount(current, max);
            }
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}