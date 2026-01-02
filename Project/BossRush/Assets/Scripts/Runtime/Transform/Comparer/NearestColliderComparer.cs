using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class NearestCollider2DComparer : IComparer<Collider2D>
    {
        public Vector3 Position;

        public int Compare(Collider2D a, Collider2D b)
        {
            if (a != null && b != null)
            {
                float distanceA = Vector3.Distance(a.transform.position, Position);
                float distanceB = Vector3.Distance(b.transform.position, Position);

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