namespace TeamSuneat
{
    public enum CharacterStyles
    {
        None,

        //Normal

        /// <summary> 암살자 </summary>
        Assassin,

        /// <summary> 전사 </summary>
        Fighter,

        /// <summary> 마법사 </summary>
        Mage,

        /// <summary> 원거리 딜러 </summary>
        Markman,

        /// <summary> 서포터 </summary>
        Supports,

        /// <summary> 탱커 </summary>
        Tank,

        /// <summary> 거인 </summary>
        Giant,

        /// <summary> 비행 </summary>
        Flying,

        /// <summary> 비행 마법사 </summary>
        FlyingMage,

        // Elite
        EliteAssassin,
        EliteFighter,
        EliteMage,
        EliteMarkman,
        EliteSupports,
        EliteTank,
        EliteGiant,
        EliteFlying,
        EliteFlyingMage,

        // Summon
        SummonAssassin,
        SummonFighter,
        SummonMage,
        SummonMarkman,
        SummonSupports,
        SummonTank,
        SummonGiant,
        SummonFlying,
        SummonFlyingMage,

        // Boss
        BossAssassin,
        BossFighter,
        BossMage,
        BossMarkman,
        BossSupports,
        BossTank,
        BossGiant,
        BossFlying,
        BossFlyingMage,
    }

    public static class CharacterStyleChecker
    {
        public static bool IsNormal(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Assassin:
                case CharacterStyles.Fighter:
                case CharacterStyles.Mage:
                case CharacterStyles.Markman:
                case CharacterStyles.Supports:

                case CharacterStyles.EliteAssassin:
                case CharacterStyles.EliteFighter:
                case CharacterStyles.EliteMage:
                case CharacterStyles.EliteMarkman:
                case CharacterStyles.EliteSupports:

                case CharacterStyles.SummonAssassin:
                case CharacterStyles.SummonFighter:
                case CharacterStyles.SummonMage:
                case CharacterStyles.SummonMarkman:
                case CharacterStyles.SummonSupports:

                case CharacterStyles.BossAssassin:
                case CharacterStyles.BossFighter:
                case CharacterStyles.BossMage:
                case CharacterStyles.BossMarkman:
                case CharacterStyles.BossSupports:
                    return true;
            }
            return false;
        }

        public static bool IsMelee(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Assassin:
                case CharacterStyles.Fighter:

                case CharacterStyles.EliteAssassin:
                case CharacterStyles.EliteFighter:

                case CharacterStyles.SummonAssassin:
                case CharacterStyles.SummonFighter:

                case CharacterStyles.BossAssassin:
                case CharacterStyles.BossFighter:
                    return true;
            }
            return false;
        }

        public static bool IsRange(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Mage:
                case CharacterStyles.Markman:
                case CharacterStyles.Supports:

                case CharacterStyles.EliteMage:
                case CharacterStyles.EliteMarkman:
                case CharacterStyles.EliteSupports:

                case CharacterStyles.SummonMage:
                case CharacterStyles.SummonMarkman:
                case CharacterStyles.SummonSupports:

                case CharacterStyles.BossMage:
                case CharacterStyles.BossMarkman:
                case CharacterStyles.BossSupports:
                    return true;
            }
            return false;
        }

        public static bool IsTank(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Tank:
                case CharacterStyles.EliteTank:
                case CharacterStyles.SummonTank:
                case CharacterStyles.BossTank:
                    return true;
            }
            return false;
        }

        public static bool IsGiant(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Giant:
                case CharacterStyles.EliteGiant:
                case CharacterStyles.SummonGiant:
                case CharacterStyles.BossGiant:
                    return true;
            }
            return false;
        }

        public static bool IsFlying(this CharacterStyles style)
        {
            switch (style)
            {
                case CharacterStyles.Flying:
                case CharacterStyles.FlyingMage:
                case CharacterStyles.EliteFlying:
                case CharacterStyles.EliteFlyingMage:
                case CharacterStyles.SummonFlying:
                case CharacterStyles.SummonFlyingMage:
                case CharacterStyles.BossFlying:
                case CharacterStyles.BossFlyingMage:
                    return true;
            }
            return false;
        }
    }
}