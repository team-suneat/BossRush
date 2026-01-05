using UnityEngine;

namespace TeamSuneat
{
    public class FarthestVitalComparer : VitalComparer
    {
        public override int Compare(Vital a, Vital b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return 1;
            if (b == null) return -1;

            float distanceA = Vector3.Distance(a.position, Position);
            float distanceB = Vector3.Distance(b.position, Position);

            // 거리가 먼 것을 우선하도록 비교 순서를 반대로 변경
            return distanceB.CompareTo(distanceA);
        }
    }
}