namespace TeamSuneat
{
    public enum PassiveTriggers
    {
        None,

        /// <summary> 패시브를 활성화합니다 </summary>
        Activate,

        #region 공격 (Attack)

        /// <summary> 몬스터를 공격합니다 </summary>
        AttackMonster,

        /// <summary> 몬스터를 치명타로 공격합니다 </summary>
        AttackMonsterCritical,

        /// <summary> 몬스터를 치명타 없이 공격합니다 </summary>
        AttackMonsterNonCritical,

        #endregion 공격 (Attack)

        #region 피해 (Damage)

        /// <summary> 플레이어가 공격을 회피합니다 </summary>
        PlayerEvasion,

        /// <summary> 플레이어가 피격됩니다 </summary>
        PlayerDamaged,

        /// <summary> 플레이어가 치명타로 피격됩니다 </summary>
        PlayerDamagedCritical,

        /// <summary> 플레이어가 죽음을 저항합니다. </summary>
        PlayerDeathDefiance,

        /// <summary> 몬스터가 피격됩니다 </summary>
        MonsterDamaged,

        #endregion 피해 (Damage)

        #region 처치 (Kill)

        /// <summary> 몬스터를 처치 합니다.</summary>
        MonsterKill,

        #endregion 처치 (Kill)

        #region 시전 및 실행 (Cast or Execute)

        /// <summary> 기술을 사용합니다 </summary>
        CastSkill,

        /// <summary> 기술의 재사용 대기를 시작합니다 </summary>
        StartSkillCooldown,

        /// <summary> 기술의 재사용 대기를 종료합니다 </summary>
        StopSkillCooldown,

        /// <summary> 기술의 재사용 대기가 시작되거나 종료합니다 </summary>
        RefreshCooldownSkill,

        /// <summary> 공격을 실행합니다 </summary>
        ExecuteAttack,

        /// <summary> 기술을 슬롯에 할당합니다 </summary>
        AssignSkill,

        /// <summary> 기술을 슬롯에 할당해제합니다 </summary>
        UnassignSkill,

        #endregion 시전 및 실행 (Cast or Execute)

        #region 전투 자원 (Battle Resource : Health, Shield, Resource)

        /// <summary> 플레이어가 회복합니다 </summary>
        PlayerHealed,

        /// <summary> 플레이어의 생명력이 변경됩니다 </summary>
        PlayerChangeHealth,

        /// <summary> 플레이어의 자원이 회복되거나 사용됩니다.</summary>
        PlayerChangeVitalResource,

        /// <summary> 플레이어의 자원이 사용됩니다.</summary>
        PlayerUseVitalResource,

        /// <summary> 플레이어의 보호막이 충전됩니다 </summary>
        PlayerShieldCharge,

        /// <summary> 플레이어의 보호막이 파괴됩니다 </summary>
        PlayerShieldDestroy,

        /// <summary> 몬스터가 전투 자원을 회복합니다 </summary>
        RestoreMonsterVitalResource,

        #endregion 전투 자원 (Battle Resource : Health, Shield, Resource)

        #region 플레이어 (Player)

        /// <summary> 플레이어 능력치가 변경됩니다.</summary>
        PlayerChangeStat,

        /// <summary> 플레이어 능력치가 추가됩니다.</summary>
        PlayerAddStat,

        /// <summary> 플레이어 능력치가 삭제됩니다.</summary>
        PlayerRemoveStat,

        /// <summary> 플레이어가 레벨업을 합니다.</summary>
        PlayerLevelUp,

        /// <summary> 플레이어가 오브젝트를 작동합니다.</summary>
        PlayerOperateMapObject,

        /// <summary> 플레이어가 스테이지를 이동합니다.</summary>
        PlayerMoveToStage,

        #endregion 플레이어 (Player)

        #region 버프 (Buff)

        /// <summary> 플레이어에게 버프를 추가합니다 </summary>
        AddBuffToPlayer,

        /// <summary> 플레이어에게 버프를 삭제합니다 </summary>
        RemoveBuffToPlayer,

        /// <summary> 몬스터에게 버프를 추가합니다 </summary>
        AddBuffToMonster,

        /// <summary> 몬스터에게 버프를 삭제합니다 </summary>
        RemoveBuffToMonster,

        /// <summary> 플레이어에게 상태이상을 추가합니다 </summary>
        AddStateEffectToPlayer,

        /// <summary> 플레이어에게 상태이상을 삭제합니다 </summary>
        RemoveStateEffectToPlayer,

        /// <summary> 몬스터에게 상태이상을 추가합니다 </summary>
        AddStateEffectToMonster,

        /// <summary> 몬스터에게 상태이상을 삭제합니다 </summary>
        RemoveStateEffectToMonster,

        /// <summary> 플레이어의 버프 스택이 올라갑니다 </summary>
        AddBuffStackToPlayer,

        /// <summary> 몬스터의 버프 스택이 올라갑니다 </summary>
        AddBuffStackToMonster,

        #endregion 버프 (Buff)

        //─────────────────────────── Item ───────────────────────────


        #region 물약 (Potion)

        /// <summary> 포션을 획득합니다 </summary>
        TakePotion,

        /// <summary> 포션을 사용합니다 </summary>
        UsePotion,

        #endregion 물약 (Potion)

        #region 요리 (Food)

        /// <summary> 요리를 먹습니다 </summary>
        EatFood,

        #endregion 요리 (Food)

        #region 유물 (Relic)

        /// <summary> 구매 품목을 초기화 합니다 </summary>
        RerollBuyItem,

        /// <summary>  유물 아이템을 버립니다 </summary>
        ThrowRelic,

        /// <summary>  상호작용을 통해 유물을 획득합니다 </summary>
        TakeRelicOperate,

        /// <summary> 유물 아이템을 파괴합니다 </summary>
        DestroyRelic,

        #endregion 유물 (Relic)

        #region 재화 (Currency)

        /// <summary> 재화를 사용합니다 </summary>
        UseCurrency,

        /// <summary> 재화가 갱신됩니다 </summary>
        CurrencyChanged,

        #endregion 재화 (Currency)
    }
}