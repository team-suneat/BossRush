namespace TeamSuneat
{
    public enum VFXParentTypes
    {
        None,

        /// <summary> 캐릭터의 머리 위치 </summary>
        Head,

        /// <summary> 캐릭터의 발 위치 </summary>
        Foot,

        /// <summary> 캐릭터의 몸통 위치 </summary>
        Body,

        /// <summary> 캐릭터의 중심 (전체 위치) </summary>
        Character,
    }

    public enum VFXPositionTypes
    {
        None,

        /// <summary> 캐릭터의 머리 위치 </summary>
        Head,

        /// <summary> 캐릭터의 바닥 위치 : 그라운드일 때만 적용됨 </summary>
        Grounded,

        /// <summary> 캐릭터의 실드 위치 </summary>
        Shield,

        /// <summary> 캐릭터의 발 위치 </summary>
        Foot,

        /// <summary> 캐릭터의 몸통 위치 </summary>
        Body,

        /// <summary> Flip 방향을 따른 캐릭터 위치 </summary>
        Character,
    }

    public enum VFXInstantDamageType
    {
        None,

        /// <summary> 예리함 </summary>
        Sharp,

        /// <summary> 둔기류 </summary>
        Blunt,
    }
}