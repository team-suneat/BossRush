namespace TeamSuneat
{
    public enum ActionNames
    {
        None,

        // Character
        MoveHorizontal,
        MoveVertical,
        MoveUp, MoveDown, MoveLeft, MoveRight,

        Jump,
        Attack, SubAttack,
        Cast1, Cast2, Cast3, Cast4,

        Potion1,
        Interact, OrderInteract,

        // UI-Popup

        PopupSkill,
        PopupStatus,
        PopupInventory,
        PopupItem,
        PopupPause,

        // UI-Common

        UIMoveUp, UIMoveDown, UIMoveLeft, UIMoveRight,
        UICursorHorizontal, UICursorHorizontal2,
        UICursorVertical, UICursorVertical2,

        UISubmit,
        UISubmit2,
        UISubmitClick,
        UICancel,

        UIHighPrevious, UIHighNext,
        UILowPrevious, UILowNext,
        UIDepthUp, UIDepthDown,

        // User Control

        Equip,      // 장착
        Use,        // 사용
        Discard,    // 버리기
        Range,      // 범위 지정
        Comparison, // 비교
        Sorting,    // 정렬
        OpenBook,   // 도감 열기
        Synergy,    // 시너지
        KeyBinding, // 키 바인딩
        Skip,       // 스킵
        WorldDifficulty, // 월드 난이도

        CameraUp, CameraDown,
    }
}