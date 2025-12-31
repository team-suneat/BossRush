using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDBossHealthGauge : MonoBehaviour
    {
        [SerializeField] private UIGauge _healthGauge;

        private Character _bossCharacter;
        private Vital _bossVital;
        private bool _isBossModeActive;
        private bool _isBossBound;

        private void Awake()
        {
            if (_healthGauge != null)
            {
                _healthGauge.SetActive(false);
            }
        }

        public void LogicUpdate()
        {
            if (!_isBossModeActive)
            {
                return;
            }

            _healthGauge?.LogicUpdate();
        }

        public void OnBossModeEntered()
        {
            if (_healthGauge == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 체력 게이지가 설정되지 않았습니다. {0}", this.GetHierarchyName());
                return;
            }

            _isBossModeActive = true;
            _healthGauge.SetActive(true);
            _healthGauge.ResetFrontValue();
            _healthGauge.ResetValueText();

            Log.Info(LogTags.UI_Gauge, "[BossHealthGauge] 보스 모드 진입. 체력 게이지 활성화. {0}", this.GetHierarchyName());
        }

        public void OnBossSpawned(Character bossCharacter)
        {
            if (_healthGauge == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 체력 게이지가 설정되지 않았습니다. {0}", this.GetHierarchyName());
                return;
            }

            if (!_isBossModeActive)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 보스 모드가 활성화되지 않은 상태에서 보스 스폰 이벤트를 받았습니다. {0}", this.GetHierarchyName());
                return;
            }

            if (bossCharacter == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 보스 캐릭터가 null입니다. {0}", this.GetHierarchyName());
                return;
            }

            Bind(bossCharacter);
        }

        public void OnBossDied()
        {
            Unbind();

            if (_healthGauge != null)
            {
                _healthGauge.SetActive(false);
                _healthGauge.ResetFrontValue();
                _healthGauge.ResetValueText();
            }

            _isBossModeActive = false;

            Log.Info(LogTags.UI_Gauge, "[BossHealthGauge] 보스 사망 또는 보스 모드 종료. 체력 게이지 비활성화. {0}", this.GetHierarchyName());
        }

        public void OnBossModeExited()
        {
            OnBossDied();
        }

        private void Bind(Character character)
        {
            Unbind();

            if (character == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 바인딩할 캐릭터가 null입니다. {0}", this.GetHierarchyName());
                return;
            }

            Vital vital = character.MyVital;
            if (vital == null)
            {
                Log.Warning(LogTags.UI_Gauge, "[BossHealthGauge] 보스 캐릭터의 Vital이 null입니다. Character: {0}, {1}", character.GetHierarchyName(), this.GetHierarchyName());
                return;
            }

            _bossCharacter = character;
            _bossVital = vital;

            if (_bossVital.DieEvent != null)
            {
                _bossVital.DieEvent.AddListener(OnBossDied);
            }

            if (_bossVital.Health != null)
            {
                _bossVital.Health.OnValueChanged += OnHealthChanged;
                SetHealth(_bossVital.Health);
            }

            _isBossBound = true;

            Log.Info(LogTags.UI_Gauge, "[BossHealthGauge] 보스 체력 게이지 바인딩 완료. Character: {0}, Vital: {1}, {2}",
                character.GetHierarchyName(), vital.GetHierarchyName(), this.GetHierarchyName());
        }

        private void Unbind()
        {
            if (_bossVital != null)
            {
                if (_bossVital.DieEvent != null)
                {
                    _bossVital.DieEvent.RemoveListener(OnBossDied);
                }

                if (_bossVital.Health != null)
                {
                    _bossVital.Health.OnValueChanged -= OnHealthChanged;
                }
            }

            _bossCharacter = null;
            _bossVital = null;
            _isBossBound = false;
        }

        private void SetHealth(VitalResource resource)
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

        private void OnHealthChanged(int current, int max)
        {
            SetHealth(_bossVital?.Health);
        }
    }
}