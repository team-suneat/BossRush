using Sirenix.OdinInspector;
using TeamSuneat.CameraSystem.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasOrder : XBehaviour
    {
        [FoldoutGroup("#CanvasOrder")]
        public CanvasOrderNames OrderName;

        private Canvas _canvas;
        private GraphicRaycaster _raycaster;

        public int OrderTID => BitConvert.Enum32ToInt(OrderName);

        public override void AutoSetting()
        {
            base.AutoSetting();
            SetSortingOrder();
        }

        public override void AutoNaming()
        {
            int order = BitConvert.Enum32ToInt(OrderName);
            SetGameObjectName(order.ToString() + ". Canvas (" + OrderName.ToString() + ")");
        }

        protected void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
        }

        public void SetSortingOrder()
        {
            if (_canvas != null)
            {
                _canvas.sortingOrder = OrderTID;
            }
        }

        public void SetEnabledRaycast(bool isEnabled)
        {
            if (_raycaster != null)
            {
                _raycaster.enabled = isEnabled;
            }
        }
    }
}