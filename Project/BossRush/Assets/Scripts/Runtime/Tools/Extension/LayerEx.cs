using UnityEngine;

namespace TeamSuneat
{
    public static class LayerEx
    {
        public static void SetLayer(this GameObject obj, int newLayer)
        {
            if (null != obj)
            {
                obj.layer = newLayer;
            }
        }

        public static void SetLayerRecursively(this GameObject obj, int newLayer)
        {
            if (null != obj)
            {
                InfiniteLoopDetector.Reset();

                obj.layer = newLayer;
                System.Collections.IEnumerator enumerator = obj.transform.GetEnumerator();

                while (enumerator.MoveNext())
                {
                    Transform child = (Transform)enumerator.Current;
                    SetLayerRecursively(child.gameObject, newLayer);

                    InfiniteLoopDetector.Run();
                }
            }
        }

        public static int ConcatLayer(int layer1, int layer2)
        {
            return (1 << layer1 | 1 << layer2);
        }

        public static int ConcatLayer(int layer1, int layer2, int layer3)
        {
            return (1 << layer1 | 1 << layer2 | 1 << layer3);
        }

        public static int ConcatMask(params int[] layerMasks)
        {
            int resultMask = 0;

            for (int i = 0; i < layerMasks.Length; i++)
            {
                resultMask = resultMask | layerMasks[i];
            }

            return resultMask;
        }

        public static int ConcatMask(int layerMask1, int layerMask2)
        {
            return (layerMask1 | layerMask2);
        }

        public static bool IsInMask(Collider2D collider, int layerMask)
        {
            return (layerMask & 1 << collider.gameObject.layer) != 0;
        }

        public static bool IsInMask(GameObject obj, int layerMask)
        {
            return (layerMask & 1 << obj.layer) != 0;
        }

        public static bool IsInMask(int layer, int layerMask)
        {
            return (layerMask & 1 << layer) != 0;
        }
    }
}