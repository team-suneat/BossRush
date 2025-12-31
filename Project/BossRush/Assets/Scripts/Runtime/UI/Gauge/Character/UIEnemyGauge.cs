using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIEnemyGauge : MonoBehaviour, ICharacterGaugeView
    {
        [SerializeField] private UIGaugePoolHandler _poolHandler;
        [SerializeField] private UIGauge _healthGauge;
        [SerializeField] private UIGauge _cooldownGauge;
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
            _cooldownGauge?.LogicUpdate();
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

            _vital.DieEvent?.AddListener(OnVitalDied);

            if (_vital.Health != null)
            {
                _vital.Health.OnValueChanged += OnHealthChanged;
                SetHealth(_vital.Health);
            }

            HideCooldown();

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

        // 쿨타임 게이지를 활성화하고 0으로 초기화합니다.
        public void ShowCooldown()
        {
            if (_cooldownGauge == null)
            {
                return;
            }

            _cooldownGauge.gameObject.SetActive(true);
            _cooldownGauge.ResetFrontValue();
        }

        // 쿨타임 게이지를 비활성화합니다.
        public void HideCooldown()
        {
            if (_cooldownGauge == null)
            {
                return;
            }

            _cooldownGauge.gameObject.SetActive(false);
            _cooldownGauge.ResetFrontValue();
        }

        // 쿨타임 게이지의 진행도를 설정합니다. (0~1)
        public void SetCooldown(float rate)
        {
            if (_cooldownGauge == null)
            {
                return;
            }

            _cooldownGauge.SetFrontValue(rate);
        }

        public void Clear()
        {
            _healthGauge?.ResetValueText();
            _healthGauge?.ResetFrontValue();

            HideCooldown();

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

        private void OnVitalDied()
        {
            Clear();
            Despawn();
        }
    }
}