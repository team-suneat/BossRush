using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary> 마나 게이지 및 온전한 마나 오브를 표시하는 클래스입니다. </summary>
    public class UIManaGauge : XBehaviour
    {
        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private Image _manaIcon;

        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private UIGauge _manaGaugeBar;

        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private Transform _manaOrbsContainer;

        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private Sprite _manaOrbFillSprite;

        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private Sprite _manaOrbEmptySprite;

        [FoldoutGroup("#UIManaGauge")]
        [SerializeField] private List<Image> _manaOrbImages = new List<Image>();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _manaIcon ??= this.FindComponent<Image>("ManaIcon");
            _manaGaugeBar ??= this.FindComponent<UIGauge>("ManaGaugeBar");
            _manaOrbsContainer ??= this.FindTransform("ManaOrbsContainer");

            if (_manaOrbImages.Count == 0 && _manaOrbsContainer != null)
            {
                Image[] childImages = _manaOrbsContainer.GetComponentsInChildren<Image>(true);
                _manaOrbImages.AddRange(childImages);
            }
        }


        /// <summary> 게이지 진행도를 설정합니다. </summary>
        public void SetGaugeProgress(float progress)
        {
            if (_manaGaugeBar != null)
            {
                _manaGaugeBar.SetFrontValue(Mathf.Clamp01(progress));
            }
        }

        /// <summary> 온전한 마나 개수를 설정합니다. </summary>
        public void SetFullManaCount(int count, int maxCount)
        {
            if (maxCount <= 0)
            {
                DeactivateAllOrbs();
                return;
            }

            if (_manaOrbImages.Count < maxCount)
            {
                Log.Warning(LogTags.UI, "마나 오브 이미지가 부족합니다. 필요: {0}, 현재: {1}. Inspector에서 충분한 마나 오브 이미지를 할당해주세요.", maxCount, _manaOrbImages.Count);
            }

            UpdateOrbs(count, maxCount);
        }

        /// <summary> 마나 오브 상태를 업데이트합니다. </summary>
        private void UpdateOrbs(int count, int maxCount)
        {
            for (int i = 0; i < _manaOrbImages.Count; i++)
            {
                if (_manaOrbImages[i] == null)
                {
                    continue;
                }

                if (i < maxCount)
                {
                    _manaOrbImages[i].gameObject.SetActive(true);

                    if (i < count)
                    {
                        _manaOrbImages[i].sprite = _manaOrbFillSprite;
                        _manaOrbImages[i].color = Color.white;
                    }
                    else
                    {
                        _manaOrbImages[i].sprite = _manaOrbEmptySprite;
                        _manaOrbImages[i].color = Color.white;
                    }
                }
                else
                {
                    _manaOrbImages[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary> 모든 마나 오브를 비활성화합니다. </summary>
        private void DeactivateAllOrbs()
        {
            for (int i = 0; i < _manaOrbImages.Count; i++)
            {
                if (_manaOrbImages[i] != null && _manaOrbImages[i].gameObject != null)
                {
                    _manaOrbImages[i].gameObject.SetActive(false);
                }
            }
        }

    }
}

