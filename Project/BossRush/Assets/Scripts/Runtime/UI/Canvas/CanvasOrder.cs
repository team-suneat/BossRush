using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasOrder : XBehaviour
    {
        [FoldoutGroup("#CanvasOrder")]
        public CanvasOrderNames OrderName;

        [FoldoutGroup("#CanvasOrder")]
        public Canvas Canvas;

        [FoldoutGroup("#CanvasOrder")]
        public CanvasGroup CanvasGroup;

        [FoldoutGroup("#CanvasOrder")]
        public GraphicRaycaster Raycaster;

        [FoldoutGroup("#CanvasOrder")]
        public RectTransform[] IngameCanvasRect;

        [FoldoutGroup("#CanvasOrder")]
        public CanvasScaler Scaler;

        public int OrderTID => BitConvert.Enum32ToInt(OrderName);

        public override void AutoGetComponents()
        {
            Canvas = GetComponent<Canvas>();
            CanvasGroup = GetComponent<CanvasGroup>();
            Raycaster = GetComponent<GraphicRaycaster>();
            Scaler = GetComponent<CanvasScaler>();
        }

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
            if (OrderName == CanvasOrderNames.IngameWorldSpace)
            {
                rectTransform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);
            }
        }

        public void Activate()
        {
            Canvas.enabled = true;
        }

        public void Deactivate()
        {
            Canvas.enabled = false;
        }

        public void SetSortingOrder()
        {
            if (Canvas != null)
            {
                Canvas.sortingOrder = OrderTID;
            }
        }

        public void Show()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 1;
            }
        }

        public void Hide()
        {
            if (CanvasGroup != null)
            {
                CanvasGroup.alpha = 0;
            }
        }

        public void SetEnabledRaycaster(bool isEnabled)
        {
            if (Raycaster != null)
            {
                Raycaster.enabled = isEnabled;
            }
        }
    }
}