using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public enum PatternStepNames
    {
        None,

        ConditionalGround = 10,

        ConditionalPlatform,

        Face = 101,

        FaceDirectional,

        ChaseGround,

        Dash = 201,

        DashWithFace,

        Jump = 211,

        DownJump,

        UpDownJump,

        JumpWithFace,

        Flash = 221,

        Blink,

        Rise = 231,

        PathMove = 241,

        Attack = 301,

        AttackWithFace,

        AttackWithCheckArea,

        Reload = 311,

        Complete = 999,
    }

}