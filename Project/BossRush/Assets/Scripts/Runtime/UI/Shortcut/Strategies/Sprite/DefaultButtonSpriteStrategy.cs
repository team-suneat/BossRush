using Rewired;

using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 기본 버튼 스프라이트 전략
    /// </summary>
    public class DefaultButtonSpriteStrategy : IShortcutSpriteStrategy
    {
        public void ApplySprite(Image shortcutImage, ActionNames actionName)
        {
            string keyName = TSInputManager.Instance.GetKey(actionName);

#if UNITY_SWITCH
            if(string.IsNullOrEmpty(keyName))
            {
                shortcutImage.ResetSprite();
                Log.Progress(LogTags.UI_Shortcut, "기본 버튼 스프라이트 - 키가 없거나 스틱 키. ActionName: {0}, Key: {1}", actionName, keyName);
                return;
            }
            if(keyName.Contains("Left Stick") || keyName.Contains("Right Stick"))
            {
                keyName += "Button";
                keyName = keyName.Replace(" ", string.Empty);
                keyName = keyName.Trim();
            }
#endif

            if (string.IsNullOrEmpty(keyName) || (keyName.Contains("Left Stick") || keyName.Contains("Right Stick")) && !keyName.Contains("Button"))
            {
                shortcutImage.ResetSprite();
                Log.Progress(LogTags.UI_Shortcut, "기본 버튼 스프라이트 - 키가 없거나 스틱 키. ActionName: {0}, Key: {1}", actionName, keyName);
                return;
            }

            ControllerType controllerType = TSInputManager.Instance.CurrentControllerType;
            if (controllerType == ControllerType.Mouse && !keyName.Contains("Mouse"))
            {
                controllerType = ControllerType.Keyboard;
            }

            Sprite sprite = controllerType.LoadSprite(keyName);
            if (sprite != null)
            {
                shortcutImage.SetSprite(sprite, true);
                Log.Progress(LogTags.UI_Shortcut, "기본 버튼 스프라이트 적용 성공. ActionName: {0}", actionName);
            }
            else
            {
                Log.Error("입력 아이콘을 찾을 수 없습니다. actionName:{0}, key:{1}", actionName, keyName);
            }
        }
    }
}