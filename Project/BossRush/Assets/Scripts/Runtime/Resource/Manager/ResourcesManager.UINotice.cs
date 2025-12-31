using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// ResourcesManager의 UI Notice 관련 메서드들
    /// </summary>
    public partial class ResourcesManager
    {
        #region Notice

        public static UIStageTitleNotice SpawnStageTitleNotice(StageNames stageName)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Ingame);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIStageTitleNotice", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIStageTitleNotice notice = spawnedObject.GetComponent<UIStageTitleNotice>();
            if (notice != null)
            {
                notice.SetContent(stageName);
                notice.Show();
            }

            return notice;
        }

        public static UICurrencyShortageNotice SpawnCurrencyShortageNotice(CurrencyNames currencyName, Transform parent)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UICurrencyShortageNotice", parent);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UICurrencyShortageNotice notice = spawnedObject.GetComponent<UICurrencyShortageNotice>();
            if (notice != null)
            {
                notice.SetContent(currencyName);
                notice.Show();
            }

            return notice;
        }

        public static UIStatPointShortageNotice SpawnStatPointShortageNotice(Transform parent)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIStatPointShortageNotice", parent);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIStatPointShortageNotice notice = spawnedObject.GetComponent<UIStatPointShortageNotice>();
            if (notice != null)
            {
                notice.SetStringKey();
                notice.Show();
            }

            return notice;
        }

        public static UIExperienceShortageNotice SpawnExperienceShortageNotice(Transform parent)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIExperienceShortageNotice", parent);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIExperienceShortageNotice notice = spawnedObject.GetComponent<UIExperienceShortageNotice>();
            if (notice != null)
            {
                notice.SetStringKey();
                notice.Show();
            }

            return notice;
        }

        public static UICurrencyObtainedNotice SpawnCurrencyObtainedNotice(CurrencyNames currencyName, int amount, Transform parent)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UICurrencyObtainedNotice", parent);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UICurrencyObtainedNotice notice = spawnedObject.GetComponent<UICurrencyObtainedNotice>();
            if (notice != null)
            {
                notice.Setup(currencyName, amount);
                notice.Show();
            }

            return notice;
        }

        #endregion Notice
    }
}