using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDBossGauge : XBehaviour
    {
        [FoldoutGroup("#HUDBossGauge")]
        [SerializeField] private UIGauge _gauge;

        private BossCharacter _bossCharacter;
        private Vital _vital;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _gauge ??= GetComponentInChildren<UIGauge>();
        }

        private void Update()
        {
            _gauge?.LogicUpdate();
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
        }

        public void Unbind()
        {
            if (_vital != null)
            {
                if (_vital.Life != null)
                {
                    _vital.Life.OnValueChanged -= OnLifeChanged;
                }
            }

            _bossCharacter = null;
            _vital = null;
            ClearGauge();
        }

        private void SetLife(Life life)
        {
            if (_gauge == null)
            {
                return;
            }

            if (life == null)
            {
                ClearGauge();
                return;
            }

            _gauge.SetValueText(life.Current, life.Max);
            _gauge.SetFrontValue(life.Rate);
        }

        private void ClearGauge()
        {
            if (_gauge == null)
            {
                return;
            }

            _gauge.ResetValueText();
            _gauge.ResetFrontValue();
        }

        private void OnLifeChanged(int current, int max)
        {
            SetLife(_vital?.Life);
        }

        private void OnDestroy()
        {
            Unbind();
        }
    }
}