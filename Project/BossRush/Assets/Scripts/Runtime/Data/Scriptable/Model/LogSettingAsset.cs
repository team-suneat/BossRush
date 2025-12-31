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
        [SuffixLabel("애널리틱스")] public bool Analytics;

        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터")] public bool Character;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("플레이어 캐릭터")] public bool Player;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("몬스터 캐릭터")] public bool Monster;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("엘리트 캐릭터")] public bool Elite;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("보스캐릭터")] public bool Boss;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 능력")] public bool Ability;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 생성")] public bool CharacterSpawn;

        [FoldoutGroup("[캐릭터 랜더러]")][SuffixLabel("애니메이션")] public bool Animation;
        [FoldoutGroup("[캐릭터 랜더러]")][SuffixLabel("랜더러")] public bool Renderer;

        [FoldoutGroup("[전투]")][SuffixLabel("공격")] public bool Attack;
        [FoldoutGroup("[전투]")][SuffixLabel("버프")] public bool Buff;
        [FoldoutGroup("[전투]")][SuffixLabel("피해량 계산")] public bool Damage;
        [FoldoutGroup("[전투]")][SuffixLabel("이펙트")] public bool Effect;
        [FoldoutGroup("[전투]")][SuffixLabel("패시브")] public bool Passive;
        [FoldoutGroup("[전투]")][SuffixLabel("능력치")] public bool Stat;
        [FoldoutGroup("[전투]")][SuffixLabel("캐릭터 전투자원")] public bool Vital;
        [FoldoutGroup("[전투]")][SuffixLabel("전투자원")] public bool BattleResource;

        [FoldoutGroup("[기술]")][SuffixLabel("기술")] public bool Skill;

        [FoldoutGroup("[아이템]")][SuffixLabel("재화")] public bool Currency;
        [FoldoutGroup("[아이템]")][SuffixLabel("아이템")] public bool Item;
        [FoldoutGroup("[아이템]")][SuffixLabel("무기")] public bool Weapon;
        [FoldoutGroup("[아이템]")][SuffixLabel("드랍 오브젝트")] public bool DropObject;

        [FoldoutGroup("[해금]")][SuffixLabel("퀘스트")] public bool Quest;
        [FoldoutGroup("[해금]")][SuffixLabel("튜토리얼")] public bool Tutorial;

        [FoldoutGroup("[인게임]")][SuffixLabel("타임라인")] public bool Timeline;
        [FoldoutGroup("[인게임]")][SuffixLabel("포지션 그룹")] public bool PositionGroup;
        [FoldoutGroup("[인게임]")][SuffixLabel("시간 관리")] public bool Time;

        [FoldoutGroup("[데이터]")][SuffixLabel("데이터")] public bool Data;
        [FoldoutGroup("[데이터]")][SuffixLabel("임시 데이터")] public bool GamePref;
        [FoldoutGroup("[데이터]")][SuffixLabel("Json 데이터")] public bool JsonData;
        [FoldoutGroup("[데이터]")][SuffixLabel("리소스")] public bool Resource;
        [FoldoutGroup("[데이터]")][SuffixLabel("스크립터블 데이터")] public bool ScriptableData;
        [FoldoutGroup("[데이터]")][SuffixLabel("경로")] public bool Path;

        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터")] public bool GameData;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 캐릭터")] public bool GameData_Character;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 기술")] public bool GameData_Skill;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 스테이지")] public bool GameData_Stage;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 통계")] public bool GameData_Statistics;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 무기")] public bool GameData_Weapon;

        [FoldoutGroup("[게임 설정]")][SuffixLabel("설정")] public bool Setting;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("음향")] public bool Audio;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("카메라")] public bool Camera;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("글로벌 이벤트")] public bool Global;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("입력")] public bool Input;

        [FoldoutGroup("[지역]")][SuffixLabel("스테이지")] public bool Stage;        
        [FoldoutGroup("[지역]")][SuffixLabel("씬")] public bool Scene;

        [FoldoutGroup("[글로벌]")][SuffixLabel("스트링 텍스트")] public bool String;
        [FoldoutGroup("[글로벌]")][SuffixLabel("폰트")] public bool Font;
        [FoldoutGroup("[글로벌]")][SuffixLabel("개발용")] public bool Develop;

        [FoldoutGroup("[UI]")] public bool UI;
        [FoldoutGroup("[UI]")][SuffixLabel("버튼")] public bool UI_Button;
        [FoldoutGroup("[UI]")][SuffixLabel("게이지")] public bool UI_Gauge;
        [FoldoutGroup("[UI]")][SuffixLabel("토글")] public bool UI_Toggle;
        [FoldoutGroup("[UI]")][SuffixLabel("페이지")] public bool UI_Page;
        [FoldoutGroup("[UI]")][SuffixLabel("알림")] public bool UI_Notice;
        [FoldoutGroup("[UI]")][SuffixLabel("소지품")] public bool UI_Inventory;
        [FoldoutGroup("[UI]")][SuffixLabel("팝업")] public bool UI_Popup;
        [FoldoutGroup("[UI]")][SuffixLabel("상세정보")] public bool UI_Details;
        [FoldoutGroup("[UI]")][SuffixLabel("기술")] public bool UI_Skill;
        [FoldoutGroup("[UI]")][SuffixLabel("유물")] public bool UI_Relic;

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
            Analytics = true;

            Character = true;
            Player = true;
            Monster = true;
            Elite = true;
            Boss = true;
            Ability = true;
            CharacterSpawn = true;

            Animation = true;
            Renderer = true;

            Attack = true;
            BattleResource = true;
            Buff = true;
            Damage = true;
            Effect = true;
            Passive = true;
            Stat = true;
            Vital = true;

            Skill = true;

            Currency = true;
            Item = true;
            Weapon = true;
            DropObject = true;

            Quest = true;
            Tutorial = true;

            Timeline = true;
            PositionGroup = true;
            Time = true;

            Data = true;
            GamePref = true;
            JsonData = true;
            Resource = true;
            ScriptableData = true;
            Path = true;

            GameData = true;
            GameData_Character = true;
            GameData_Skill = true;
            GameData_Stage = true;
            GameData_Statistics = true;
            GameData_Weapon = true;

            Setting = true;
            Audio = true;
            Camera = true;
            Global = true;
            Input = true;

            Stage = true;            
            Scene = true;

            String = true;
            Font = true;
            Develop = true;

            UI = true;
            UI_Button = true;
            UI_Gauge = true;
            UI_Toggle = true;
            UI_Page = true;
            UI_Notice = true;
            UI_Inventory = true;
            UI_Popup = true;
            UI_Details = true;
            UI_Skill = true;
            UI_Relic = true;
        }

        private void SwitchOffAll()
        {
            Analytics = false;

            Character = false;
            Player = false;
            Monster = false;
            Elite = false;
            Boss = false;
            Ability = false;
            CharacterSpawn = false;

            Animation = false;
            Renderer = false;

            Attack = false;
            BattleResource = false;
            Buff = false;
            Damage = false;
            Effect = false;
            Passive = false;
            Stat = false;
            Vital = false;

            Skill = false;

            Currency = false;
            Item = false;
            Weapon = false;
            DropObject = false;

            Quest = false;
            Tutorial = false;

            Timeline = false;
            PositionGroup = false;
            Time = false;

            Data = false;
            GamePref = false;
            JsonData = false;
            Resource = false;
            ScriptableData = false;
            Path = false;

            GameData = false;
            GameData_Character = false;
            GameData_Skill = false;
            GameData_Stage = false;
            GameData_Statistics = false;
            GameData_Weapon = false;

            Setting = false;
            Audio = false;
            Camera = false;
            Global = false;
            Input = false;

            Stage = false;
            Scene = false;

            String = false;
            Font = false;
            Develop = false;

            UI = false;
            UI_Button = false;
            UI_Gauge = false;
            UI_Toggle = false;
            UI_Page = false;
            UI_Notice = false;
            
            UI_Inventory = false;
            UI_Popup = false;
            UI_Details = false;
            UI_Skill = false;
            UI_Relic = false;
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
                LogTags.Analytics => Analytics,

                LogTags.Character => Character,
                LogTags.Player => Player,
                LogTags.Monster => Monster,
                LogTags.Elite => Elite,
                LogTags.Boss => Boss,
                LogTags.Ability => Ability,
                LogTags.CharacterSpawn => CharacterSpawn,

                LogTags.Animation => Animation,
                LogTags.Renderer => Renderer,

                LogTags.Attack => Attack,
                LogTags.BattleResource => BattleResource,
                LogTags.Buff => Buff,
                LogTags.Damage => Damage,
                LogTags.Effect => Effect,
                LogTags.Passive => Passive,
                LogTags.Stat => Stat,
                LogTags.Vital => Vital,

                LogTags.Skill => Skill,

                LogTags.Currency => Currency,
                LogTags.Item => Item,
                LogTags.Weapon => Weapon,
                LogTags.DropObject => DropObject,

                LogTags.Quest => Quest,
                LogTags.Tutorial => Tutorial,

                LogTags.Timeline => Timeline,
                LogTags.PositionGroup => PositionGroup,
                LogTags.Time => Time,

                LogTags.Data => Data,
                LogTags.GamePref => GamePref,
                LogTags.JsonData => JsonData,
                LogTags.Resource => Resource,
                LogTags.ScriptableData => ScriptableData,
                LogTags.Path => Path,

                LogTags.GameData => GameData,
                LogTags.GameData_Character => GameData_Character,
                LogTags.GameData_Skill => GameData_Skill,
                LogTags.GameData_Stage => GameData_Stage,
                LogTags.GameData_Statistics => GameData_Statistics,
                LogTags.GameData_Weapon => GameData_Weapon,

                LogTags.Setting => Setting,
                LogTags.Audio => Audio,
                LogTags.Camera => Camera,
                LogTags.Global => Global,
                LogTags.Input => Input,

                LogTags.Stage => Stage,
                LogTags.Scene => Scene,

                LogTags.String => String,
                LogTags.Font => Font,
                LogTags.Develop => Develop,

                LogTags.UI => UI,
                LogTags.UI_Button => UI_Button,
                LogTags.UI_Gauge => UI_Gauge,
                LogTags.UI_Toggle => UI_Toggle,
                LogTags.UI_Page => UI_Page,
                LogTags.UI_Notice => UI_Notice,
                LogTags.UI_Inventory => UI_Inventory,
                LogTags.UI_Popup => UI_Popup,
                LogTags.UI_Details => UI_Details,
                LogTags.UI_Skill => UI_Skill,
                LogTags.UI_Relic => UI_Relic,

                _ => false,
            };
        }

        public void SwitchOn(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Analytics: { Analytics = true; } break;

                case LogTags.Character: { Character = true; } break;
                case LogTags.Player: { Player = true; } break;
                case LogTags.Monster: { Monster = true; } break;
                case LogTags.Elite: { Elite = true; } break;
                case LogTags.Boss: { Boss = true; } break;
                case LogTags.Ability: { Ability = true; } break;
                case LogTags.CharacterSpawn: { CharacterSpawn = true; } break;

                case LogTags.Animation: { Animation = true; } break;
                case LogTags.Renderer: { Renderer = true; } break;

                case LogTags.Attack: { Attack = true; } break;
                case LogTags.BattleResource: { BattleResource = true; } break;
                case LogTags.Buff: { Buff = true; } break;
                case LogTags.Damage: { Damage = true; } break;
                case LogTags.Effect: { Effect = true; } break;
                case LogTags.Passive: { Passive = true; } break;
                case LogTags.Stat: { Stat = true; } break;
                case LogTags.Vital: { Vital = true; } break;

                case LogTags.Skill: { Skill = true; } break;

                case LogTags.Currency: { Currency = true; } break;
                case LogTags.Item: { Item = true; } break;
                case LogTags.Weapon: { Weapon = true; } break;
                case LogTags.DropObject: { DropObject = true; } break;

                case LogTags.Quest: { Quest = true; } break;
                case LogTags.Tutorial: { Tutorial = true; } break;

                case LogTags.Timeline: { Timeline = true; } break;
                case LogTags.PositionGroup: { PositionGroup = true; } break;
                case LogTags.Time: { Time = true; } break;

                case LogTags.Data: { Data = true; } break;
                case LogTags.GamePref: { GamePref = true; } break;
                case LogTags.JsonData: { JsonData = true; } break;
                case LogTags.Resource: { Resource = true; } break;
                case LogTags.ScriptableData: { ScriptableData = true; } break;
                case LogTags.Path: { Path = true; } break;

                case LogTags.GameData: { GameData = true; } break;
                case LogTags.GameData_Character: { GameData_Character = true; } break;
                case LogTags.GameData_Skill: { GameData_Skill = true; } break;
                case LogTags.GameData_Stage: { GameData_Stage = true; } break;
                case LogTags.GameData_Statistics: { GameData_Statistics = true; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = true; } break;

                case LogTags.Setting: { Setting = true; } break;
                case LogTags.Audio: { Audio = true; } break;
                case LogTags.Camera: { Camera = true; } break;
                case LogTags.Global: { Global = true; } break;
                case LogTags.Input: { Input = true; } break;

                case LogTags.Stage: { Stage = true; } break;
                case LogTags.Scene: { Scene = true; } break;

                case LogTags.String: { String = true; } break;
                case LogTags.Font: { Font = true; } break;
                case LogTags.Develop: { Develop = true; } break;

                case LogTags.UI: { UI = true; } break;
                case LogTags.UI_Button: { UI_Button = true; } break;
                case LogTags.UI_Gauge: { UI_Gauge = true; } break;
                case LogTags.UI_Toggle: { UI_Toggle = true; } break;
                case LogTags.UI_Page: { UI_Page = true; } break;
                case LogTags.UI_Notice: { UI_Notice = true; } break;
                case LogTags.UI_Inventory: { UI_Inventory = true; } break;
                case LogTags.UI_Popup: { UI_Popup = true; } break;
                case LogTags.UI_Details: { UI_Details = true; } break;
                case LogTags.UI_Skill: { UI_Skill = true; } break;
                case LogTags.UI_Relic: { UI_Relic = true; } break;
            }
        }

        public void SwitchOff(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Analytics: { Analytics = false; } break;

                case LogTags.Character: { Character = false; } break;
                case LogTags.Player: { Player = false; } break;
                case LogTags.Monster: { Monster = false; } break;
                case LogTags.Elite: { Elite = false; } break;
                case LogTags.Boss: { Boss = false; } break;
                case LogTags.Ability: { Ability = false; } break;
                case LogTags.CharacterSpawn: { CharacterSpawn = false; } break;

                case LogTags.Animation: { Animation = false; } break;
                case LogTags.Renderer: { Renderer = false; } break;

                case LogTags.Attack: { Attack = false; } break;
                case LogTags.BattleResource: { BattleResource = false; } break;
                case LogTags.Buff: { Buff = false; } break;
                case LogTags.Damage: { Damage = false; } break;
                case LogTags.Effect: { Effect = false; } break;
                case LogTags.Passive: { Passive = false; } break;
                case LogTags.Stat: { Stat = false; } break;
                case LogTags.Vital: { Vital = false; } break;

                case LogTags.Skill: { Skill = false; } break;

                case LogTags.Currency: { Currency = false; } break;
                case LogTags.Item: { Item = false; } break;
                case LogTags.Weapon: { Weapon = false; } break;
                case LogTags.DropObject: { DropObject = false; } break;

                case LogTags.Quest: { Quest = false; } break;
                case LogTags.Tutorial: { Tutorial = false; } break;

                case LogTags.Timeline: { Timeline = false; } break;
                case LogTags.PositionGroup: { PositionGroup = false; } break;
                case LogTags.Time: { Time = false; } break;

                case LogTags.Data: { Data = false; } break;
                case LogTags.GamePref: { GamePref = false; } break;
                case LogTags.JsonData: { JsonData = false; } break;
                case LogTags.Resource: { Resource = false; } break;
                case LogTags.ScriptableData: { ScriptableData = false; } break;
                case LogTags.Path: { Path = false; } break;

                case LogTags.GameData: { GameData = false; } break;
                case LogTags.GameData_Character: { GameData_Character = false; } break;
                case LogTags.GameData_Skill: { GameData_Skill = false; } break;
                case LogTags.GameData_Stage: { GameData_Stage = false; } break;
                case LogTags.GameData_Statistics: { GameData_Statistics = false; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = false; } break;

                case LogTags.Setting: { Setting = false; } break;
                case LogTags.Audio: { Audio = false; } break;
                case LogTags.Camera: { Camera = false; } break;
                case LogTags.Global: { Global = false; } break;
                case LogTags.Input: { Input = false; } break;

                case LogTags.Stage: { Stage = false; } break;
                case LogTags.Scene: { Scene = false; } break;

                case LogTags.String: { String = false; } break;
                case LogTags.Font: { Font = false; } break;
                case LogTags.Develop: { Develop = false; } break;

                case LogTags.UI: { UI = false; } break;
                case LogTags.UI_Button: { UI_Button = false; } break;
                case LogTags.UI_Gauge: { UI_Gauge = false; } break;
                case LogTags.UI_Toggle: { UI_Toggle = false; } break;
                case LogTags.UI_Page: { UI_Page = false; } break;
                case LogTags.UI_Notice: { UI_Notice = false; } break;
                case LogTags.UI_Inventory: { UI_Inventory = false; } break;
                case LogTags.UI_Popup: { UI_Popup = false; } break;
                case LogTags.UI_Details: { UI_Details = false; } break;
                case LogTags.UI_Skill: { UI_Skill = false; } break;
                case LogTags.UI_Relic: { UI_Relic = false; } break;
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