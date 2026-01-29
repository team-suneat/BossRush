using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public enum PatternStepNames
    {
        None,
        ConditionalGround,
        ConditionalPlatform,

        Face,
        FaceDirectional,
        FaceToPositionGroup,

        ChaseGround,

        Dash,
        DashWithFace,

        JumpToTarget,
        JumpToPositionGroup,

        Flash,
        Blink,

        Rise,
        PathMove,

        Attack,
        AttackWithFace,
        AttackWithCheckArea,

        Reload,

        Complete = 999,
    }
}