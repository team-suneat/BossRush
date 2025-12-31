namespace TeamSuneat
{
    public enum SoliloquyTypes
    {
        None,

        /// <summary> 마나가 부족합니다 </summary>
        LackOfMana,

        /// <summary> 무기를 장비하세요. </summary>
        EquipWeapon,

        /// <summary> 가방이 가득 찼습니다. </summary>
        InventoryBagFull,

        /// <summary> 아직 사용할 수 없습니다. </summary>
        CanNotUsedYet,

        /// <summary> 장착한 장비는 분해할 수 없습니다. </summary>
        NotSalvageEquippedEquipment,

        /// <summary> 장비만 분해할 수 있습니다. </summary>
        OnlySalvageEquipment,

        /// <summary> 전설 장비만 분해할 수 있습니다. </summary>
        OnlySalvageLegendary,

        /// <summary> 사용할 재화 {0}가 부족합니다. </summary>
        LackOfCurrency,

        /// <summary> 장비에만 룬워드를 부여 할 수 있습니다. </summary>
        NotUseRunewordEquipment,

        /// <summary> 레어, 매직 등급 아이템만 부여할 수 있습니다.</summary>
        OnlytRunewordGrade,

        /// <summary> 부여할 수 있는 장비 종류가 아닙니다. </summary>
        NotUseRunewordEuipmentType,

        /// <summary> 일반 등급의 장비는 각인할 수 없습니다. </summary>
        LockGradeEngraving,

        /// <summary> 일반 등급의 장비는 강화할 수 없습니다. </summary>
        LockGradeEnhance,

        /// <summary> 일반 등급의 장비는 초월할 수 없습니다. </summary>
        LockGradeTranscend,

        /// <summary> 생명력이 가득 찬 상태에선 사용 할 수 없습니다. </summary>
        LackHealingPotion,

        /// <summary> 물약을 더 가질 수 없습니다. </summary>
        FullPotion,

        /// <summary> 사용할 수 있는 물약이 없습니다. </summary>
        LackOfPotion,

        /// <summary> 루비 열쇠가 필요합니다.</summary>
        RequiredKey,

        /// <summary> 아직 미구현 캐릭터 입니다.</summary>
        NotVersionCharacter,

        /// <summary> 오직 다음 지역으로만 이동할 수 있습니다.</summary>
        OnlyMoveToNextStage,

        /// <summary> 아직 지역 정보를 확인 할 수 없습니다.</summary>
        NotAreaInformation,

        /// <summary> 이미 획득한 아이템입니다.</summary>
        TakedItem,

        /// <summary> 생명의 샘에서 캐릭터를 선택 하세요.</summary>
        SelectCharacter,

        /// <summary> 신비한 힘에 의해 금화를 사용할 수 없습니다.</summary>
        LockGold,

        /// <summary> 아직 치트를 사용할 수 없습니다.</summary>
        LockCheat,

        /// <summary> 더 이상 새로고침을 할 수 없습니다.</summary>
        LockReroll,

        /// <summary> 해당 캐릭터로는 이 장비를 도박해 볼 수 없습니다.</summary>
        LockGambleByCharacter,

        /// <summary> {0} 효과가 발동합니다. </summary>
        ExecuteEffect,

        /// <summary> {0} 효과가 발동합니다. 재사용 대기({1}초)가 시작됩니다. </summary>
        ExecuteEffectWithCooldown,

        /// <summary> {0} 효과가 중첩됩니다. </summary>
        StackEffect,

        /// <summary> {0} 효과가 사라집니다. </summary>
        UnstackEffect,

        /// <summary> {0} 의 재사용 대기시간({1}초)이 종료되었습니다.조건을 만족하면 자동으로 발동됩니다. </summary>
        PassiveCooldownComplete,

        /// <summary> {0}을 사용합니다. </summary>
        UseItem,

        /// <summary> 양손 무기를 착용 중입니다. 보조 무기에 장비를 착용할 수 없습니다. </summary>
        FailedToEquipOffhand,

        /// <summary> 해당 캐릭터로는 이 장비를 착용할 수 없습니다. </summary>
        FailedToEquipByCharacter,

        /// <summary> 획득한 적 없는 전설 장비를 제작할 수 없습니다. </summary>
        FailedToCraftLegendary,

        /// <summary> 전설 장비 [{0}] 도안을 획득했습니다. 가하즈에게 가져가면 이 전설 장비를 제작할 수 있습니다. </summary>
        TakeLegendaryBlueprint,

        /// <summary> 가방에 있는 아이템이 아니라면 팔거나 버릴 수 없습니다. </summary>
        CanNotSellOrDiscard1,

        /// <summary> 장비가 아닌 아이템은 팔거나 버릴 수 없습니다. </summary>
        CanNotSellOrDiscard2,

        /// <summary> 죽음을 저항합니다. 죽음에 이르는 피해를 견디고 생명력을 회복합니다. </summary>
        DeathDefiance,

        /// <summary> 재입고 비용이 무시되었습니다. </summary>
        IgnoreRerollCost,

        /// <summary> 몬스터 웨이브 진행 중에는 스킬 트리를 열 수 없습니다. </summary>
        BlockOpenSkillPopupWhileWave,

        /// <summary> 해금한 균열 보석 슬롯에만 착용할 수 있습니다. </summary>
        RiftGemLockedSlot,

        /// <summary> 해당 보석은 신화 전용 슬롯에서만 착용할 수 있습니다. </summary>
        RiftGemMythicSlot,

        /// <summary> 전설 등급 균열 보석에서만 전설 장비의 효과를 부여를 할 수 있습니다. </summary>
        InjectRiftGemOnlyLegendary,
    }
}