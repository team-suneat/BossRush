using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class MouseButtonSpriteStrategy : IShortcutSpriteStrategy
    {
        public void ApplySprite(Image shortcutImage, ActionNames actionName)
        {
            ControllerType controllerType = TSInputManager.Instance.CurrentControllerType;
            if (controllerType != ControllerType.Joystick)
            {
                Sprite sprite = SpriteEx.LoadMouseSprite(actionName);
                if (sprite != null)
                {
                    shortcutImage.SetSprite(sprite, true);
                    Log.Progress(LogTags.UI_Shortcut, "마우스 버튼 스프라이트 적용 성공. ActionName: {0}", actionName);
                }
                else
                {
                    Log.Error("마우스 입력 아이콘을 찾을 수 없습니다. ActionName:{0}", actionName);
                }
            }
            else
            {
                // 조이스틱일 경우 기본 버튼 전략 사용
                var defaultStrategy = new DefaultButtonSpriteStrategy();
                defaultStrategy.ApplySprite(shortcutImage, actionName);
            }
        }
    }
}