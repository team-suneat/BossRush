namespace TeamSuneat
{
    public enum GlobalEventType
    {
        NONE,

        #region 플레이어 캐릭터 PLAYER CHARACTER

        ///<summary> 플레이어 캐릭터를 추가합니다 </summary>
        PLAYER_CHARACTER_ADDED,

        ///<summary> 플레이어 캐릭터를 삭제합니다 </summary>
        PLAYER_CHARACTER_REMOVED,

        ///<summary> 플레이어 캐릭터를 선택합니다 </summary>
        PLAYER_CHARACTER_SELECTED,

        ///<summary> 플레이어 캐릭터를 선택 해제합니다 </summary>
        PLAYER_CHARACTER_DESELECTED,

        ///<summary> 플레이어 캐릭터가 생성됩니다 </summary>
        PLAYER_CHARACTER_SPAWNED,

        ///<summary> 플레이어 캐릭터가 전투 준비를 완료합니다 </summary>
        PLAYER_CHARACTER_BATTLE_READY,

        ///<summary> 플레이어 캐릭터가 삭제됩니다 </summary>
        PLAYER_CHARACTER_DESPAWNED,

        ///<summary> 플레이어 캐릭터가 사망합니다 </summary>
        PLAYER_CHARACTER_DEATH,

        ///<summary> 플레이어 캐릭터가 적을 공격합니다 </summary>
        PLAYER_CHARACTER_ATTACK_MONSTER,

        ///<summary> 플레이어 캐릭터가 발사체를 공격합니다 </summary>
        PLAYER_CHARACTER_ATTACK_PROJECTILE,

        ///<summary> 플레이어 캐릭터가 생명력이 가득 찬 몬스터를 공격합니다. </summary>
        PLAYER_CHARACTER_ATTACK_MONSTER_FULL_LIFE_TARGET,

        ///<summary> 플레이어 캐릭터가 적을 공격할 때 치명타가 발생합니다. </summary>
        PLAYER_CHARACTER_ATTACK_MONSTER_CRITICAL,

        ///<summary> 플레이어 캐릭터가 적을 공격할 때 회심의 일격이 발생합니다. </summary>
        PLAYER_CHARACTER_ATTACK_MONSTER_DEVASTATING_STRIKE,

        //

        ///<summary> 플레이어 캐릭터 피해를 입습니다. </summary>
        PLAYER_CHARACTER_DAMAGED,

        ///<summary> 플레이어 캐릭터가 죽음을 저항합니다. </summary>
        PLAYER_CHARACTER_DEATH_DEFIANCE,

        ///<summary> 플레이어 캐릭터가 생명력이 회복되었습니다.</summary>
        PLAYER_CHARACTER_HEAL,

        ///<summary> 플레이어 캐릭터가 생명력이 변경되었습니다 </summary>
        PLAYER_CHARACTER_REFRESH_LIFE,

        ///<summary> 플레이어 캐릭터가 보호막이 충전되었습니다.</summary>
        PLAYER_CHARACTER_SHIELD_CHARGE,

        ///<summary> 플레이어 캐릭터가 보호막이 파괴되었습니다.</summary>
        PLAYER_CHARACTER_SHIELD_DESTROY,

        //

        ///<summary> 플레이어 캐릭터가 회피합니다.</summary>
        PLAYER_CHARACTER_DODGE,

        ///<summary> 플레이어 캐릭터가 적을 죽입니다 </summary>
        PLAYER_CHARACTER_KILL_MONSTER,

        ///<summary> 플레이어 캐릭터가 기술을 시전합니다 </summary>
        PLAYER_CHARACTER_CAST_SKILL,

        ///<summary> 플레이어 캐릭터의 기술 재사용 대기가 시작됩니다. </summary>
        PLAYER_CHARACTER_START_SKILL_COOLDOWN,

        ///<summary> 플레이어 캐릭터의 기술 재사용 대기가 종료됩니다. </summary>
        PLAYER_CHARACTER_STOP_SKILL_COOLDOWN,

        //

        ///<summary> 플레이어 캐릭터가 공격에 성공합니다 </summary>
        PLAYER_CHARACTER_EXECUTE_ATTACK_SUCCESS,

        ///<summary> 플레이어 캐릭터가 공격에 실패합니다 </summary>
        PLAYER_CHARACTER_EXECUTE_ATTACK_FAILED,

        //

        ///<summary> 플레이어 캐릭터에게 버프를 추가합니다 </summary>
        PLAYER_CHARACTER_ADD_BUFF,

        ///<summary> 플레이어 캐릭터에게 버프를 삭제합니다 </summary>
        PLAYER_CHARACTER_REMOVE_BUFF,

        ///<summary> 플레이어 캐릭터에게 상태이상을 추가합니다 </summary>
        PLAYER_CHARACTER_ADD_STATE_EFFECT,

        ///<summary> 플레이어 캐릭터에게 상태이상을 삭제합니다 </summary>
        PLAYER_CHARACTER_REMOVE_STATE_EFFECT,

        ///<summary> 몬스터 캐릭터에게 상태이상을 추가합니다 </summary>
        MONSTER_CHARACTER_ADD_STATE_EFFECT,

        ///<summary> 몬스터 캐릭터에게 상태이상을 삭제합니다 </summary>
        MONSTER_CHARACTER_REMOVE_STATE_EFFECT,

        ///<summary> 플레이어 캐릭터에게 버프 스택을 추가합니다 </summary>
        PLAYER_CHARACTER_ADD_BUFF_STACK,

        //

        ///<summary> 플레이어 캐릭터의 패시브를 실행합니다 </summary>
        PLAYER_CHARACTER_EXECUTE_PASSIVE,

        //

        ///<summary> 플레이어 캐릭터가 상호작용 오브젝트를 작동합니다 </summary>
        PLAYER_CHARACTER_OPERATE_MAP_OBJECT,

        ///<summary> 플레이어의 생명력이 50% 이상입니다. </summary>
        PLAYER_CHARACTER_MORE_HALF_LIFE,

        ///<summary> 플레이어의 생명력이 50% 이하입니다. </summary>
        PLAYER_CHARACTER_LESS_HALF_LIFE,

        ///<summary> 플레이어 캐릭터가 자원이 변경됩니다. </summary>
        PLAYER_CHARACTER_CHANGE_VITAL_RESOURCE,

        ///<summary> 플레이어 캐릭터가 자원이 사용됩니다. </summary>
        PLAYER_CHARACTER_USE_VITAL_RESOURCE,

        ///<summary> 플레이어 캐릭터의 능력치가 추가됩니다. </summary>
        PLAYER_CHARACTER_ADD_STAT,

        ///<summary> 플레이어 캐릭터의 능력치가 삭제됩니다. </summary>
        PLAYER_CHARACTER_REMOVE_STAT,

        #endregion 플레이어 캐릭터 PLAYER CHARACTER

        #region 몬스터 캐릭터 MONSTER CHARACTER

        ///<summary> 몬스터 캐릭터가 생성됩니다 </summary>
        MONSTER_CHARACTER_SPAWNED,

        ///<summary> 보스 몬스터 캐릭터 피해를 입습니다. </summary>
        BOSS_CHARACTER_BATTLE_READY,

        ///<summary> 몬스터 캐릭터가 공격을 시전합니다. </summary>
        MONSTER_CHARACTER_ATTACK_CAST,

        ///<summary> 몬스터 캐릭터에게 버프를 추가하거나, 중첩합니다. </summary>
        MONSTER_CHARACTER_ADD_BUFF,

        ///<summary> 몬스터 캐릭터에게 버프를 삭제합니다 </summary>
        MONSTER_CHARACTER_REMOVE_BUFF,

        ///<summary> 몬스터 캐릭터에게 버프 스택을 추가합니다 </summary>
        MONSTER_CHARACTER_ADD_BUFF_STACK,

        ///<summary> 몬스터 캐릭터에게 버프의 공격이 적용됩니다. </summary>
        MONSTER_CHARACTER_APPLY_ATTACK_BUFF,

        ///<summary> 몬스터 캐릭터 피해를 입습니다. </summary>
        MONSTER_CHARACTER_DAMAGED,

        ///<summary> 보스 몬스터 캐릭터 피해를 입습니다. </summary>
        BOSS_CHARACTER_DAMAGED,

        ///<summary> 몬스터 캐릭터가 생명력을 회복하였습니다. </summary>
        MONSTER_CHARACTER_HEAL,

        ///<summary> 몬스터 캐릭터가 생명력이 변경되었습니다 </summary>
        MONSTER_CHARACTER_REFRESH_LIFE,

        ///<summary> 몬스터 캐릭터가 자원을 회복합니다 </summary>
        MONSTER_CHARACTER_RESTORE_VITAL_RESOURCE,

        ///<summary> 몬스터 캐릭터가 사망합니다. </summary>
        MONSTER_CHARACTER_DEATH,

        ///<summary> 보스 캐릭터가 사망합니다. </summary>
        BOSS_CHARACTER_DEATH,

        #endregion 몬스터 캐릭터 MONSTER CHARACTER

        #region 발사체 PROJECTILE

        ///<summary> 발사체가 생성됩니다 </summary>
        PROJECTILE_SPAWND,

        ///<summary> 발사체가 삭제됩니다 </summary>
        PROJECTILE_DESPAWND,

        #endregion 발사체 PROJECTILE

        #region 게임 데이터 GAME DATA

        ///<summary> 기술을 추가합니다 </summary>
        GAME_DATA_ADD_CHARACTER_SKILL,

        ///<summary> 기술을 삭제합니다 </summary>
        GAME_DATA_REMOVE_CHARACTER_SKILL,

        ///<summary> 기술의 레벨이 변경됩니다 </summary>
        GAME_DATA_CHARACTER_SKILL_LEVEL_REFRESH,

        ///<summary> 강화 능력치 레벨이 변경됩니다 </summary>
        GAME_DATA_CHARACTER_ENHANCEMENT_LEVEL_CHANGED,

        ///<summary> 성장 능력치 레벨이 변경됩니다 </summary>
        GAME_DATA_CHARACTER_GROWTH_LEVEL_CHANGED,

        ///<summary> 성장 능력치 포인트가 변경됩니다 </summary>
        GAME_DATA_CHARACTER_GROWTH_STAT_POINT_CHANGED,

        ///<summary> 기술을 전부 초기화합니다. </summary>
        GAME_DATA_CHARACTER_SKILL_LEVEL_RESET,

        ///<summary> 기술을 할당합니다 </summary>
        GAME_DATA_ASSIGN_CHARACTER_SKILL,

        ///<summary> 기술을 할당해제합니다 </summary>
        GAME_DATA_UNASSIGN_CHARACTER_SKILL,

        ///<summary> 기술 허용을 등록합니다 </summary>
        GAME_DATA_REGISTER_ALLOWED_SKILL,

        ///<summary> 기술 허용을 등록해제합니다 </summary>
        GAME_DATA_UNREGISTER_ALLOWED_SKILL,

        ///<summary> 캐릭터 레벨을 변경합니다 </summary>
        GAME_DATA_CHARACTER_LEVEL_CHANGED,

        ///<summary> 캐릭터가 경험치를 획득합니다 </summary>
        GAME_DATA_CHARACTER_ADD_EXPERIENCE,

        ///<summary> 플레이어 캐릭터가 전직합니다 </summary>
        GAME_DATA_PLAYER_CHARACTER_PROMOTION,

        ///<summary> 플레이어 캐릭터를 변경합니다 </summary>
        GAME_DATA_PLAYER_CHARACTER_CHANGED,

        ///<summary> 캐릭터가 아이템을 장착합니다 </summary>
        GAME_DATA_PLAYER_EQUIP_ITEM,

        ///<summary> 캐릭터가 아이템을 장착해제합니다 </summary>
        GAME_DATA_PLAYER_UNEQUIP_ITEM,

        ///<summary> 기술 포인트를 획득합니다. </summary>
        GAME_DATA_PLAYER_ADD_SKILL_REMAININGPOINT,

        ///<summary> 기술 포인트를 사용합니다. </summary>
        GAME_DATA_PLAYER_USE_SKILL_REMAININGPOINT,

        ///<summary> 스테이지를 설정합니다. </summary>
        GAME_DATA_STAGE_SET,

        /// <summary> 게임 데이터를 저장합니다. </summary>
        SAVE_GAME_DATA,


        #endregion 게임 데이터 GAME DATA

        #region 타임라인 TIMELINE

        ///<summary> 타임라인이 재생됩니다. </summary>
        TIMELINE_PLAY,

        ///<summary> 타임라인이 완료되었습니다. </summary>
        TIMELINE_COMPLETED,

        #endregion 타임라인 TIMELINE

        #region 재화 CURRENCY

        ///<summary> 재화를 획득합니다. </summary>
        CURRENCY_EARNED,

        ///<summary> 재화를 지불합니다. </summary>
        CURRENCY_PAYED,

        ///<summary> 재화가 부족합니다. </summary>
        CURRENCY_SHORTAGE,

        #endregion 재화 CURRENCY

        #region 성장 GROWTH

        ///<summary> 능력치 포인트가 부족합니다. </summary>
        STAT_POINT_SHORTAGE,

        #endregion 성장 GROWTH

        #region 레벨 LEVEL

        ///<summary> 경험치가 부족합니다. </summary>
        EXPERIENCE_SHORTAGE,

        #endregion 레벨 LEVEL

        #region 설정 SETTING

        ///<summary> 게임 언어가 변경됩니다. </summary>
        GAME_LANGUAGE_CHANGED,

        ///<summary> 게임 컨트롤러 타입이 변경됩니다. </summary>
        GAME_CONTROLLER_TYPE_CHANGED,

        ///<summary> 게임 컨트롤러 조이스틱이 변경됩니다. </summary>
        GAME_CONTROLLER_JOYSTICK_CHANGED,

        ///<summary> 게임 컨트롤러 타입이 추가됩니다. </summary>
        GAME_CONTROLLER_TYPE_ADDED,

        ///<summary> 게임 컨트롤러 타입이 제거됩니다. </summary>
        GAME_CONTROLLER_TYPE_REMOVED,

        ///<summary> 게임 입력 키가 변경됩니다. </summary>
        GAME_INPUT_KEY_CHANGED,

        ///<summary> 게임 플레이에서 게이지 값 텍스트를 표시합니다. </summary>
        GAME_PLAY_SHOW_GAUGE_VALUE_TEXT,

        ///<summary> 모든 버프 아이콘을 표시합니다. </summary>
        GAME_PLAY_SHOW_ALL_BUFF_ICON,

        #endregion 설정 SETTING

        #region 스테이지 STAGE

        ///<summary> 스테이지로 이동합니다. </summary>
        MOVE_TO_STAGE,

        ///<summary> 스테이지가 생성되었습니다. </summary>
        STAGE_SPAWNED,

        ///<summary> 보스 모드에 진입합니다. </summary>
        BOSS_MODE_ENTERED,

        ///<summary> 보스 모드에서 종료합니다. </summary>
        BOSS_MODE_EXITED,

        #endregion 스테이지 STAGE

        #region 난이도 DIFFICULTY

        ///<summary> 난이도를 변경합니다. </summary>
        DIFFICULTY_STEP_CHANGE,

        #endregion 난이도 DIFFICULTY

        #region 씬 SCENE

        ///<summary> 게임 메인 씬을 변경합니다. </summary>
        CHANGE_GAME_MAIN_SCENE,

        ///<summary> 게임 메인 씬 변경이 완료되었습니다. </summary>
        CHANGE_GAME_MAIN_SCENE_COMPLETE,

        ///<summary> 타이틀로 이동합니다. </summary>
        MOVE_TO_TITLE,

        ///<summary> 엔딩으로 이동합니다. </summary>
        MOVE_TO_ENDING,

        ///<summary> 게임을 포기합니다. </summary>
        GIVE_UP_GAME,

        ///<summary> 게임을 클리어합니다. </summary>
        CLEAR_GAME,

        #endregion 씬 SCENE

        #region 경고 WARNING

        ///<summary> 경고 알림을 표시합니다. </summary>
        SHOW_WARNING_NOTICE,

        #endregion 경고 WARNING

        #region UI

        ///<summary> 게임 팝업을 엽니다. </summary>
        GAME_POPUP_OPEN,

        ///<summary> 게임 팝업을 닫습니다. </summary>
        GAME_POPUP_CLOSE,

        ///<summary> 모든 게임 팝업을 닫습니다. </summary>
        GAME_POPUP_CLOSE_ALL,

        ///<summary> 게임 커서를 새로고침합니다. </summary>
        GAME_CURSOR_REFRESH,

        ///<summary> 게임 토글을 새로고침합니다. </summary>
        GAME_TOGGLE_REFRESH,

        ///<summary> 게임 필터를 새로고침합니다. </summary>
        GAME_FILTER_REFRESH,

        ///<summary> 게임 팝업 월드맵 버튼을 클릭합니다. </summary>
        GAME_POPUP_WORLDMAP_BUTTON_CLICK,

        #endregion UI

        #region 대화 DIALOGUE

        ///<summary> 대화가 완료되었습니다. </summary>
        DIALOGUE_COMPLETE,

        #endregion 대화 DIALOGUE

        #region NPC

        ///<summary> NPC와의 상호작용으로 판매 음식을 갱신합니다. </summary>
        NPC_INTERACTION_FOOD_REROLL,

        ///<summary> NPC와의 상호작용으로 판매 유물 후보를 갱신합니다. </summary>
        NPC_INTERACTION_RELIC_REROLL,

        ///<summary> NPC와의 상호작용으로 판매 기술 후보를 갱신합니다. </summary>
        NPC_INTERACTION_SKILL_REROLL,

        ///<summary> NPC와의 상호작용으로 판매 비약 후보를 갱신합니다. </summary>
        NPC_INTERACTION_ELIXIR_REROLL,

        ///<summary> NPC와의 상호작용으로 유물 조화 기도 후보를 갱신합니다. </summary>
        NPC_INTERACTION_PRAYER_REROLL,

        ///<summary> NPC와의 상호작용으로 정수 후보를 갱신합니다. </summary>
        NPC_INTERACTION_ESSENCE_REROLL,

        #endregion NPC
    }
}