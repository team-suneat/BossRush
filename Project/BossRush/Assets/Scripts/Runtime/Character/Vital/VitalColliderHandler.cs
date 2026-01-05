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
        public List<VitalColliderData> VitalColliderDatas;

        public VitalColliderData Find(string typeString)
        {
            if (VitalColliderDatas != null)
            {
                if (VitalColliderDatas.Count > 0)
                {
                    for (int i = 0; i < VitalColliderDatas.Count; i++)
                    {
                        if (VitalColliderDatas[i].TypeString == typeString)
                        {
                            return VitalColliderDatas[i];
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