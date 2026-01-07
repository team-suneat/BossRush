using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public enum VitalColliderTypes
    {
        None,
        Default,
        Type1,
        Type2,
        Type3,
    }

    [Serializable]
    public class VitalColliderHandler
    {
        public List<VitalColliderData> DataList;

        public VitalColliderData Find(string typeString)
        {
            if (DataList != null)
            {
                if (DataList.Count > 0)
                {
                    for (int i = 0; i < DataList.Count; i++)
                    {
                        if (DataList[i].TypeString == typeString)
                        {
                            return DataList[i];
                        }
                    }
                }
            }

            return null;
        }
    }

    [Serializable]
    public class VitalColliderData
    {
        public string TypeString;
        public Vector2 Offset;
        public Vector2 Size;
    }
}