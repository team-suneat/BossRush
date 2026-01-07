namespace TeamSuneat
{
    public enum LogTags
    {
        None,

        #region Core

        /// <summary> 탐지 </summary>
        Detect,

        /// <summary> 물리 </summary>
        Physics,

        #endregion Core

        #region Character

        /// <summary> 캐릭터 </summary>
        Character,

        /// <summary> 플레이어 캐릭터 </summary>
        Player,

        /// <summary> 몬스터 캐릭터 </summary>
        Monster,

        /// <summary> 캐릭터 생성 </summary>
        CharacterSpawn,

        /// <summary> 캐릭터 상태 </summary>
        CharacterState,

        #endregion Character

        #region Character-Renderer

        /// <summary> 애니메이션 </summary>
        Animation,

        #endregion Character-Renderer

        #region Character-Battle

        /// <summary> 공격 </summary>
        Attack,

        /// <summary> 전투 자원(생명력/자원/보호막) </summary>
        BattleResource,

        /// <summary> 피해량 계산 </summary>
        Damage,

        /// <summary> 이펙트 </summary>
        Effect,

        /// <summary> 능력치 </summary>
        Stat,

        /// <summary> 캐릭터 전투 자원 </summary>
        Vital,

        #endregion Character-Battle

        #region Skill

        /// <summary> 기술 </summary>
        Skill,

        #endregion Skill

        #region Item

        /// <summary> 재화 </summary>
        Currency,

        #endregion Item

        #region Game-Data

        /// <summary> 게임 데이터 </summary>
        GameData,

        /// <summary> 게임 데이터 : 스테이지 </summary>
        GameData_Stage,

        /// <summary> 게임 데이터 : 무기 </summary>
        GameData_Weapon,

        /// <summary> 게임 데이터 : 악세사리 </summary>
        GameData_Accessory,

        #endregion Game-Data

        #region Data

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

        #endregion Data

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

        /// <summary> 입력 버튼 상태 </summary>
        Input_ButtonState,

        #endregion Setting

        #region Stage

        /// <summary> 스테이지 </summary>
        Stage,

        /// <summary> 씬 </summary>
        Scene,

        #endregion Stage

        #region Time

        /// <summary> 시간 관리 </summary>
        Time,

        #endregion Time

        #region MapObject

        /// <summary> 포지션 그룹 </summary>
        PositionGroup,

        #endregion MapObject

        #region String

        /// <summary> 스트링 텍스트 </summary>
        String,

        /// <summary> 폰트 </summary>
        Font,

        #endregion String

        #region UI

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

        /// <summary> UI 팝업 </summary>
        UI_Popup,

        /// <summary> UI 상세정보 </summary>
        UI_Details,

        /// <summary> UI 스킬 </summary>
        UI_Skill,

        #endregion UI

        #region Media

        /// <summary> 비디오 </summary>
        Video,

        #endregion Media
    }
}
