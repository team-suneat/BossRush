using UnityEngine;

namespace TeamSuneat
{
    public class VFXSpriteRenderer : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetSortingLayerName(string layerName)
        {
            _renderer.SetSortingLayer(layerName);
        }

        public void SetSortingOrder(int order)
        {
            _renderer.SetSortingOrder(order);
        }
    }
}