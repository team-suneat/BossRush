using System.Collections.Generic;


namespace TeamSuneat.UserInterface
{
    /// <summary>
    /// 단축키 스프라이트 전략을 생성하는 팩토리 클래스
    /// </summary>
    public static class ShortcutSpriteStrategyFactory
    {
        private static readonly Dictionary<ActionNames, IShortcutSpriteStrategy> _strategies = new Dictionary<ActionNames, IShortcutSpriteStrategy>
        {
            { ActionNames.MoveHorizontal, new StickSpriteStrategy() },
            { ActionNames.MoveVertical, new StickSpriteStrategy() },
            { ActionNames.MoveUp, new DirectionalButtonSpriteStrategy() },
            { ActionNames.MoveDown, new DirectionalButtonSpriteStrategy() },
            { ActionNames.MoveLeft, new DirectionalButtonSpriteStrategy() },
            { ActionNames.MoveRight, new DirectionalButtonSpriteStrategy() },
            { ActionNames.UIMoveUp, new DirectionalButtonSpriteStrategy() },
            { ActionNames.UIMoveDown, new DirectionalButtonSpriteStrategy() },
            { ActionNames.UIMoveLeft, new DirectionalButtonSpriteStrategy() },
            { ActionNames.UIMoveRight, new DirectionalButtonSpriteStrategy() },
        };

        /// <summary>
        /// 액션 이름과 스틱 사용 여부에 따른 스프라이트 전략을 가져옵니다.
        /// </summary>
        /// <param name="actionName">액션 이름</param>
        /// <param name="useStickSprite">스틱 스프라이트 사용 여부</param>
        /// <returns>스프라이트 전략</returns>
        public static IShortcutSpriteStrategy GetStrategy(ActionNames actionName, bool useStickSprite)
        {
            Log.Info(LogTags.UI_Shortcut, "스프라이트 전략 팩토리 호출. ActionName: {0}, UseStickSprite: {1}", actionName, useStickSprite);

            if (_strategies.TryGetValue(actionName, out IShortcutSpriteStrategy strategy))
            {
                // 방향키의 경우 스틱 사용 여부에 따라 전략 변경
                if (strategy is DirectionalButtonSpriteStrategy directionalStrategy)
                {
                    var finalStrategy = useStickSprite ? new StickSpriteStrategy() : strategy;
                    Log.Info(LogTags.UI_Shortcut, "방향키 스프라이트 전략 선택. UseStickSprite: {0}, 전략 타입: {1}", useStickSprite, finalStrategy.GetType().Name);
                    return finalStrategy;
                }

                Log.Info(LogTags.UI_Shortcut, "스프라이트 전략 생성 성공. 전략 타입: {0}", strategy.GetType().Name);
                return strategy;
            }

            Log.Info(LogTags.UI_Shortcut, "기본 스프라이트 전략 사용");
            return new DefaultButtonSpriteStrategy();
        }
    }
}