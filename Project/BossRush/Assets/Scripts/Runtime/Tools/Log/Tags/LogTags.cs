namespace TeamSuneat
{
    public enum LogTags
    {
        None,

        Analytics,

        #region Character

        /// <summary> 캐릭터 </summary>
        Character,

        /// <summary> 플레이어 캐릭터 </summary>
        Player,

        /// <summary> 몬스터 캐릭터 </summary>
        Monster,

        /// <summary> 엘리트 캐릭터 </summary>
        Elite,

        /// <summary> 보스 캐릭터 </summary>
        Boss,

        /// <summary> 캐릭터 능력 </summary>
        Ability,

        /// <summary> 캐릭터 생성 </summary>
        CharacterSpawn,

        #endregion Character

        #region Character-Battle

        /// <summary> 공격 </summary>
        Attack,

        /// <summary> 전투 자원(생명력/자원/보호막) </summary>
        BattleResource,

        /// <summary> 버프 </summary>
        Buff,

        /// <summary> 피해량 계산 </summary>
        Damage,

        /// <summary> 이펙트 </summary>
        Effect,

        /// <summary> 패시브 </summary>
        Passive,

        /// <summary> 능력치 </summary>
        Stat,

        /// <summary> 캐릭터 전투 자원 </summary>
        Vital,

        #endregion Character-Battle

        #region Character-Data

        /// <summary> 데이터 </summary>
        Data,

        /// <summary> 임시 데이터 </summary>
        GamePref,

        /// <summary> Json 데이터 </summary>
        JsonData,

        /// <summary> 리소스 </summary>
        Resource,

        /// <summary> 스크립터블 데이터 </summary>
        ScriptableData,

        /// <summary> 경로 </summary>
        Path,

        #endregion Character-Data

        #region Game-Data

        /// <summary> 게임 데이터 </summary>
        GameData,

        /// <summary> 게임 데이터 : 캐릭터 </summary>
        GameData_Character,

        /// <summary> 게임 데이터 : 기술 </summary>
        GameData_Skill,

        /// <summary> 게임 데이터 : 스테이지 </summary>
        GameData_Stage,

        /// <summary> 게임 데이터 : 통계 </summary>
        GameData_Statistics,

        /// <summary> 게임 데이터 : 무기 </summary>
        GameData_Weapon,

        /// <summary> 게임 데이터 : 악세사리 </summary>
        GameData_Accessory,

        /// <summary> 퀘스트 </summary>
        Quest,

        /// <summary> 튜토리얼 </summary>
        Tutorial,

        #endregion Game-Data

        #region Develop

        /// <summary> 개발용 </summary>
        Develop,

        #endregion Develop

        #region Item

        /// <summary> 재화 </summary>
        Currency,

        /// <summary> 아이템 </summary>
        Item,

        /// <summary> 무기 </summary>
        Weapon,

        /// <summary> 드랍 오브젝트 </summary>
        DropObject,

        #endregion Item

        #region MapObject

        /// <summary> 포지션 그룹 </summary>
        PositionGroup,

        #endregion MapObject

        #region Renderer

        /// <summary> 애니메이션 </summary>
        Animation,

        /// <summary> 랜더러 </summary>
        Renderer,

        #endregion Renderer

        /// <summary> 기술 </summary>
        Skill,

        #region Setting

        /// <summary> 설정 </summary>
        Setting,

        /// <summary> 음향 </summary>
        Audio,

        /// <summary> 카메라 </summary>
        Camera,


        /// <summary> 글로벌 이벤트 </summary>
        Global,

        /// <summary> 입력 </summary>
        Input,


        #endregion Setting

        #region Stage

        Stage,

        #endregion Stage

        #region String

        /// <summary> 스트링 텍스트 </summary>
        String,

        /// <summary> 폰트 </summary>
        Font,

        #endregion String

        #region Timeline

        /// <summary> 타임라인 </summary>
        Timeline,

        #endregion Timeline

        #region Time

        /// <summary> 시간 관리 </summary>
        Time,

        #endregion Time

        #region Scene

        /// <summary> 씬 </summary>
        Scene,

        #endregion Scene

        #region UI (일반)

        /// <summary> UI </summary>
        UI,

        /// <summary> UI 버튼 </summary>
        UI_Button,

        /// <summary> UI 게이지 </summary>
        UI_Gauge,

        /// <summary> UI 토글 </summary>
        UI_Toggle,

        /// <summary> UI 페이지 </summary>
        UI_Page,

        /// <summary> UI 알림 </summary>
        UI_Notice,

        #endregion UI

        #region  UI (특수)

        /// <summary> UI 인벤토리 </summary>
        UI_Inventory,

        /// <summary> UI 팝업 </summary>
        UI_Popup,

        /// <summary> UI 상세정보 </summary>
        UI_Details,

        /// <summary> UI 스킬 </summary>
        UI_Skill,

        /// <summary> UI 유물 </summary>
        UI_Relic,

        #endregion

    }
}