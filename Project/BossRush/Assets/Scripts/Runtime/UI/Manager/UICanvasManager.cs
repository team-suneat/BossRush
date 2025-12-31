using System.Collections.Generic;

namespace TeamSuneat.UserInterface
{
    public class UICanvasManager : XBehaviour
    {
        public Dictionary<CanvasOrderNames, CanvasOrder> canvasOrders = new Dictionary<CanvasOrderNames, CanvasOrder>();

        protected void Awake()
        {
            CanvasOrder[] canvasOrders = GetComponentsInChildren<CanvasOrder>();

            for (int i = 0; i < canvasOrders.Length; i++)
            {
                canvasOrders[i].SetSortingOrder();

                this.canvasOrders.Add(canvasOrders[i].OrderName, canvasOrders[i]);
            }
        }

        public CanvasOrder Get(CanvasOrderNames canvasOrderName)
        {
            if (canvasOrders.ContainsKey(canvasOrderName))
            {
                return canvasOrders[canvasOrderName];
            }

            return null;
        }
    }
}