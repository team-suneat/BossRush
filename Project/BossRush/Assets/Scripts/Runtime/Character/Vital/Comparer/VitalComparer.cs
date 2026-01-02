using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class VitalComparer : IComparer<Vital>
    {
        public Vector3 Position;

        public virtual int Compare(Vital a, Vital b)
        {
            return 0;
        }
    }
}