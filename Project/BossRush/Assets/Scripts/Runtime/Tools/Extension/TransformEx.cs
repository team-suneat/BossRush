using UnityEngine;

namespace TeamSuneat
{
    public static class TransformEx
    {
        public static void ResetLocalTransform(this GameObject go)
        {
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
        }

        public static void ResetLocalTransform(this Transform transform)
        {
            transform.localScale = Vector3.one;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public static void ResetLocalTransform(this Component component)
        {
            component.transform.localScale = Vector3.one;
            component.transform.localPosition = Vector3.zero;
            component.transform.localRotation = Quaternion.identity;
        }
    }
}