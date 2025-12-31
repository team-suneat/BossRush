using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDStageProgressGauge : XBehaviour
    {
        [SerializeField]
        private UIGauge _gauge;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _gauge ??= GetComponentInChildren<UIGauge>();
        }

        public void SetProgress(int currentWave, int totalWave)
        {
            if (_gauge == null)
            {
                return;
            }

            _gauge.SetFrontValue(currentWave.SafeDivide01(totalWave));
            _gauge.SetValueText($"{currentWave}/{totalWave}");
        }
    }
}