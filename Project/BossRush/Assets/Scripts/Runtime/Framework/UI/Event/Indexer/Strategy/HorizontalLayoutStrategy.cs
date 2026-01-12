using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    public class HorizontalLayoutStrategy : ILayoutIndexStrategy
    {
        public void SetupIndices(UISelectElement[] elements, IndexerConfig config)
        {
            List<UISelectElement> validElements = GetValidElements(elements);
            if (validElements.Count == 0)
            {
                return;
            }

            if (config.IsReverseHorizontal)
            {
                validElements.Reverse();
            }

            for (int i = 0; i < validElements.Count; i++)
            {
                UISelectElement currentElement = validElements[i];
                int currentIndex = config.DefaultIndex + i;
                currentElement.SetSelectIndex(currentIndex);
                AssignHorizontalIndices(currentElement, i, validElements.Count, config);

                Log.Info(LogTags.UI_SelectEvent,
                    "이벤트의 인덱스를 설정합니다. {0}, {1}",
                    currentElement.GetHierarchyName(), currentIndex);
            }

            if (config.IsCycling && validElements.Count > 0)
            {
                validElements[0].SelectLeftIndex = config.DefaultIndex + (validElements.Count - 1);
                validElements[^1].SelectRightIndex = config.DefaultIndex;
            }
        }

        private void AssignHorizontalIndices(UISelectElement element, int index, int count, IndexerConfig config)
        {
            element.SelectLeftIndex = index > 0 ? config.DefaultIndex + (index - 1) : -1;
            element.SelectRightIndex = index < count - 1 ? config.DefaultIndex + index + 1 : -1;
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
