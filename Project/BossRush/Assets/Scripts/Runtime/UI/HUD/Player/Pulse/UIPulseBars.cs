using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class UIPulseBars : XBehaviour
    {
        [FoldoutGroup("#UIPulseBars")]
        [SerializeField] private Transform _barsContainer;

        [FoldoutGroup("#UIPulseBars")]
        [SerializeField] private List<UIPulseBar> _pulseBars = new List<UIPulseBar>();

        private float _currentGaugeProgress = 0f;
        private int _currentFullPulseCount = 0;
        private int _currentMaxPulseCount = 0;
        private bool _isBurnout = false;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _barsContainer ??= this.FindTransform("BarsContainer");

            if (_pulseBars.Count == 0 && _barsContainer != null)
            {
                UIPulseBar[] childBars = _barsContainer.GetComponentsInChildren<UIPulseBar>(true);
                for (int i = 0; i < childBars.Length; i++)
                {
                    _pulseBars.Add(childBars[i]);
                }
            }
        }

        public void SetGaugeProgress(float progress)
        {
            _currentGaugeProgress = Mathf.Clamp01(progress);
            UpdateBars();
        }

        public void SetFullPulseCount(int count, int maxCount)
        {
            if (maxCount <= 0)
            {
                DeactivateAllBars();
                return;
            }

            if (_pulseBars.Count < maxCount)
            {
                Log.Warning(LogTags.UI, "펄스 바가 부족합니다. 필요: {0}, 현재: {1}. Inspector에서 충분한 펄스 바를 할당해주세요.", maxCount, _pulseBars.Count);
            }

            _currentFullPulseCount = count;
            _currentMaxPulseCount = maxCount;
            UpdateBars();
        }

        public void SetBurnoutState(bool isBurnout)
        {
            _isBurnout = isBurnout;
            UpdateBars();
        }

        private void UpdateBars()
        {
            for (int i = 0; i < _pulseBars.Count; i++)
            {
                if (_pulseBars[i] == null)
                {
                    continue;
                }

                if (i < _currentMaxPulseCount)
                {
                    _pulseBars[i].SetActive(true);

                    // 번아웃 상태에 따라 이미지 활성화/비활성화
                    _pulseBars[i].SetBurnoutState(_isBurnout);

                    float grayAmount = 1f;
                    float fillAmount = 0f;

                    if (_isBurnout)
                    {
                        // 번아웃 상태일 때: 그레이 이미지가 게이지 진행도를 표시
                        if (i < _currentFullPulseCount)
                        {
                            grayAmount = 1f;
                        }
                        else if (i == _currentFullPulseCount)
                        {
                            grayAmount = _currentGaugeProgress;
                        }
                        else
                        {
                            grayAmount = 0f;
                        }
                        fillAmount = 0f; // fill 이미지는 비활성화되어 있음
                    }
                    else
                    {
                        // 번아웃이 아닐 때: fill 이미지가 게이지 진행도를 표시
                        grayAmount = 1f;
                        if (i < _currentFullPulseCount)
                        {
                            fillAmount = 1f;
                        }
                        else if (i == _currentFullPulseCount)
                        {
                            fillAmount = _currentGaugeProgress;
                        }
                    }

                    _pulseBars[i].SetFillAmount(grayAmount, fillAmount);
                }
                else
                {
                    _pulseBars[i].SetActive(false);
                }
            }
        }

        private void DeactivateAllBars()
        {
            for (int i = 0; i < _pulseBars.Count; i++)
            {
                if (_pulseBars[i] != null)
                {
                    _pulseBars[i].SetActive(false);
                }
            }
        }
    }
}