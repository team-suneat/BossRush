using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace TeamSuneat
{
    public class RandomSortStrategy : IPositionGroupSortStrategy
    {
        public void Clear()
        {
        }

        public Vector3 GetPosition(Vector3 originPosition, List<Transform> children)
        {
            int index = RandomEx.Range(0, children.Count);
            return children[index].position;
        }

        public List<Vector3> GetPositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            return Enumerable.Range(0, positionCount).Select(_ => children[RandomEx.Range(0, children.Count)].position).ToList();
        }

        public List<Vector3> GetShufflePositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            return GetPositions(originPosition, children, positionCount).OrderBy(_ => RandomEx.Range(0, children.Count)).ToList();
        }
    }
}