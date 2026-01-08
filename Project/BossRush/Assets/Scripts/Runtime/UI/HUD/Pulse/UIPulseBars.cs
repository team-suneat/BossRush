using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary> 펄스 게이지를 한 칸 단위로 표시하는 클래스입니다. </summary>
    public class UIPulseBars : XBehaviour
    {
        [FoldoutGroup("#UIPulseBars")]
        [SerializeField] private Transform _barsContainer;

        [FoldoutGroup("#UIPulseBars")]
        [SerializeField] private List<UIPulseBar> _pulseBars = new List<UIPulseBar>();

        private float _currentGaugeProgress = 0f;
        private int _currentFullPulseCount = 0;
        private int _currentMaxPulseCount = 0;

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

        /// <summary> 게이지 진행도를 설정합니다. </summary>
        /// <param name="progress">게이지 진행도 (0~1)</param>
        public void SetGaugeProgress(float progress)
        {
            _currentGaugeProgress = Mathf.Clamp01(progress);
            UpdateBars();
        }

        /// <summary> 온전한 펄스 개수를 설정합니다. </summary>
        /// <param name="count">현재 온전한 펄스 개수</param>
        /// <param name="maxCount">최대 온전한 펄스 개수</param>
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

        /// <summary> 펄스 바 상태를 업데이트합니다. </summary>
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

                    // 그레이 이미지는 항상 fillAmount 1.0 (배경 역할)
                    float grayAmount = 1f;

                    // 온전한 펄스는 fillAmount 1.0, 그 외는 게이지 진행도
                    float fillAmount = 0f;
                    if (i < _currentFullPulseCount)
                    {
                        // 온전한 펄스
                        fillAmount = 1f;
                    }
                    else if (i == _currentFullPulseCount)
                    {
                        // 마지막 부분 채워진 펄스는 게이지 진행도로 표시
                        fillAmount = _currentGaugeProgress;
                    }

                    _pulseBars[i].SetFillAmount(grayAmount, fillAmount);
                }
                else
                {
                    _pulseBars[i].SetActive(false);
                }
            }
        }

        /// <summary> 모든 펄스 바를 비활성화합니다. </summary>
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

