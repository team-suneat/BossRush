using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class ObjectPaddingInOrder : XBehaviour
    {
        public enum ObjectAlignments
        {
            None,
            Center, Left, Right,
        }

        [Title("#Object Padding In Order", "Position")]
        public ObjectAlignments Alignment;

        [EnableIf("Alignment", ObjectAlignments.Center)]
        public bool IsReverseFirstWhenCenter;

        public bool isVertical;
        public float Padding;
        public Vector2 PaddingOffset;

        [Title("#Object Padding In Order", "Renderer")]
        public bool SortRendererOrder;
        public bool SortAscending;
        public int SortOrderOffset;

        [Title("#Object Padding In Order", "Editor")]
        public bool ShowGizmo;

        private List<Transform> children = new List<Transform>();

        public override void AutoSetting()
        {
            ResetLocalPositionInChildren();
            PaddingInOrder();
        }

        public void ResetLocalPositionInChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                child.localPosition = Vector3.zero;
            }
        }

        public void PaddingInOrder()
        {
            SetChildren();

            Vector3[] positions;

            if (isVertical)
            {
                positions = GetVerticalPositions(transform.position, Alignment, Padding, children.Count);
            }
            else
            {
                positions = GetHorizontalPositions(transform.position, Alignment, Padding, children.Count);
            }

            ApplyPositions(children, positions);
            ApplySortingIfEnabled();
        }

        private void SortingRendererInOrder()
        {
            if (!SortRendererOrder)
                return;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            if (renderers != null)
            {
                if (SortAscending)
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].sortingOrder = (renderers.Length - i) + SortOrderOffset;
                    }
                }
                else
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].sortingOrder = i + SortOrderOffset;
                    }
                }
            }
        }

        private void SetChildren()
        {
            children.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (child.gameObject.activeSelf)
                {
                    children.Add(child);
                }
            }
        }

        public Vector3[] GetHorizontalPositions(Vector3 position, ObjectAlignments alignments, float padding, int count)
        {
            if (alignments == ObjectAlignments.Center)
            {
                return BuildCenterPositions(position, false, padding, count, IsReverseFirstWhenCenter);
            }
            if (alignments == ObjectAlignments.Left)
            {
                return BuildLinearPositions(position, Vector3.left, padding, count);
            }
            if (alignments == ObjectAlignments.Right)
            {
                return BuildLinearPositions(position, Vector3.right, padding, count);
            }

            return new Vector3[count];
        }

        public Vector3[] GetVerticalPositions(Vector3 position, ObjectAlignments alignments, float padding, int count)
        {
            if (alignments == ObjectAlignments.Center)
            {
                return BuildCenterPositions(position, true, padding, count, IsReverseFirstWhenCenter);
            }
            if (alignments == ObjectAlignments.Left)
            {
                return BuildLinearPositions(position, Vector3.down, padding, count);
            }
            if (alignments == ObjectAlignments.Right)
            {
                return BuildLinearPositions(position, Vector3.up, padding, count);
            }

            return new Vector3[count];
        }

        private float CalculateCenterIntervalByPairIndex(int pairIndex, float padding)
        {
            float halfPadding = padding * 0.5f;
            return (2 * pairIndex + 1) * halfPadding;
        }

        private Vector3 GetAlternatingDirection(int index, bool isVerticalDirection, bool reverseFirst)
        {
            if (isVerticalDirection)
            {
                return reverseFirst
                    ? (index % 2 == 0 ? Vector3.down : Vector3.up)
                    : (index % 2 == 0 ? Vector3.up : Vector3.down);
            }

            return reverseFirst
                ? (index % 2 == 0 ? Vector3.right : Vector3.left)
                : (index % 2 == 0 ? Vector3.left : Vector3.right);
        }

        private Vector3[] BuildCenterPositions(Vector3 origin, bool isVerticalDirection, float padding, int count, bool reverseFirst)
        {
            Vector3[] result = new Vector3[count];
            if (count <= 0)
            {
                return result;
            }

            if (count % 2 == 1)
            {
                // Odd count: center at origin, then distances grow by padding (1,1,2,2,...)
                result[0] = origin;
                for (int i = 1; i < count; i++)
                {
                    int magnitudeFactor = Mathf.CeilToInt(i * 0.5f); // 1,1,2,2,3,3,...
                    float interval = magnitudeFactor * padding;

                    Vector3 direction;
                    if (isVerticalDirection)
                    {
                        direction = reverseFirst
                            ? (i % 2 == 1 ? Vector3.up : Vector3.down)
                            : (i % 2 == 1 ? Vector3.down : Vector3.up);
                    }
                    else
                    {
                        direction = reverseFirst
                            ? (i % 2 == 1 ? Vector3.right : Vector3.left)
                            : (i % 2 == 1 ? Vector3.left : Vector3.right);
                    }

                    result[i] = origin + (direction * interval);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int pairIndex = i / 2;
                    float interval = CalculateCenterIntervalByPairIndex(pairIndex, padding);
                    Vector3 direction = GetAlternatingDirection(i, isVerticalDirection, reverseFirst);
                    result[i] = origin + (direction * interval);
                }
            }

            return result;
        }

        private Vector3[] BuildLinearPositions(Vector3 origin, Vector3 baseDirection, float padding, int count)
        {
            Vector3[] result = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = origin + (baseDirection * padding * i);
            }
            return result;
        }

        private void ApplyPositions(IReadOnlyList<Transform> targets, IReadOnlyList<Vector3> positions)
        {
            int length = Mathf.Min(targets.Count, positions.Count);
            for (int i = 0; i < length; i++)
            {
                targets[i].position = positions[i];
            }
        }

        private void ApplySortingIfEnabled()
        {
            SortingRendererInOrder();
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (ShowGizmo)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    GizmoEx.DrawWireSphere(transform.GetChild(i).position, 0.1f);
                }
            }
        }

#endif
    }
}