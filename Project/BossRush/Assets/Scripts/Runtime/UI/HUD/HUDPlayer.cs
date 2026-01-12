using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
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

        public override void AutoGetComponents() {
            base.AutoGetComponents();

            _characterPortrait ??= this.FindComponent<UnityEngine.UI.Image>("CharacterPortrait");
            _healthHearts ??= GetComponentInChildren<UIHealthHearts>();
            _manaGauge ??= GetComponentInChildren<UIManaGauge>();
            _pulseBars ??= GetComponentInChildren<UIPulseBars>();
        }

        public void Bind(PlayerCharacter playerCharacter) {
            Unbind();

            if (playerCharacter == null) {
                return;
            }

            Vital vital = playerCharacter.MyVital;
            if (vital == null) {
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

        public void Unbind() {
            if (_vital != null) {
                if (_vital.Life != null) {
                    _vital.Life.OnValueChanged -= OnLifeChanged;
                }

                if (_pulse != null) {
                    _pulse.OnGaugeProgressChanged -= OnPulseGaugeProgressChanged;
                    _pulse.OnValueChanged -= OnPulseValueChanged;
                    _pulse.OnBurnoutStateChanged -= OnPulseBurnoutStateChanged;
                }

                if (_mana != null) {
                    _mana.OnGaugeProgressChanged -= OnManaGaugeProgressChanged;
                    _mana.OnValueChanged -= OnManaValueChanged;
                }
            }

            _playerCharacter = null;
            _vital = null;
            _mana = null;
            _pulse = null;
        }

        private void SetupHealth() {
            if (_vital.Life != null) {
                _vital.Life.OnValueChanged += OnLifeChanged;
                OnLifeChanged(_vital.Life.Current, _vital.Life.Max);
            }
        }

        private void SetupMana() {
            if (_mana != null) {
                _mana.OnGaugeProgressChanged += OnManaGaugeProgressChanged;
                _mana.OnValueChanged += OnManaValueChanged;

                OnManaGaugeProgressChanged(_mana.GaugeProgress);
                OnManaValueChanged(_mana.Current, _mana.Max);
            }
        }

        private void SetupPulse() {
            if (_pulse != null) {
                _pulse.OnGaugeProgressChanged += OnPulseGaugeProgressChanged;
                _pulse.OnValueChanged += OnPulseValueChanged;
                _pulse.OnBurnoutStateChanged += OnPulseBurnoutStateChanged;

                OnPulseGaugeProgressChanged(_pulse.GaugeProgress);
                OnPulseValueChanged(_pulse.Current, _pulse.Max);
                OnPulseBurnoutStateChanged(_pulse.IsBurnout);
            }
        }

        private void OnLifeChanged(int current, int max) {
            _healthHearts?.SetHealth(current, max);
        }

        private void OnManaGaugeProgressChanged(float progress) {
            _manaGauge?.SetGaugeProgress(progress);
        }

        private void OnManaValueChanged(int current, int max) {
            if (_mana != null) {
                _manaGauge?.SetFullManaCount(current, max);
            }
        }

        private void OnPulseGaugeProgressChanged(float progress) {
            _pulseBars?.SetGaugeProgress(progress);
        }

        private void OnPulseValueChanged(int current, int max) {
            if (_pulse != null) {
                _pulseBars?.SetFullPulseCount(current, max);
            }
        }

        private void OnPulseBurnoutStateChanged(bool isBurnout) {
            _pulseBars?.SetBurnoutState(isBurnout);
        }

        private void OnDestroy() {
            Unbind();
        }
    }
}