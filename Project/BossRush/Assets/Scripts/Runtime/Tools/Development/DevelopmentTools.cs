using System;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.Development
{
    public class DevelopmentTools : MonoBehaviour
    {
        private bool _isWindowOpen = false;
        private bool _isFirstOpen = true;
        private Rect _windowRect;
        private DevelopmentToolsGUI _gui;

        private const KeyCode TOGGLE_KEY = KeyCode.F1;

        private void Awake()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                gameObject.SetActive(false);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _gui = new DevelopmentToolsGUI();
            InitializeWindowRect();
        }

        private void InitializeWindowRect()
        {
            float width = Screen.width * 0.5f;
            float height = Screen.height * 0.5f;
            float x = 0f;
            float y = 0f;
            _windowRect = new Rect(x, y, width, height);
        }

        private void Update()
        {
            if (Input.GetKeyDown(TOGGLE_KEY))
            {
                bool wasOpen = _isWindowOpen;
                _isWindowOpen = !_isWindowOpen;

                if (_isWindowOpen && !wasOpen)
                {
                    if (_isFirstOpen)
                    {
                        InitializeWindowRect();
                        _isFirstOpen = false;
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                return;
            }

            if (!_isWindowOpen)
            {
                return;
            }

            // OnGUI 내부에서는 GUI.skin이 유효하므로 스타일 초기화 확인
            if (_gui.WindowStyle == null)
            {
                _gui.RefreshStyle(isEditor: false);
                // OnGUI 내부이므로 GUI.skin을 기반으로 스타일 업데이트
                if (GUI.skin != null)
                {
                    _gui.RefreshStyleFromSkin();
                }
            }

            float width = Screen.width * 0.5f;
            float height = Screen.height * 0.5f;
            _gui.RefreshSize(width, height);
            _windowRect.width = width;
            _windowRect.height = height;

            if (_isFirstOpen)
            {
                _windowRect.x = 0f;
                _windowRect.y = 0f;
            }

            _windowRect = GUILayout.Window(0, _windowRect, DrawWindow, "개발 도구 (F1 토글)", _gui.WindowStyle);
        }

        private void DrawWindow(int windowID)
        {
            _gui.ScrollPosition = GUILayout.BeginScrollView(_gui.ScrollPosition);

            _gui.DrawTitleLabel("[인게임 개발 도구]");
            GUILayout.Space(10);

            DrawGameTimeSection();
            GUILayout.Space(10);

            DrawLogTagSection();
            GUILayout.Space(10);

            DrawGamePlaySection();
            GUILayout.Space(10);

            DrawCheatSection();

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void DrawGameTimeSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("게임 타임 스케일");

            GUILayout.BeginHorizontal();
            _gui.DrawButton("0.1x", () => GameTimeManager.Instance?.SetFactor(0.1f), useWidth: false, useHeight: false);
            _gui.DrawButton("0.5x", () => GameTimeManager.Instance?.SetFactor(0.5f), useWidth: false, useHeight: false);
            _gui.DrawButton("1.0x", () => GameTimeManager.Instance?.SetFactor(1.0f), useWidth: false, useHeight: false);
            _gui.DrawButton("2.0x", () => GameTimeManager.Instance?.SetFactor(2.0f), useWidth: false, useHeight: false);
            _gui.DrawButton("3.0x", () => GameTimeManager.Instance?.SetFactor(3.0f), useWidth: false, useHeight: false);
            GUILayout.EndHorizontal();

            _gui.DrawContentLabel($"현재 타임 스케일: {Time.timeScale:F1}x");

            GUILayout.EndVertical();
        }

        private int _selectedLogTagIndex = -1;

        private void DrawLogTagSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("Log Tags");

            LogSettingAsset logSetting = ScriptableDataManager.Instance?.GetLogSetting();
            if (logSetting == null)
            {
                _gui.DrawContentLabel("LogSettingAsset을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            LogTags[] allTags = Enum.GetValues(typeof(LogTags))
                .Cast<LogTags>()
                .Where(tag => tag != LogTags.None)
                .ToArray();

            // 각 태그의 표시 이름과 상태를 포함한 문자열 배열 생성
            string[] tagDisplayNames = new string[allTags.Length];
            for (int i = 0; i < allTags.Length; i++)
            {
                LogTags tag = allTags[i];
                string displayName = GetLogTagDisplayName(tag);
                bool isEnabled = logSetting.Find(tag);

                // 상태에 따라 색상 표시
                if (isEnabled)
                {
                    tagDisplayNames[i] = displayName.ToSelectString(); // 녹색
                }
                else
                {
                    tagDisplayNames[i] = displayName.ToDisableString(); // 회색
                }
            }

            // SelectionGrid로 표시 (3열로 배치)
            int newSelectedIndex = _gui.DrawSelectionGrid(_selectedLogTagIndex, tagDisplayNames, 4, useWidth: true, useHeight: true);

            // 선택된 태그가 있으면 토글
            if (newSelectedIndex >= 0 && newSelectedIndex < allTags.Length && newSelectedIndex != _selectedLogTagIndex)
            {
                LogTags selectedTag = allTags[newSelectedIndex];
                bool isEnabled = logSetting.Find(selectedTag);

                if (isEnabled)
                {
                    logSetting.SwitchOff(selectedTag);
                }
                else
                {
                    logSetting.SwitchOn(selectedTag);
                }
                logSetting.Refresh();

                // 선택을 해제하여 다음 클릭에도 반응하도록 함
                _selectedLogTagIndex = -1;
            }
            else
            {
                _selectedLogTagIndex = newSelectedIndex;
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            _gui.DrawButton("All On", () =>
            {
                logSetting.ExternSwitchOnAll();
                logSetting.Refresh();
            }, useWidth: true, useHeight: false);

            _gui.DrawButton("All Off", () =>
            {
                logSetting.ExternSwitchOffAll();
                logSetting.Refresh();
            }, useWidth: true, useHeight: false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private string GetLogTagDisplayName(LogTags tag)
        {
            return tag switch
            {
                LogTags.CharacterSpawn => "Spawn",
                LogTags.CharacterState => "State",
                LogTags.BattleResource => "Resource",
                LogTags.GameData_Stage => "Stage",
                LogTags.GameData_Weapon => "Weapon",
                LogTags.GameData_Accessory => "Accessory",
                LogTags.Input_ButtonState => "BtnState",
                LogTags.Input_Command => "Command",
                LogTags.UI_Button => "UI_Btn",
                LogTags.UI_Gauge => "UI_Gauge",
                LogTags.UI_Toggle => "UI_Toggle",
                LogTags.UI_Page => "UI_Page",
                LogTags.UI_Notice => "UI_Notice",
                LogTags.UI_Popup => "UI_Popup",
                LogTags.UI_Details => "UI_Details",
                LogTags.UI_Skill => "UI_Skill",
                LogTags.UI_SelectEvent => "UI_Select",
                _ => tag.ToString()
            };
        }

        private void DrawGamePlaySection()
        {
            GUILayout.BeginVertical("box");
            string title = JsonDataManager.FindStringClone("Option_GameSetting");
            if (string.IsNullOrEmpty(title))
            {
                title = "게임 플레이 설정";
            }
            _gui.DrawTitleLabel(title, useWidth: true, useHeight: true);

            if (GameSetting.Instance == null)
            {
                _gui.DrawContentLabel("GameSetting을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GamePlay play = GameSetting.Instance.Play;

            // 카메라 쉐이크
            GUILayout.BeginHorizontal();
            play.CameraShake = _gui.DrawToggleButton(play.CameraShake, useColor: true, useWidth: true, useHeight: true);
            string cameraShakeLabel = JsonDataManager.FindStringClone("Option_CameraShake");
            if (string.IsNullOrEmpty(cameraShakeLabel))
            {
                cameraShakeLabel = "카메라 쉐이크";
            }
            _gui.DrawContentLabel(cameraShakeLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 진동
            GUILayout.BeginHorizontal();
            play.Vibration = _gui.DrawToggleButton(play.Vibration, useColor: true, useWidth: true, useHeight: true);
            string vibrationLabel = JsonDataManager.FindStringClone("Option_Vibration");
            if (string.IsNullOrEmpty(vibrationLabel))
            {
                vibrationLabel = "진동";
            }
            _gui.DrawContentLabel(vibrationLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 튜토리얼
            GUILayout.BeginHorizontal();
            play.UseTutorial = _gui.DrawToggleButton(play.UseTutorial, useColor: true, useWidth: true, useHeight: true);
            string tutorialLabel = JsonDataManager.FindStringClone("Option_Tutorial");
            if (string.IsNullOrEmpty(tutorialLabel))
            {
                tutorialLabel = "튜토리얼";
            }
            _gui.DrawContentLabel(tutorialLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 피해량 텍스트
            GUILayout.BeginHorizontal();
            play.UseDamageText = _gui.DrawToggleButton(play.UseDamageText, useColor: true, useWidth: true, useHeight: true);
            string damageTextLabel = JsonDataManager.FindStringClone("Option_DamageText");
            if (string.IsNullOrEmpty(damageTextLabel))
            {
                damageTextLabel = "피해량 텍스트";
            }
            _gui.DrawContentLabel(damageTextLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 상태이상 텍스트
            GUILayout.BeginHorizontal();
            play.UseStateEffectText = _gui.DrawToggleButton(play.UseStateEffectText, useColor: true, useWidth: true, useHeight: true);
            string stateEffectTextLabel = JsonDataManager.FindStringClone("Option_StateEffectText");
            if (string.IsNullOrEmpty(stateEffectTextLabel))
            {
                stateEffectTextLabel = "상태이상 텍스트";
            }
            _gui.DrawContentLabel(stateEffectTextLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 몬스터 게이지
            GUILayout.BeginHorizontal();
            play.UseMonsterGauge = _gui.DrawToggleButton(play.UseMonsterGauge, useColor: true, useWidth: true, useHeight: true);
            string monsterGaugeLabel = JsonDataManager.FindStringClone("Option_MonsterGauge");
            if (string.IsNullOrEmpty(monsterGaugeLabel))
            {
                monsterGaugeLabel = "몬스터 게이지";
            }
            _gui.DrawContentLabel(monsterGaugeLabel, useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 몬스터 생명력 텍스트
            GUILayout.BeginHorizontal();
            play.ShowMonsterLifeText = _gui.DrawToggleButton(play.ShowMonsterLifeText, useColor: true, useWidth: true, useHeight: true);
            _gui.DrawContentLabel("Show Monster Gauge Text", useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DrawCheatSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("치트 설정", useWidth: true, useHeight: true);

            if (GameSetting.Instance == null)
            {
                _gui.DrawContentLabel("GameSetting을 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GameCheat cheat = GameSetting.Instance.Cheat;

            // 죽지 않음
            GUILayout.BeginHorizontal();
            cheat.IsNotDead = _gui.DrawToggleButton(cheat.IsNotDead, useColor: true, useWidth: true, useHeight: true);
            _gui.DrawContentLabel("죽지 않음", useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            // 펄스 비용 없음
            GUILayout.BeginHorizontal();
            cheat.IsNotCostPulse = _gui.DrawToggleButton(cheat.IsNotCostPulse, useColor: true, useWidth: true, useHeight: true);
            _gui.DrawContentLabel("펄스 비용 없음", useWidth: false, useHeight: true);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}