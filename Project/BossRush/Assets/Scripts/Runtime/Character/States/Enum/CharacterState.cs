namespace TeamSuneat
{
    public enum CharacterState
    {
        None,

        // 이동 상태
        Idle,
        Walk,
        Jumping,
        Falling,
        Dash,
        Attack,

        // 조건 상태 (우선순위 높음)
        Dead,
        Stunned,
        ControlledMovement
    }
}