using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class PositionGroupLink : XBehaviour
    {
        [Title("#PositionGroupLink")]
        [InfoBox("Source의 자식 위치를 Target 자식에 인덱스대로 복사합니다.\nTarget이 비어 있으면 같은 오브젝트의 PositionGroup을 사용합니다.")]
        [SerializeField]
        private PositionGroup _source;

        [SerializeField]
        [LabelText("Target (비우면 자신)")]
        private PositionGroup _target;

        public PositionGroup Source => _source;
        public PositionGroup Target => _target != null ? _target : this.GetComponent<PositionGroup>();

        public void PasteChildrenPositions()
        {
            PositionGroup targetGroup = Target;
            if (_source == null || targetGroup == null)
            {
                return;
            }

            List<Transform> sourceChildren = _source.Children;
            List<Transform> targetChildren = targetGroup.Children;
            if (sourceChildren == null || targetChildren == null)
            {
                return;
            }

            int count = sourceChildren.Count < targetChildren.Count ? sourceChildren.Count : targetChildren.Count;
            for (int i = 0; i < count; i++)
            {
                if (sourceChildren[i] != null && targetChildren[i] != null)
                {
                    targetChildren[i].position = sourceChildren[i].position;
                }
            }
        }

#if UNITY_EDITOR

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("Paste Children Positions", ButtonSizes.Medium)]
        private void PasteChildrenPositionsButton()
        {
            PasteChildrenPositions();
        }

#endif
    }
}
