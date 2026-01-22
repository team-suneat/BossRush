using Rewired;

using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 방향키 버튼 스프라이트 전략
    /// </summary>
    public class DirectionalButtonSpriteStrategy : IShortcutSpriteStrategy
    {
        public void ApplySprite(Image shortcutImage, ActionNames actionName)
        {
            Log.Progress(LogTags.UI_Shortcut, "방향키 버튼 스프라이트 적용. ActionName: {0}", actionName);

            if (TSInputManager.Instance.CurrentControllerType == ControllerType.Joystick)
            {
                switch (actionName)
                {
                    case ActionNames.MoveUp:
                        ApplySprite(shortcutImage, actionName, "leftstickup");
                        return;

                    case ActionNames.MoveDown:
                        ApplySprite(shortcutImage, actionName, "leftstickdown");
                        return;

                    case ActionNames.MoveLeft:
                        ApplySprite(shortcutImage, actionName, "leftstickleft");
                        return;

                    case ActionNames.MoveRight:
                        ApplySprite(shortcutImage, actionName, "leftstickright");
                        return;

                    case ActionNames.UIMoveUp:
                        ApplySprite(shortcutImage, actionName, "d-padup");
                        return;

                    case ActionNames.UIMoveDown:
                        ApplySprite(shortcutImage, actionName, "d-paddown");
                        return;

                    case ActionNames.UIMoveLeft:
                        ApplySprite(shortcutImage, actionName, "d-padleft");
                        return;

                    case ActionNames.UIMoveRight:
                        ApplySprite(shortcutImage, actionName, "d-padright");
                        return;
                }
            }

            // 기본 버튼 전략과 동일하지만 스틱 사용 여부에 따라 변경될 수 있음
            DefaultButtonSpriteStrategy defaultStrategy = new DefaultButtonSpriteStrategy();
            defaultStrategy.ApplySprite(shortcutImage, actionName);
        }

        private void ApplySprite(Image shortcutImage, ActionNames actionName, string keyName)
        {
            if (string.IsNullOrEmpty(keyName))
            {
                shortcutImage.ResetSprite();
                Log.Progress(LogTags.UI_Shortcut, "이동 버튼 스프라이트 - 키가 없습니다. ActionName: {0}, Key: {1}", actionName, keyName);
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