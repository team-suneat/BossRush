using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    public class GridLayoutStrategy : ILayoutIndexStrategy
    {
        public void SetupIndices(UISelectElement[] elements, IndexerConfig config)
        {
            if (elements == null || elements.Length == 0)
            {
                Log.Warning(LogTags.UI_SelectEvent, "요소 배열이 비어 있습니다.");
                return;
            }

            int totalCount = elements.Length;
            if (config.Rows * config.Columns < totalCount)
            {
                Log.Warning(LogTags.UI_SelectEvent,
                    "그리드가 요소보다 작습니다. Rows * Columns : {0}, Elements : {1}",
                    config.Rows * config.Columns, totalCount);
                return;
            }

            if (config.IsCycling)
            {
                SetupCyclingGrid(elements, config);
            }
            else
            {
                SetupNonCyclingGrid(elements, config, totalCount);
                ConnectEdgesGrid(elements, config);
            }
        }

        private void SetupCyclingGrid(UISelectElement[] elements, IndexerConfig config)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                UISelectElement element = elements[i];
                if (element == null) continue;

                int finalIndex = config.DefaultIndex + i;
                element.SetSelectIndex(finalIndex);

                int row = i / config.Columns;
                int col = i % config.Columns;

                int actualRow = config.IsReverseVertical ? (config.Rows - 1 - row) : row;

                int downRow = (actualRow + 1) % config.Rows;
                if (config.IsReverseVertical) downRow = config.Rows - 1 - downRow;
                int downIdx = downRow * config.Columns + col;
                element.SelectDownIndex = config.DefaultIndex + downIdx;

                int upRow = (actualRow - 1 + config.Rows) % config.Rows;
                if (config.IsReverseVertical) upRow = config.Rows - 1 - upRow;
                int upIdx = upRow * config.Columns + col;
                element.SelectUpIndex = config.DefaultIndex + upIdx;

                int rightCol = (col + 1) % config.Columns;
                int rightIdx = row * config.Columns + rightCol;
                element.SelectRightIndex = config.DefaultIndex + rightIdx;

                int leftCol = (col - 1 + config.Columns) % config.Columns;
                int leftIdx = row * config.Columns + leftCol;
                element.SelectLeftIndex = config.DefaultIndex + leftIdx;

                Log.Info(LogTags.UI_SelectEvent,
                    "순환 GridIndex 설정 - row: {0}, col: {1}, 인덱스: {2}, Up: {3}, Down: {4}, Left: {5}, Right: {6}",
                    row, col, element.SelectIndex,
                    element.SelectUpIndex, element.SelectDownIndex,
                    element.SelectLeftIndex, element.SelectRightIndex);
            }
            Log.Info(LogTags.UI_SelectEvent, "순환 그리드 인덱스 설정이 완료되었습니다.");
        }

        private void SetupNonCyclingGrid(UISelectElement[] elements, IndexerConfig config, int totalCount)
        {
            int rowStart = config.IsReverseVertical ? config.Rows - 1 : 0;
            int rowEnd = config.IsReverseVertical ? -1 : config.Rows;
            int rowStep = config.IsReverseVertical ? -1 : 1;

            int colStart = config.IsReverseHorizontal ? config.Columns - 1 : 0;
            int colEnd = config.IsReverseHorizontal ? -1 : config.Columns;
            int colStep = config.IsReverseHorizontal ? -1 : 1;

            for (int row = rowStart; row != rowEnd; row += rowStep)
            {
                for (int col = colStart; col != colEnd; col += colStep)
                {
                    if (!IsValidGridIndex(row, col, config.Columns, elements.Length, out int currentIndex))
                    {
                        continue;
                    }

                    UISelectElement currentEvent = elements[currentIndex];
                    if (IsInvalidElement(currentEvent))
                    {
                        continue;
                    }

                    int finalIndex = config.DefaultIndex + currentIndex;
                    currentEvent.SetSelectIndex(finalIndex);

                    AssignDirectionalIndices(currentEvent, row, col, config, totalCount, elements);

                    Log.Info(LogTags.UI_SelectEvent,
                        "GridIndex 설정 - row: {0}, col: {1}, 인덱스: {2}, Up: {3}, Down: {4}, Left: {5}, Right: {6}",
                        row, col, currentEvent.SelectIndex,
                        currentEvent.SelectUpIndex, currentEvent.SelectDownIndex,
                        currentEvent.SelectLeftIndex, currentEvent.SelectRightIndex);
                }
            }
        }

        private bool IsValidGridIndex(int row, int col, int columns, int elementsLength, out int index)
        {
            index = (row * columns) + col;
            return row >= 0 && col >= 0 && index >= 0 && index < elementsLength;
        }

        private bool IsInvalidElement(UISelectElement element)
        {
            return element == null || !element.ActiveSelf || (element.Selectable == null && element.Clickable == null);
        }

        private void AssignDirectionalIndices(UISelectElement element, int row, int col, IndexerConfig config, int totalCount, UISelectElement[] elements)
        {
            int actualRow = config.IsReverseVertical ? (config.Rows - 1 - row) : row;

            int upRow = actualRow - 1;
            int downRow = actualRow + 1;

            if (config.IsReverseVertical)
            {
                upRow = config.Rows - 1 - upRow;
                downRow = config.Rows - 1 - downRow;
            }

            element.SelectUpIndex = GetGridIndexOrDefault(upRow, col, config, totalCount, elements);
            element.SelectDownIndex = GetGridIndexOrDefault(downRow, col, config, totalCount, elements);

            int leftCol = config.IsReverseHorizontal ? col + 1 : col - 1;
            int rightCol = config.IsReverseHorizontal ? col - 1 : col + 1;

            element.SelectLeftIndex = GetGridIndexOrDefault(row, leftCol, config, totalCount, elements);
            element.SelectRightIndex = GetGridIndexOrDefault(row, rightCol, config, totalCount, elements);
        }

        private int GetGridIndexOrDefault(int row, int col, IndexerConfig config, int totalCount, UISelectElement[] elements)
        {
            int idx = (row * config.Columns) + col;
            if (row < 0 || col < 0 || row >= config.Rows || col >= config.Columns || idx >= totalCount)
            {
                return -1;
            }

            UISelectElement element = elements[idx];
            return IsInvalidElement(element) ? -1 : config.DefaultIndex + idx;
        }

        private void ConnectEdgesGrid(UISelectElement[] elements, IndexerConfig config)
        {
            if (elements == null || elements.Length == 0)
            {
                return;
            }

            Log.Info(LogTags.UI_SelectEvent, "그리드 가장자리 연결을 시작합니다.");
            for (int row = 0; row < config.Rows; row++)
            {
                ConnectRowEdges(row, elements, config);
            }

            for (int col = 0; col < config.Columns; col++)
            {
                ConnectColumnEdges(col, elements, config);
            }
            Log.Info(LogTags.UI_SelectEvent, "그리드 가장자리 연결이 완료되었습니다.");
        }

        private void ConnectRowEdges(int row, UISelectElement[] elements, IndexerConfig config)
        {
            int leftIndex = row * config.Columns;
            int rightIndex = leftIndex + config.Columns - 1;

            if (!TryGetElement(leftIndex, elements, out UISelectElement leftEvent))
            {
                return;
            }

            if (!TryGetElement(rightIndex, elements, out UISelectElement rightEvent))
            {
                return;
            }

            leftEvent.SelectLeftIndex = config.DefaultIndex + rightIndex;
            rightEvent.SelectRightIndex = config.DefaultIndex + leftIndex;

            Log.Info(LogTags.UI_SelectEvent,
                "행 가장자리 연결 - 행: {0}, 왼쪽: {1}({2}), 오른쪽: {3}({4})",
                row, leftEvent.GetHierarchyName(), leftEvent.SelectLeftIndex,
                rightEvent.GetHierarchyName(), rightEvent.SelectRightIndex);
        }

        private void ConnectColumnEdges(int col, UISelectElement[] elements, IndexerConfig config)
        {
            int? topIndex = null;
            int? bottomIndex = null;

            for (int row = config.Rows - 1; row >= 0; row--)
            {
                int idx = row * config.Columns + col;
                if (idx >= elements.Length) continue;
                if (TryGetElement(idx, elements, out _))
                {
                    bottomIndex = idx;
                    break;
                }
            }

            for (int row = 0; row < config.Rows; row++)
            {
                int idx = row * config.Columns + col;
                if (idx >= elements.Length) continue;
                if (TryGetElement(idx, elements, out _))
                {
                    topIndex = idx;
                    break;
                }
            }

            if (topIndex.HasValue && bottomIndex.HasValue)
            {
                UISelectElement topEvent = elements[topIndex.Value];
                UISelectElement bottomEvent = elements[bottomIndex.Value];

                topEvent.SelectDownIndex = config.DefaultIndex + bottomIndex.Value;
                bottomEvent.SelectUpIndex = config.DefaultIndex + topIndex.Value;

                Log.Info(LogTags.UI_SelectEvent,
                    "열 가장자리 연결 - 열: {0}, 위: {1}({2}), 아래: {3}({4})",
                    col,
                    topEvent.GetHierarchyName(), topEvent.SelectDownIndex,
                    bottomEvent.GetHierarchyName(), bottomEvent.SelectUpIndex);
            }
        }

        private bool TryGetElement(int index, UISelectElement[] elements, out UISelectElement element)
        {
            element = null;

            if (index < 0 || index >= elements.Length)
            {
                return false;
            }

            element = elements[index];
            return element != null && element.ActiveSelf;
        }
    }
}
