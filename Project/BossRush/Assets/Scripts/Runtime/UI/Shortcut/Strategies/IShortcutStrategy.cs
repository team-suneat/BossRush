using Rewired;

using UnityEngine;

namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 단축키 설정을 위한 전략 패턴 인터페이스
    /// </summary>
    public interface IShortcutStrategy
    {
        /// <summary>
        /// 단축키 설정을 적용합니다.
        /// </summary>
        /// <param name="shortcutElements">단축키 요소 배열</param>
        /// <param name="activateCount">활성화된 단축키 개수 (참조로 전달)</param>
        void ApplyShortcuts(UIShortcutElement[] shortcutElements, ref int activateCount);

        /// <summary>
        /// 컨트롤러 타입에 따른 단축키 설정을 가져옵니다.
        /// </summary>
        /// <param name="controllerType">컨트롤러 타입</param>
        /// <returns>단축키 설정 데이터</returns>
        ShortcutData GetShortcutData(ControllerType controllerType);
    }

    /// <summary>
    /// 단축키 설정 데이터 구조체
    /// </summary>
    public readonly struct ShortcutData
    {
        public readonly ActionNames[] Actions;
        public readonly string[] TextKeys;
        public readonly Vector3? PositionOffset;

        public ShortcutData(ActionNames[] actions, string[] textKeys, Vector3? positionOffset = null)
        {
            Actions = actions;
            TextKeys = textKeys;
            PositionOffset = positionOffset;
        }
    }
}