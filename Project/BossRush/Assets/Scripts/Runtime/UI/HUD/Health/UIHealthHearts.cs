using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary> 하트 기반 체력 UI를 관리하는 클래스입니다. </summary>
    public class UIHealthHearts : XBehaviour
    {
        [FoldoutGroup("#UIHealthHearts")]
        [SerializeField] private Transform _heartsContainer;

        [FoldoutGroup("#UIHealthHearts")]
        [SerializeField] private Sprite _heartFillSprite;

        [FoldoutGroup("#UIHealthHearts")]
        [SerializeField] private Sprite _heartEmptySprite;

        [FoldoutGroup("#UIHealthHearts")]
        [SerializeField] private List<Image> _heartImages = new List<Image>();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _heartsContainer ??= this.FindTransform("HeartsContainer");

            if (_heartImages.Count == 0 && _heartsContainer != null)
            {
                Image[] childImages = _heartsContainer.GetComponentsInChildren<Image>(true);
                _heartImages.AddRange(childImages);
            }
        }


        /// <summary> 체력을 설정합니다. </summary>
        public void SetHealth(int current, int max)
        {
            if (max <= 0)
            {
                DeactivateAllHearts();
                return;
            }

            if (_heartImages.Count < max)
            {
                Log.Warning(LogTags.UI, "하트 이미지가 부족합니다. 필요: {0}, 현재: {1}. Inspector에서 충분한 하트 이미지를 할당해주세요.", max, _heartImages.Count);
            }

            UpdateHearts(current, max);
        }

        /// <summary> 하트 상태를 업데이트합니다. </summary>
        private void UpdateHearts(int current, int max)
        {
            for (int i = 0; i < _heartImages.Count; i++)
            {
                if (_heartImages[i] == null)
                {
                    continue;
                }

                if (i < max)
                {
                    _heartImages[i].gameObject.SetActive(true);

                    if (i < current)
                    {
                        _heartImages[i].sprite = _heartFillSprite;
                        _heartImages[i].color = Color.white;
                    }
                    else
                    {
                        _heartImages[i].sprite = _heartEmptySprite;
                        _heartImages[i].color = Color.white;
                    }
                }
                else
                {
                    _heartImages[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary> 모든 하트를 비활성화합니다. </summary>
        private void DeactivateAllHearts()
        {
            for (int i = 0; i < _heartImages.Count; i++)
            {
                if (_heartImages[i] != null && _heartImages[i].gameObject != null)
                {
                    _heartImages[i].gameObject.SetActive(false);
                }
            }
        }

    }
}

