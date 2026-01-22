using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class StickSpriteStrategy : IShortcutSpriteStrategy
    {
        public void ApplySprite(Image shortcutImage, ActionNames actionName)
        {
            Sprite sprite = SpriteEx.LoadStickSprite(actionName);
            if (sprite != null)
            {
                shortcutImage.SetSprite(sprite, true);
                Log.Progress(LogTags.UI_Shortcut, "스틱 스프라이트 적용 성공. ActionName: {0}", actionName);
            }
            else
            {
                Log.Error("입력 아이콘을 찾을 수 없습니다. ActionName: {0}", actionName);
            }
        }
    }
}