using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public interface IPositionGroupSortStrategy
    {
        void Clear();

        Vector3 GetPosition(Vector3 originPosition, List<Transform> children);

        List<Vector3> GetPositions(Vector3 originPosition, List<Transform> children, int positionCount);

        List<Vector3> GetShufflePositions(Vector3 originPosition, List<Transform> children, int positionCount);
    }
}