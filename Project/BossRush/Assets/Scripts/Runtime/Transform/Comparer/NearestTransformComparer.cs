using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class NearestTransformComparer : IComparer<Transform>
    {
        public Vector3 Position;

        public int Compare(Transform a, Transform b)
        {
            if (a != null && b != null)
            {
                float distanceA = Vector3.Distance(a.position, Position);
                float distanceB = Vector3.Distance(b.position, Position);

                if (distanceA < distanceB)
                {
                    return -1;
                }
                else if (distanceA > distanceB)
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}