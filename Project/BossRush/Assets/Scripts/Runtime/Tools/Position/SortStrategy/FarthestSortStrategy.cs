using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace TeamSuneat
{
    public class FarthestSortStrategy : IPositionGroupSortStrategy
    {
        public void Clear()
        {
        }

        public Vector3 GetPosition(Vector3 originPosition, List<Transform> children)
        {
            IOrderedEnumerable<Transform> ordered = children.OrderByDescending(child => Vector3.Distance(originPosition, child.position));
            Transform point = ordered.FirstOrDefault();

            if (point != null)
            {
                return point.position;
            }

            return Vector3.zero;
        }

        public List<Vector3> GetPositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            return children.OrderByDescending(child => Vector3.Distance(originPosition, child.position)).Take(positionCount).Select(child => child.position).ToList();
        }

        public List<Vector3> GetShufflePositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            return GetPositions(originPosition, children, positionCount).OrderBy(_ => RandomEx.Range(0, children.Count)).ToList();
        }
    }
}