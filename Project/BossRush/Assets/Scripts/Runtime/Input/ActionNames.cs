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
        Attack,
        Cast,
        Dash,
        Interact,

        // UI-Popup

        PopupInventory,
        PopupPause,

        // UI-Common

        UIMoveUp, UIMoveDown, UIMoveLeft, UIMoveRight,

        UISubmit,
        UISubmit2,
        UICancel,

        UIHighPrevious, UIHighNext,
        UILowPrevious, UILowNext,

        // Camera

        CameraUp,
        CameraDown,

        // User Control
        Skip,
    }
}