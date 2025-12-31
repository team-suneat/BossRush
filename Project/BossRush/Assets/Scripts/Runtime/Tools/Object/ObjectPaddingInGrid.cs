using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class ObjectPaddingInGrid : XBehaviour
    {
        public enum Corner
        { UpperLeft, UpperRight, LowerLeft, LowerRight }

        public enum Axis
        { Horizontal, Vertical }

        public enum Constraint
        { Flexible, FixedColumnCount, FixedRowCount }

        [Title("Grid Settings")]
        public Vector2 cellSize = new(1, 1);
        public Vector2 spacing = new(0.1f, 0.1f);
        public Corner startCorner = Corner.UpperLeft;
        public Axis startAxis = Axis.Horizontal;
        public Constraint constraint = Constraint.Flexible;
        public int constraintCount = 2;

        [Title("Child Objects")]
        public Transform[] items;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            items = this.GetComponentsInOnlyChildren<Transform>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            AlignGrid();
        }
        
        public void AlignGrid()
        {
            if (items == null || items.Length == 0)
            {
                return;
            }

            int row = 0, column = 0; // �ʱ�ȭ
            for (int i = 0; i < items.Length; i++)
            {
                switch (constraint)
                {
                    case Constraint.FixedColumnCount:
                        column = i % constraintCount;
                        row = i / constraintCount;
                        break;

                    case Constraint.FixedRowCount:
                        row = i % constraintCount;
                        column = i / constraintCount;
                        break;

                    case Constraint.Flexible:
                        column = i % (int)Mathf.Sqrt(items.Length);
                        row = i / (int)Mathf.Sqrt(items.Length);
                        break;
                }

                Vector2 newPosition = CalculatePosition(row, column);
                if (items[i] != null)
                {
                    items[i].localPosition = newPosition;
                }
            }
        }

        private Vector2 CalculatePosition(int row, int column)
        {
            float x = column * (cellSize.x + spacing.x);
            float y = row * (cellSize.y + spacing.y);

            switch (startCorner)
            {
                case Corner.UpperRight:
                    x = -x;
                    break;

                case Corner.LowerLeft:
                    y = -y;
                    break;

                case Corner.LowerRight:
                    x = -x;
                    y = -y;
                    break;
            }

            if (startAxis == Axis.Vertical)
            {
                return new Vector2(y, x);
            }
            return new Vector2(x, y);
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            AlignGrid();
        }
    }
}