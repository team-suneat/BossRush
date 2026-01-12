using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TeamSuneat
{
    [CreateAssetMenu(fileName = "LogSetting", menuName = "TeamSuneat/Scriptable/LogSetting")]
    public class LogSettingAsset : ScriptableObject
    {
        #region Core

        [FoldoutGroup("[Core]")][SuffixLabel("탐지")] public bool Detect;
        [FoldoutGroup("[Core]")][SuffixLabel("물리")] public bool Physics;

        #endregion Core

        #region Character

        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터")] public bool Character;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("플레이어 캐릭터")] public bool Player;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("몬스터 캐릭터")] public bool Monster;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 생성")] public bool CharacterSpawn;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 상태")] public bool CharacterState;

        #endregion Character

        #region Character-Renderer

        [FoldoutGroup("[캐릭터 렌더러]")][SuffixLabel("애니메이션")] public bool Animation;

        #endregion Character-Renderer

        #region Character-Battle

        [FoldoutGroup("[전투]")][SuffixLabel("공격")] public bool Attack;
        [FoldoutGroup("[전투]")][SuffixLabel("전투 자원")] public bool BattleResource;
        [FoldoutGroup("[전투]")][SuffixLabel("피해량 계산")] public bool Damage;
        [FoldoutGroup("[전투]")][SuffixLabel("이펙트")] public bool Effect;
        [FoldoutGroup("[전투]")][SuffixLabel("능력치")] public bool Stat;
        [FoldoutGroup("[전투]")][SuffixLabel("캐릭터 전투 자원")] public bool Vital;

        #endregion Character-Battle

        #region Skill

        [FoldoutGroup("[기술]")][SuffixLabel("기술")] public bool Skill;

        #endregion Skill

        #region Item

        [FoldoutGroup("[아이템]")][SuffixLabel("재화")] public bool Currency;

        #endregion Item

        #region Game-Data

        [FoldoutGroup("[게임 데이터]")][SuffixLabel("게임 데이터")] public bool GameData;
        [FoldoutGroup("[게임 데이터]")][SuffixLabel("게임 데이터 - 스테이지")] public bool GameData_Stage;
        [FoldoutGroup("[게임 데이터]")][SuffixLabel("게임 데이터 - 무기")] public bool GameData_Weapon;
        [FoldoutGroup("[게임 데이터]")][SuffixLabel("게임 데이터 - 악세사리")] public bool GameData_Accessory;

        #endregion Game-Data

        #region Data

        [FoldoutGroup("[데이터]")][SuffixLabel("임시 데이터")] public bool GamePref;
        [FoldoutGroup("[데이터]")][SuffixLabel("Json 데이터")] public bool JsonData;
        [FoldoutGroup("[데이터]")][SuffixLabel("리소스")] public bool Resource;
        [FoldoutGroup("[데이터]")][SuffixLabel("스크립터블 데이터")] public bool ScriptableData;
        [FoldoutGroup("[데이터]")][SuffixLabel("경로")] public bool Path;

        #endregion Data

        #region Setting

        [FoldoutGroup("[게임 설정]")][SuffixLabel("설정")] public bool Setting;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("음향")] public bool Audio;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("카메라")] public bool Camera;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("글로벌 이벤트")] public bool Global;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("입력")] public bool Input;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("입력 버튼 상태")] public bool Input_ButtonState;

        #endregion Setting

        #region Stage

        [FoldoutGroup("[지역]")][SuffixLabel("스테이지")] public bool Stage;
        [FoldoutGroup("[지역]")][SuffixLabel("씬")] public bool Scene;

        #endregion Stage

        #region Time

        [FoldoutGroup("[시간]")][SuffixLabel("시간 관리")] public bool Time;

        #endregion Time

        #region MapObject

        [FoldoutGroup("[맵 오브젝트]")][SuffixLabel("포지션 그룹")] public bool PositionGroup;

        #endregion MapObject

        #region String

        [FoldoutGroup("[문자열]")][SuffixLabel("스트링 텍스트")] public bool String;
        [FoldoutGroup("[문자열]")][SuffixLabel("폰트")] public bool Font;

        #endregion String

        #region UI

        [FoldoutGroup("[UI]")] public bool UI;
        [FoldoutGroup("[UI]")][SuffixLabel("버튼")] public bool UI_Button;
        [FoldoutGroup("[UI]")][SuffixLabel("게이지")] public bool UI_Gauge;
        [FoldoutGroup("[UI]")][SuffixLabel("토글")] public bool UI_Toggle;
        [FoldoutGroup("[UI]")][SuffixLabel("페이지")] public bool UI_Page;
        [FoldoutGroup("[UI]")][SuffixLabel("알림")] public bool UI_Notice;
        [FoldoutGroup("[UI]")][SuffixLabel("팝업")] public bool UI_Popup;
        [FoldoutGroup("[UI]")][SuffixLabel("상세정보")] public bool UI_Details;
        [FoldoutGroup("[UI]")][SuffixLabel("기술")] public bool UI_Skill;
        [FoldoutGroup("[UI]")][SuffixLabel("선택 이벤트")] public bool UI_SelectEvent;

        #endregion UI

        #region Media

        [FoldoutGroup("[미디어]")][SuffixLabel("비디오")] public bool Video;

        #endregion Media

        public bool LoadString;

        #region FOR EDITOR

        [FoldoutGroup("#Button")]
        [Button("Switch On All", ButtonSizes.Large)]
        public void ExternSwitchOnAll()
        {
            SwitchOnAll();
        }

        [FoldoutGroup("#Button")]
        [Button("Switch Off All", ButtonSizes.Large)]
        public void ExternSwitchOffAll()
        {
            SwitchOffAll();
        }

        #endregion FOR EDITOR

        private void SwitchOnAll()
        {
            Detect = true;
            Physics = true;

            Character = true;
            Player = true;
            Monster = true;
            CharacterSpawn = true;
            CharacterState = true;

            Animation = true;

            Attack = true;
            BattleResource = true;
            Damage = true;
            Effect = true;
            Stat = true;
            Vital = true;

            Skill = true;

            Currency = true;

            GameData = true;
            GameData_Stage = true;
            GameData_Weapon = true;
            GameData_Accessory = true;

            GamePref = true;
            JsonData = true;
            Resource = true;
            ScriptableData = true;
            Path = true;

            Setting = true;
            Audio = true;
            Camera = true;
            Global = true;
            Input = true;
            Input_ButtonState = true;

            Stage = true;
            Scene = true;

            Time = true;

            PositionGroup = true;

            String = true;
            Font = true;

            UI = true;
            UI_Button = true;
            UI_Gauge = true;
            UI_Toggle = true;
            UI_Page = true;
            UI_Notice = true;
            UI_Popup = true;
            UI_Details = true;
            UI_Skill = true;
            UI_SelectEvent = true;

            Video = true;
        }

        private void SwitchOffAll()
        {
            Detect = false;
            Physics = false;

            Character = false;
            Player = false;
            Monster = false;
            CharacterSpawn = false;
            CharacterState = false;

            Animation = false;

            Attack = false;
            BattleResource = false;
            Damage = false;
            Effect = false;
            Stat = false;
            Vital = false;

            Skill = false;

            Currency = false;

            GameData = false;
            GameData_Stage = false;
            GameData_Weapon = false;
            GameData_Accessory = false;

            GamePref = false;
            JsonData = false;
            Resource = false;
            ScriptableData = false;
            Path = false;

            Setting = false;
            Audio = false;
            Camera = false;
            Global = false;
            Input = false;
            Input_ButtonState = false;

            Stage = false;
            Scene = false;

            Time = false;

            PositionGroup = false;

            String = false;
            Font = false;

            UI = false;
            UI_Button = false;
            UI_Gauge = false;
            UI_Toggle = false;
            UI_Page = false;
            UI_Notice = false;
            UI_Popup = false;
            UI_Details = false;
            UI_Skill = false;
            UI_SelectEvent = false;

            Video = false;
        }

        public void OnLoadData()
        {
#if !UNITY_EDITOR
            SwitchOnAll();
#endif
        }

        public bool Find(LogTags logTag)
        {
            return logTag switch
            {
                LogTags.Detect => Detect,
                LogTags.Physics => Physics,

                LogTags.Character => Character,
                LogTags.Player => Player,
                LogTags.Monster => Monster,
                LogTags.CharacterSpawn => CharacterSpawn,
                LogTags.CharacterState => CharacterState,

                LogTags.Animation => Animation,

                LogTags.Attack => Attack,
                LogTags.BattleResource => BattleResource,
                LogTags.Damage => Damage,
                LogTags.Effect => Effect,
                LogTags.Stat => Stat,
                LogTags.Vital => Vital,

                LogTags.Skill => Skill,

                LogTags.Currency => Currency,

                LogTags.GameData => GameData,
                LogTags.GameData_Stage => GameData_Stage,
                LogTags.GameData_Weapon => GameData_Weapon,
                LogTags.GameData_Accessory => GameData_Accessory,

                LogTags.GamePref => GamePref,
                LogTags.JsonData => JsonData,
                LogTags.Resource => Resource,
                LogTags.ScriptableData => ScriptableData,
                LogTags.Path => Path,

                LogTags.Setting => Setting,
                LogTags.Audio => Audio,
                LogTags.Camera => Camera,
                LogTags.Global => Global,
                LogTags.Input => Input,
                LogTags.Input_ButtonState => Input_ButtonState,

                LogTags.Stage => Stage,
                LogTags.Scene => Scene,

                LogTags.Time => Time,

                LogTags.PositionGroup => PositionGroup,

                LogTags.String => String,
                LogTags.Font => Font,

                LogTags.UI => UI,
                LogTags.UI_Button => UI_Button,
                LogTags.UI_Gauge => UI_Gauge,
                LogTags.UI_Toggle => UI_Toggle,
                LogTags.UI_Page => UI_Page,
                LogTags.UI_Notice => UI_Notice,
                LogTags.UI_Popup => UI_Popup,
                LogTags.UI_Details => UI_Details,
                LogTags.UI_Skill => UI_Skill,
                LogTags.UI_SelectEvent => UI_SelectEvent,

                LogTags.Video => Video,

                _ => false,
            };
        }

        public void SwitchOn(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Detect: { Detect = true; } break;
                case LogTags.Physics: { Physics = true; } break;

                case LogTags.Character: { Character = true; } break;
                case LogTags.Player: { Player = true; } break;
                case LogTags.Monster: { Monster = true; } break;
                case LogTags.CharacterSpawn: { CharacterSpawn = true; } break;
                case LogTags.CharacterState: { CharacterState = true; } break;

                case LogTags.Animation: { Animation = true; } break;

                case LogTags.Attack: { Attack = true; } break;
                case LogTags.BattleResource: { BattleResource = true; } break;
                case LogTags.Damage: { Damage = true; } break;
                case LogTags.Effect: { Effect = true; } break;
                case LogTags.Stat: { Stat = true; } break;
                case LogTags.Vital: { Vital = true; } break;

                case LogTags.Skill: { Skill = true; } break;

                case LogTags.Currency: { Currency = true; } break;

                case LogTags.GameData: { GameData = true; } break;
                case LogTags.GameData_Stage: { GameData_Stage = true; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = true; } break;
                case LogTags.GameData_Accessory: { GameData_Accessory = true; } break;

                case LogTags.GamePref: { GamePref = true; } break;
                case LogTags.JsonData: { JsonData = true; } break;
                case LogTags.Resource: { Resource = true; } break;
                case LogTags.ScriptableData: { ScriptableData = true; } break;
                case LogTags.Path: { Path = true; } break;

                case LogTags.Setting: { Setting = true; } break;
                case LogTags.Audio: { Audio = true; } break;
                case LogTags.Camera: { Camera = true; } break;
                case LogTags.Global: { Global = true; } break;
                case LogTags.Input: { Input = true; } break;
                case LogTags.Input_ButtonState: { Input_ButtonState = true; } break;

                case LogTags.Stage: { Stage = true; } break;
                case LogTags.Scene: { Scene = true; } break;

                case LogTags.Time: { Time = true; } break;

                case LogTags.PositionGroup: { PositionGroup = true; } break;

                case LogTags.String: { String = true; } break;
                case LogTags.Font: { Font = true; } break;

                case LogTags.UI: { UI = true; } break;
                case LogTags.UI_Button: { UI_Button = true; } break;
                case LogTags.UI_Gauge: { UI_Gauge = true; } break;
                case LogTags.UI_Toggle: { UI_Toggle = true; } break;
                case LogTags.UI_Page: { UI_Page = true; } break;
                case LogTags.UI_Notice: { UI_Notice = true; } break;
                case LogTags.UI_Popup: { UI_Popup = true; } break;
                case LogTags.UI_Details: { UI_Details = true; } break;
                case LogTags.UI_Skill: { UI_Skill = true; } break;
                case LogTags.UI_SelectEvent: { UI_SelectEvent = true; } break;

                case LogTags.Video: { Video = true; } break;
            }
        }

        public void SwitchOff(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Detect: { Detect = false; } break;
                case LogTags.Physics: { Physics = false; } break;

                case LogTags.Character: { Character = false; } break;
                case LogTags.Player: { Player = false; } break;
                case LogTags.Monster: { Monster = false; } break;
                case LogTags.CharacterSpawn: { CharacterSpawn = false; } break;
                case LogTags.CharacterState: { CharacterState = false; } break;

                case LogTags.Animation: { Animation = false; } break;

                case LogTags.Attack: { Attack = false; } break;
                case LogTags.BattleResource: { BattleResource = false; } break;
                case LogTags.Damage: { Damage = false; } break;
                case LogTags.Effect: { Effect = false; } break;
                case LogTags.Stat: { Stat = false; } break;
                case LogTags.Vital: { Vital = false; } break;

                case LogTags.Skill: { Skill = false; } break;

                case LogTags.Currency: { Currency = false; } break;

                case LogTags.GameData: { GameData = false; } break;
                case LogTags.GameData_Stage: { GameData_Stage = false; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = false; } break;
                case LogTags.GameData_Accessory: { GameData_Accessory = false; } break;

                case LogTags.GamePref: { GamePref = false; } break;
                case LogTags.JsonData: { JsonData = false; } break;
                case LogTags.Resource: { Resource = false; } break;
                case LogTags.ScriptableData: { ScriptableData = false; } break;
                case LogTags.Path: { Path = false; } break;

                case LogTags.Setting: { Setting = false; } break;
                case LogTags.Audio: { Audio = false; } break;
                case LogTags.Camera: { Camera = false; } break;
                case LogTags.Global: { Global = false; } break;
                case LogTags.Input: { Input = false; } break;
                case LogTags.Input_ButtonState: { Input_ButtonState = false; } break;

                case LogTags.Stage: { Stage = false; } break;
                case LogTags.Scene: { Scene = false; } break;

                case LogTags.Time: { Time = false; } break;

                case LogTags.PositionGroup: { PositionGroup = false; } break;

                case LogTags.String: { String = false; } break;
                case LogTags.Font: { Font = false; } break;

                case LogTags.UI: { UI = false; } break;
                case LogTags.UI_Button: { UI_Button = false; } break;
                case LogTags.UI_Gauge: { UI_Gauge = false; } break;
                case LogTags.UI_Toggle: { UI_Toggle = false; } break;
                case LogTags.UI_Page: { UI_Page = false; } break;
                case LogTags.UI_Notice: { UI_Notice = false; } break;
                case LogTags.UI_Popup: { UI_Popup = false; } break;
                case LogTags.UI_Details: { UI_Details = false; } break;
                case LogTags.UI_Skill: { UI_Skill = false; } break;
                case LogTags.UI_SelectEvent: { UI_SelectEvent = false; } break;

                case LogTags.Video: { Video = false; } break;
            }
        }

        public void Refresh()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}