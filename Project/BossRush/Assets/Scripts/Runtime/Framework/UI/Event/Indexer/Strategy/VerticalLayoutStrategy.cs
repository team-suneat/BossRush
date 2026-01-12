using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    public class VerticalLayoutStrategy : ILayoutIndexStrategy
    {
        public void SetupIndices(UISelectElement[] elements, IndexerConfig config)
        {
            List<UISelectElement> validElements = GetValidElements(elements);
            if (validElements.Count == 0)
            {
                return;
            }

            if (config.IsReverseVertical)
            {
                validElements.Reverse();
            }

            for (int i = 0; i < validElements.Count; i++)
            {
                UISelectElement currentElement = validElements[i];
                int currentIndex = config.DefaultIndex + i;
                currentElement.SetSelectIndex(currentIndex);
                AssignVerticalIndices(currentElement, i, validElements.Count, config);

                Log.Info(LogTags.UI_SelectEvent,
                    "이벤트의 인덱스를 설정합니다. {0}, {1}",
                    currentElement.GetHierarchyName(), currentIndex);
            }

            if (config.IsCycling && validElements.Count > 0)
            {
                UISelectElement first = validElements[0];
                UISelectElement last = validElements[^1];
                first.SelectUpIndex = config.DefaultIndex + (validElements.Count - 1);
                last.SelectDownIndex = config.DefaultIndex;

                Log.Info(LogTags.UI_SelectEvent,
                    "순환 선택 설정 - 첫 번째 요소: {0}(위쪽->{1}), 마지막 요소: {2}(아래쪽->{3})",
                    first.GetHierarchyName(), first.SelectUpIndex,
                    last.GetHierarchyName(), last.SelectDownIndex);
            }
        }

        private void AssignVerticalIndices(UISelectElement element, int index, int count, IndexerConfig config)
        {
            element.SelectUpIndex = index > 0 ? config.DefaultIndex + (index - 1) : -1;
            element.SelectDownIndex = index < count - 1 ? config.DefaultIndex + index + 1 : -1;
        }

        private List<UISelectElement> GetValidElements(UISelectElement[] elements)
        {
            List<UISelectElement> result = new();
            if (elements == null)
            {
                return result;
            }

            foreach (UISelectElement element in elements)
            {
                if (element != null && element.ActiveSelf && (element.Selectable != null || element.Clickable != null))
                {
                    result.Add(element);
                }
            }
            return result;
        }
    }
}
