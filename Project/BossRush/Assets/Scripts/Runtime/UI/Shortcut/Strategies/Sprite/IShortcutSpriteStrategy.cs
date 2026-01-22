
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 단축키 스프라이트 적용을 위한 전략 패턴 인터페이스
    /// </summary>
    public interface IShortcutSpriteStrategy
    {
        /// <summary>
        /// 단축키 이미지에 스프라이트를 적용합니다.
        /// </summary>
        /// <param name="shortcutImage">단축키 이미지</param>
        /// <param name="actionName">액션 이름</param>
        void ApplySprite(Image shortcutImage, ActionNames actionName);
    }
}