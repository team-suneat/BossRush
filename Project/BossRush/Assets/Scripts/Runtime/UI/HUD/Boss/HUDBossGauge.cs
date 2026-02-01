using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDBossGauge : XBehaviour
    {
        [FoldoutGroup("#HUDBossGauge")]
        [SerializeField] private UIGauge _lifeGauge;

        [FoldoutGroup("#HUDBossGauge")]
        [SerializeField] private UIGauge _poiseGauge;

        private BossCharacter _bossCharacter;
        private Vital _vital;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _lifeGauge = this.FindComponent<UIGauge>("UIGauge(Life)");
            _poiseGauge = this.FindComponent<UIGauge>("UIGauge(Poise)");
        }

        private void Update()
        {
            _lifeGauge?.LogicUpdate();
            _poiseGauge?.LogicUpdate();
        }

        public void Bind(BossCharacter bossCharacter)
        {
            Unbind();

            if (bossCharacter == null)
            {
                return;
            }

            Vital vital = bossCharacter.MyVital;
            if (vital == null)
            {
                return;
            }

            _bossCharacter = bossCharacter;
            _vital = vital;

            if (_vital.Life != null)
            {
                _vital.Life.OnValueChanged += OnLifeChanged;
                SetLife(_vital.Life);
            }

            if (_vital.Poise != null)
            {
                _vital.Poise.OnValueChanged += OnPoiseChanged;
                SetPoise(_vital.Poise);
            }
        }

        public void Unbind()
        {
            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged -= OnLifeChanged;
                }

                if (_vital.Poise != null)
                {
                    _vital.Poise.OnValueChanged -= OnPoiseChanged;
                }
            }

            _bossCharacter = null;
            _vital = null;
            ClearGauge();
        }

        private void SetLife(Life life)
        {
            if (_lifeGauge == null)
            {
                return;
            }

            if (life == null)
            {
                ClearGauge();
                return;
            }

            _lifeGauge.SetValueText(life.Current, life.Max);
            _lifeGauge.SetFrontValue(life.Rate);
        }

        private void SetPoise(Poise poise)
        {
            if (_poiseGauge == null)
            {
                return;
            }

            if (poise == null)
            {
                ClearPoiseGauge();
                return;
            }

            _poiseGauge.SetFrontValue(poise.Rate);
        }

        private void ClearGauge()
        {
            if (_lifeGauge == null)
            {
                return;
            }

            _lifeGauge.ResetValueText();
            _lifeGauge.ResetFrontValue();
            ClearPoiseGauge();
        }

        private void ClearPoiseGauge()
        {
            if (_poiseGauge == null)
            {
                return;
            }

            _poiseGauge.ResetFrontValue();
        }

        private void OnLifeChanged(int current, int max)
        {
            SetLife(_vital?.Life);
        }

        private void OnPoiseChanged(int current, int max)
        {
            SetPoise(_vital?.Poise);
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}