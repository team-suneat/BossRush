using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// ResourcesManager의 UI 관련 메서드들
    /// </summary>
    public partial class ResourcesManager
    {
        #region Floaty Text

        public static UIFloatyText SpawnFloatyText(string content, bool isFacingRight, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, UIFloatyMoveNames.Content);
                component.SetFacingRight(isFacingRight);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnFloatyText(string content, UIFloatyMoveNames moveName, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, moveName);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnCurrencyFloatyText(CurrencyNames currencyName, int amount, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            if (currencyName == CurrencyNames.None)
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                string format = currencyName.GetFormatString();
                string content = string.Format(format, amount);

                UIFloatyMoveNames moveName = UIFloatyMoveNames.Content;
                if (currencyName == CurrencyNames.Gold)
                {
                    moveName = UIFloatyMoveNames.Content;
                }
                else if (currencyName == CurrencyNames.Diamond)
                {
                    moveName = UIFloatyMoveNames.Content;
                }
                else if (currencyName == CurrencyNames.Emerald)
                {
                    moveName = UIFloatyMoveNames.Content;
                }

                component.Setup(content, moveName);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnUIFloatyText(string content, UIFloatyMoveNames moveName)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = false;
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, moveName);
            }

            component.rectTransform.anchoredPosition3D = new Vector3(GameDefine.DEFAULT_SCREEN_WIDTH * 0.5f, GameDefine.DEFAULT_SCREEN_HEIGHT * 0.5f);

            return component;
        }

        #endregion Floaty Text

        public static UIEnemyGauge SpawnEnemyGauge(Vital vital)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIEnemyGauge", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            return spawnedObject.GetComponent<UIEnemyGauge>();
        }

        public static UIPlayerGauge SpawnPlayerGauge(Vital vital)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIPlayerGauge", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            return spawnedObject.GetComponent<UIPlayerGauge>();
        }

        internal static UIDetails SpawnDetails(UIDetailsNames detailsName, Transform parent)
        {
            return null;
        }

        internal static void SpawnDropPotion(Vector3 position)
        {
        }

        internal static UIDetails SpawnIngameDetails(UIDetailsNames detailsName, Transform toFollow)
        {
            return null;
        }

        public static UIPopup SpawnPopup(UIPopupNames popupName)
        {
            CanvasOrder popupCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.Popup);
            string prefabName = "UI" + popupName.ToString() + "Popup";
            GameObject spawnedObject = SpawnPrefab(prefabName, popupCanvas.transform);

            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();
            return spawnedObject.GetComponent<UIPopup>();
        }
    }
}