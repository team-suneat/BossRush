using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIPlayerGauge : MonoBehaviour, ICharacterGaugeView
    {
        [SerializeField] private UIGaugePoolHandler _poolHandler;
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _shieldGauge;
        [SerializeField] private UIGauge _manaGauge;
        [SerializeField] private UIFollowObject _followObject;
        [SerializeField] private Vector3 _worldOffset;
        [SerializeField] private Vector3 _screenOffset;

        private Character _character;
        private Vital _vital;

        private void Awake()
        {
            if (_poolHandler != null)
            {
                _poolHandler.OnClearRequested += Clear;
            }
        }

        private void OnDestroy()
        {
            if (_poolHandler != null)
            {
                _poolHandler.OnClearRequested -= Clear;
            }
        }

        public void Despawn() => _poolHandler?.Despawn();
        public void SetDespawnMark() => _poolHandler?.SetDespawnMark();

        public void LogicUpdate()
        {
            _poolHandler?.LogicUpdate();
            _healthGauge?.LogicUpdate();
            _shieldGauge?.LogicUpdate();
            _manaGauge?.LogicUpdate();
        }

        public void Bind(Character character)
        {
            Unbind();

            if (character == null)
            {
                return;
            }

            Vital vital = character.MyVital;
            if (vital == null)
            {
                return;
            }

            _character = character;
            _vital = vital;

            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null)
            {
                if (!gaugeManager.RegisterCharacter(_vital, this))
                {
                    _character = null;
                    _vital = null;
                    return;
                }
            }

            if (_vital.DieEvent != null)
            {
                _vital.DieEvent.AddListener(OnVitalDied);
            }

            if (_vital.Health != null)
            {
                _vital.Health.OnValueChanged += OnHealthChanged;
                SetHealth(_vital.Health);
            }

            if (_vital.Shield != null)
            {
                _vital.Shield.OnValueChanged += OnShieldChanged;
            }
            SetShield(_vital.Shield);

            if (_vital.Mana != null)
            {
                _vital.Mana.OnValueChanged += OnResourceChanged;
            }
            SetResource(_vital.Mana);

            Transform anchor = _vital.GaugePoint != null ? _vital.GaugePoint : _character.transform;
            SetupFollow(anchor);
        }

        public void Unbind()
        {
            UIGaugeManager gaugeManager = UIManager.Instance != null ? UIManager.Instance.GaugeManager : null;
            if (gaugeManager != null && _vital != null)
            {
                _ = gaugeManager.UnregisterCharacter(_vital);
            }

            if (_vital != null && _vital.DieEvent != null)
            {
                _vital.DieEvent.RemoveListener(OnVitalDied);
            }

            if (_vital != null)
            {
                if (_vital.Health != null)
                {
                    _vital.Health.OnValueChanged -= OnHealthChanged;
                }

                if (_vital.Shield != null)
                {
                    _vital.Shield.OnValueChanged -= OnShieldChanged;
                }

                if (_vital.Mana != null)
                {
                    _vital.Mana.OnValueChanged -= OnResourceChanged;
                }
            }

            _character = null;
            _vital = null;

            _followObject?.StopFollowing();
        }

        public void SetHealth(VitalResource resource)
        {
            if (_healthGauge == null)
            {
                return;
            }

            if (resource == null)
            {
                _healthGauge.ResetValueText();
                _healthGauge.ResetFrontValue();
                return;
            }

            _healthGauge.SetValueText(resource.Current, resource.Max);
            _healthGauge.SetFrontValue(resource.Rate);
        }

        public void SetShield(VitalResource resource)
        {
            if (_shieldGauge == null)
            {
                return;
            }

            bool hasShield = resource != null && resource.Max > 0;
            _shieldGauge.gameObject.SetActive(hasShield);

            if (!hasShield)
            {
                _shieldGauge.ResetValueText();
                _shieldGauge.ResetFrontValue();
                return;
            }

            _shieldGauge.SetValueText(resource.Current, resource.Max);
            _shieldGauge.SetFrontValue(resource.Rate);
        }

        public void SetResource(VitalResource resource)
        {
            if (_manaGauge == null)
            {
                return;
            }

            bool hasMana = resource != null && resource.Max > 0;
            _manaGauge.gameObject.SetActive(hasMana);

            if (!hasMana)
            {
                _manaGauge.ResetValueText();
                _manaGauge.ResetFrontValue();
                return;
            }

            _manaGauge.SetValueText(resource.Current, resource.Max);
            _manaGauge.SetFrontValue(resource.Rate);
        }

        public void Clear()
        {
            _healthGauge?.ResetValueText();
            _healthGauge?.ResetFrontValue();

            _shieldGauge?.ResetValueText();
            _shieldGauge?.ResetFrontValue();

            _manaGauge?.ResetValueText();
            _manaGauge?.ResetFrontValue();

            Unbind();
        }

        private void SetupFollow(Transform anchor)
        {
            if (_followObject == null)
            {
                return;
            }

            _followObject.IsWorldSpaceCanvas = true;
            _followObject.SetWorldOffset(_worldOffset);
            _followObject.ScreenOffset = _screenOffset;
            _followObject.Setup(anchor);
        }

        private void OnHealthChanged(int current, int max)
        {
            SetHealth(_vital?.Health);
        }

        private void OnShieldChanged(int current, int max)
        {
            SetShield(_vital?.Shield);
        }

        private void OnResourceChanged(int current, int max)
        {
            SetResource(_vital?.Mana);
        }

        private void OnVitalDied()
        {
            Clear();
            Despawn();
        }
    }
}