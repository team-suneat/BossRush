namespace TeamSuneat
{
    public enum CharacterConditions
    {
        Normal,

        /// <summary> 조작된 움직임 </summary>
        ControlledMovement,

        /// <summary> 얼어 붙음 </summary>
        Frozen,

        /// <summary> 일시 중지 </summary>
        Paused,

        /// <summary> 죽음 </summary>
        Dead,

        /// <summary> 기절 </summary>
        Stunned,

        /// <summary> 당겨지는 </summary>
        Grabbed,

        /// <summary> 속박 </summary>
        Snared,
    }

    public enum MovementStates
    {
        Null,

        /// <summary> 유휴 </summary>
        Idle,

        /// <summary> 걷는 </summary>
        Walking,

        /// <summary> 떨어지는 </summary>
        Falling,

        /// <summary> 달리기 </summary>
        Running,

        /// <summary> 웅크리는 </summary>
        Crouching,

        /// <summary> 기어다니는 </summary>
        Crawling,

        /// <summary> 돌진하는 </summary>
        Dashing,

        /// <summary> 올려다보는 </summary>
        LookingUp,

        /// <summary> 벽을 타는 </summary>
        WallClinging,

        /// <summary> 다이빙하는 </summary>
        Diving,

        /// <summary> 그립하는 </summary>
        Gripping,

        /// <summary> 매달린 </summary>
        Dangling,

        /// <summary> 점프하는 </summary>
        Jumping,

        /// <summary> 미는 </summary>
        Pushing,

        /// <summary> 더블점프하는 </summary>
        DoubleJumping,

        /// <summary> 벽점프하는 </summary>
        WallJumping,

        /// <summary> 사다리 타는 </summary>
        LadderClimbing,

        /// <summary> 사다리 오르는 </summary>
        LadderClimbed,

        /// <summary> 수영 유휴 </summary>
        SwimmingIdle,

        /// <summary> 날아다니는 </summary>
        Flying,

        /// <summary> 경로를 따르는 </summary>
        FollowingPath,

        /// <summary> 난간에 매달린 </summary>
        LedgeHanging,

        /// <summary> 난간을 오르는 </summary>
        LedgeClimbing,

        /// <summary> 공격하는 </summary>
        Attack,

        /// <summary> 포션을 소모하는 </summary>
        ConsumingPotion,
    }

}