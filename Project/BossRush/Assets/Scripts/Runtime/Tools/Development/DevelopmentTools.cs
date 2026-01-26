using System;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat.Development
{
    public enum DevelopmentToolTab
    {
        GameTime,
        LogTag,
        GamePlay,
        Cheat,
        Stat,
        GameData,

        // Charm,
    }

    public class DevelopmentTools : MonoBehaviour
    {
        private bool _isWindowOpen = false;
        private bool _isFirstOpen = true;
        private Rect _windowRect;
        private DevelopmentToolsGUI _gui;
        private DevelopmentToolTab _selectedTab = DevelopmentToolTab.GameTime;

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

            DrawTabButtons();
            GUILayout.Space(10);

            DrawSelectedTabContent();

            GUILayout.EndScrollView();

            GUI.DragWindow();
        }

        private void DrawTabButtons()
        {
            string[] tabNames = new string[]
            {
                "게임 타임",
                "로그 태그",
                "게임 플레이",
                "치트",
                // "부적",
                "능력치",
                "게임 데이터"
            };

            int newSelectedTab = _gui.DrawSelectionGrid((int)_selectedTab, tabNames, tabNames.Length, useWidth: true, useHeight: false);

            if (newSelectedTab != (int)_selectedTab)
            {
                _selectedTab = (DevelopmentToolTab)newSelectedTab;
            }
        }

        private void DrawSelectedTabContent()
        {
            switch (_selectedTab)
            {
                case DevelopmentToolTab.GameTime:
                    DrawGameTimeSection();
                    break;

                case DevelopmentToolTab.LogTag:
                    DrawLogTagSection();
                    break;

                case DevelopmentToolTab.GamePlay:
                    DrawGamePlaySection();
                    break;

                case DevelopmentToolTab.Cheat:
                    DrawCheatSection();
                    break;

                // case DevelopmentToolTab.Charm:
                // DrawCharmSection();
                // break;

                case DevelopmentToolTab.Stat:
                    DrawStatSection();
                    break;

                case DevelopmentToolTab.GameData:
                    DrawGameDataSection();
                    break;
            }
        }

        private void DrawGameTimeSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("게임 타임 스케일");

            GUILayout.BeginHorizontal();
            _gui.DrawButton("0.1x", () => GameTimeManager.Instance?.SetFactor(0.1f));
            _gui.DrawButton("0.5x", () => GameTimeManager.Instance?.SetFactor(0.5f));
            _gui.DrawButton("1.0x", () => GameTimeManager.Instance?.SetFactor(1.0f));
            _gui.DrawButton("2.0x", () => GameTimeManager.Instance?.SetFactor(2.0f));
            _gui.DrawButton("3.0x", () => GameTimeManager.Instance?.SetFactor(3.0f));
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
                LogTags.Charm => "Charm",
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
            string cameraShakeLabel = JsonDataManager.FindStringClone("Option_CameraShake");
            if (string.IsNullOrEmpty(cameraShakeLabel))
            {
                cameraShakeLabel = "카메라 쉐이크";
            }
            play.CameraShake = _gui.DrawContentToggleButton(cameraShakeLabel, play.CameraShake, useWidth: true, useHeight: true);

            // 진동
            string vibrationLabel = JsonDataManager.FindStringClone("Option_Vibration");
            if (string.IsNullOrEmpty(vibrationLabel))
            {
                vibrationLabel = "진동";
            }
            play.Vibration = _gui.DrawContentToggleButton(vibrationLabel, play.Vibration, useWidth: true, useHeight: true);

            // 피해량 텍스트
            string damageTextLabel = JsonDataManager.FindStringClone("Option_DamageText");
            if (string.IsNullOrEmpty(damageTextLabel))
            {
                damageTextLabel = "피해량 텍스트";
            }
            play.UseDamageText = _gui.DrawContentToggleButton(damageTextLabel, play.UseDamageText, useWidth: true, useHeight: true);

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
            cheat.IsNotDead = _gui.DrawContentToggleButton("죽지 않음", cheat.IsNotDead, useWidth: true, useHeight: true);

            // 펄스 비용 없음
            cheat.IsNotCostPulse = _gui.DrawContentToggleButton("펄스 비용 없음", cheat.IsNotCostPulse, useWidth: true, useHeight: true);

            GUILayout.EndVertical();
        }

        private void DrawCharmSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("부적 관리", useWidth: true, useHeight: true);

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null || profile.Charm == null)
            {
                _gui.DrawContentLabel("프로필 또는 부적 데이터를 불러올 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            VCharacterCharm charmData = profile.Charm;

            // 슬롯 정보 표시
            GUILayout.BeginHorizontal();
            _gui.DrawContentLabel($"슬롯: {charmData.SlotCharmNames.Count}/{charmData.UnlockedSlotCount}", useWidth: false, useHeight: true);
            _gui.DrawButton("슬롯 해금", () =>
            {
                charmData.UnlockSlot(1);
            });
            _gui.DrawButton("슬롯 잠금", () =>
            {
                charmData.LockSlot(1);
            });
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            // 현재 장착된 부적 목록
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("장착된 부적", useWidth: false, useHeight: true);
            if (charmData.SlotCharmNames.Count == 0)
            {
                _gui.DrawContentLabel("장착된 부적이 없습니다.");
            }
            else
            {
                for (int i = 0; i < charmData.SlotCharmNames.Count; i++)
                {
                    CharmName charmName = charmData.SlotCharmNames[i];
                    GUILayout.BeginVertical("box");

                    GUILayout.BeginHorizontal();
                    _gui.DrawContentLabel($"{i + 1}. {charmName}");
                    _gui.DrawButton("제거", () =>
                    {
                        charmData.RemoveCharm(charmName);
                    });
                    GUILayout.EndHorizontal();

                    // 부적 효과 설명 표시
                    CharmAssetData charmAssetData = ScriptableDataManager.Instance?.FindCharmClone(charmName);
                    if (charmAssetData != null && !string.IsNullOrEmpty(charmAssetData.Description))
                    {
                        GUILayout.Space(3);
                        _gui.DrawContentLabel(charmAssetData.Description, useWidth: true, useHeight: false);
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(3);
                }
            }
            GUILayout.EndVertical();

            GUILayout.Space(5);

            // 부적 추가 섹션
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("부적 추가", useWidth: false, useHeight: true);

            CharmName[] allCharms = Enum.GetValues(typeof(CharmName))
                .Cast<CharmName>()
                .Where(charm => charm != CharmName.None)
                .ToArray();

            if (allCharms.Length == 0)
            {
                _gui.DrawContentLabel("사용 가능한 부적이 없습니다.");
            }
            else
            {
                // 카테고리별로 그룹화
                var attackCharms = allCharms.Where(c => (int)c >= 100 && (int)c < 200).ToArray();
                var skillCharms = allCharms.Where(c => (int)c >= 200 && (int)c < 300).ToArray();
                var supportCharms = allCharms.Where(c => (int)c >= 300 && (int)c < 400).ToArray();
                var counterCharms = allCharms.Where(c => (int)c >= 400 && (int)c < 500).ToArray();

                // 공격 부적
                if (attackCharms.Length > 0)
                {
                    DrawCharmCategory("공격", attackCharms, charmData);
                }

                // 기술 부적
                if (skillCharms.Length > 0)
                {
                    DrawCharmCategory("기술", skillCharms, charmData);
                }

                // 보조 부적
                if (supportCharms.Length > 0)
                {
                    DrawCharmCategory("보조", supportCharms, charmData);
                }

                // 반격 부적
                if (counterCharms.Length > 0)
                {
                    DrawCharmCategory("반격", counterCharms, charmData);
                }
            }

            GUILayout.EndVertical();

            GUILayout.EndVertical();
        }

        private void DrawCharmCategory(string categoryName, CharmName[] charms, VCharacterCharm characterCharmInfo)
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel(categoryName, useWidth: false, useHeight: true);

            foreach (CharmName charmName in charms)
            {
                bool isUnlocked = characterCharmInfo.CheckUnlocked(charmName);
                bool isEquipped = characterCharmInfo.SlotCharmNames.Contains(charmName);

                GUILayout.BeginVertical("box");

                GUILayout.BeginHorizontal();

                // 부적 이름 표시
                string displayName = charmName.ToString();
                if (isEquipped)
                {
                    displayName = displayName.ToSelectString();
                }
                else if (!isUnlocked)
                {
                    displayName = displayName.ToDisableString();
                }

                _gui.DrawContentLabel(displayName);

                // 해금 버튼
                if (!isUnlocked)
                {
                    _gui.DrawButton("해금", () =>
                    {
                        characterCharmInfo.Unlock(charmName);
                    }, useWidth: true, useHeight: false);
                }

                // 추가 버튼
                if (!isEquipped)
                {
                    _gui.DrawButton("추가", () =>
                    {
                        if (!isUnlocked)
                        {
                            characterCharmInfo.Unlock(charmName);
                        }
                        characterCharmInfo.AddCharm(charmName);
                    }, useWidth: true, useHeight: false);
                }
                else
                {
                    _gui.DrawContentLabel("(장착됨)");
                }

                GUILayout.EndHorizontal();

                // 부적 효과 설명 표시
                CharmAssetData charmAssetData = ScriptableDataManager.Instance?.FindCharmClone(charmName);
                if (charmAssetData != null && !string.IsNullOrEmpty(charmAssetData.Description))
                {
                    GUILayout.Space(3);
                    _gui.DrawContentLabel(charmAssetData.Description, useWidth: true, useHeight: false);
                }

                GUILayout.EndVertical();
                GUILayout.Space(3);
            }

            GUILayout.EndVertical();
            GUILayout.Space(5);
        }

        private void DrawStatSection()
        {
            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("플레이어 능력치", useWidth: true, useHeight: true);

            PlayerCharacter player = CharacterManager.Instance?.Player;
            if (player == null)
            {
                _gui.DrawContentLabel("플레이어 캐릭터를 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            StatSystem statSystem = player.Stat;
            if (statSystem == null)
            {
                _gui.DrawContentLabel("능력치 시스템을 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            // 모든 능력치 이름 가져오기
            StatNames[] allStatNames = Enum.GetValues(typeof(StatNames))
                .Cast<StatNames>()
                .Where(stat => stat != StatNames.None)
                .ToArray();

            if (allStatNames.Length == 0)
            {
                _gui.DrawContentLabel("표시할 능력치가 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            // 능력치별로 표시
            foreach (StatNames statName in allStatNames)
            {
                CharacterStat characterStat = statSystem.GetCharacterStat(statName);

                GUILayout.BeginVertical("box");

                if (characterStat != null)
                {
                    // 능력치 이름과 최종 값
                    float baseValue = characterStat.BaseValue;
                    int modifierCount = characterStat.ModifierCount;

                    string statDisplayName = GetStatDisplayName(statName);
                    string valueString = characterStat.ValueString;

                    GUILayout.BeginHorizontal();
                    _gui.DrawContentLabel($"{statDisplayName}: {valueString}");

                    if (modifierCount > 0)
                    {
                        _gui.DrawContentLabel($"(기본: {baseValue:F2}, 모디파이어: {modifierCount}개)");
                    }
                    else
                    {
                        _gui.DrawContentLabel($"(기본: {baseValue:F2})");
                    }
                    GUILayout.EndHorizontal();

                    // 모디파이어 상세 정보 표시
                    if (modifierCount > 0)
                    {
                        GUILayout.Space(3);
                        GUILayout.BeginVertical("box");
                        _gui.DrawContentLabel("모디파이어:");

                        foreach (var modifier in characterStat.StatModifiers)
                        {
                            string modifierValueString = modifier.GetValueString();
                            string sourceString = modifier.GetSourceString();

                            if (string.IsNullOrEmpty(sourceString))
                            {
                                sourceString = "알 수 없음";
                            }
                            else
                            {
                                // 마지막 쉼표 제거
                                sourceString = sourceString.TrimEnd(',', ' ');
                            }

                            string modifierTypeString = GetModifierTypeString(modifier.Type);
                            _gui.DrawContentLabel($"  • {modifierTypeString}: {modifierValueString} ({sourceString})");
                        }

                        GUILayout.EndVertical();
                    }
                }
                else
                {
                    // 능력치가 등록되지 않은 경우 기본값 표시
                    float defaultValue = statSystem.FindValueOrDefault(statName);
                    string statDisplayName = GetStatDisplayName(statName);
                    _gui.DrawContentLabel($"{statDisplayName}: {defaultValue:F2} (기본값)");
                }

                GUILayout.EndVertical();
                GUILayout.Space(3);
            }

            GUILayout.EndVertical();
        }

        private string GetStatDisplayName(StatNames statName)
        {
            return statName switch
            {
                StatNames.Attack => "공격력",
                StatNames.Life => "최대 체력",
                StatNames.AttackSpeed => "공격 속도",
                StatNames.AttackRange => "공격 범위",
                StatNames.MoveSpeed => "이동 속도",
                StatNames.MoveSpeedMulti => "이동 속도 배율",
                StatNames.Mana => "마나",
                StatNames.Pulse => "펄스",
                StatNames.PulseRegen => "펄스 재생량",
                StatNames.Barrier => "보호막",
                StatNames.BarrierMulti => "보호막 배율",
                _ => statName.ToString()
            };
        }

        private string GetModifierTypeString(StatModType modType)
        {
            return modType switch
            {
                StatModType.Flat => "고정값",
                StatModType.PercentAdd => "퍼센트 추가",
                StatModType.PercentMulti => "퍼센트 배율",
                StatModType.Use => "사용",
                _ => modType.ToString()
            };
        }

        private void DrawGameDataSection()
        {
            GameDataManager dataManager = GetGameDataManager();

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("저장/로드", useWidth: true, useHeight: true);

            if (dataManager == null)
            {
                _gui.DrawContentLabel("GameDataManager 인스턴스를 찾을 수 없습니다.");
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginHorizontal();
            _gui.DrawButton("게임 데이터 저장", () =>
            {
                dataManager.Save();
                Debug.Log("게임 데이터를 저장했습니다.");
            }, useWidth: true, useHeight: false);
            _gui.DrawButton("게임 데이터 로드", () =>
            {
                dataManager.LoadGameDataWithRecovery();
                Debug.Log("게임 데이터를 로드했습니다.");
            }, useWidth: false);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("백업 복구", useWidth: true, useHeight: true);

            _gui.DrawButton("백업 파일에서 복구", () =>
            {
                bool success = dataManager.TryLoadFromBackup();
                if (!success)
                {
                    Debug.LogWarning("백업 파일에서 복구에 실패했습니다.");
                }
            }, useWidth: false);

            _gui.DrawButton("모든 파일에서 복구 시도", () =>
            {
                bool success = dataManager.TryLoadFromAnyAvailableFile();
                if (!success)
                {
                    Debug.LogWarning("모든 파일에서 복구에 실패했습니다.");
                }
            }, useWidth: false);

            _gui.DrawButton("가장 최근 백업으로 복구", () =>
            {
                bool success = dataManager.RestoreFromBackup();
                if (!success)
                {
                    Debug.LogWarning("백업 복구에 실패했습니다.");
                }
            }, useWidth: false);

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("진단/분석", useWidth: true, useHeight: true);

            _gui.DrawButton("세이브 파일 상태 확인", () =>
            {
                dataManager.LogAllSaveFileStatus();
            }, useWidth: false);

            _gui.DrawButton("백업 파일 정보 출력", () =>
            {
                dataManager.LogBackupFileInfo();
            }, useWidth: false);

            _gui.DrawButton("모든 세이브 파일 진단", () =>
            {
                dataManager.DiagnoseAllSaveFiles();
            }, useWidth: false);

            _gui.DrawButton("마이그레이션 상태 점검", () =>
            {
                dataManager.CheckAllSaveFilesMigrationStatus();
            }, useWidth: false);

            _gui.DrawButton("세이브 파일 통계 출력", () =>
            {
                dataManager.PrintSaveFileStatistics();
            }, useWidth: false);

            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            _gui.DrawTitleLabel("파일 관리", useWidth: true, useHeight: true);

            _gui.DrawButton("에디터용 세이브 파일 삭제", () =>
            {
                GameDataManager.DeleteSaveFileForEditor();
                Debug.Log("에디터용 세이브 파일을 삭제했습니다.");
            }, useWidth: false);

            GUILayout.EndVertical();
        }

        private GameDataManager GetGameDataManager()
        {
            var gameApp = GameApp.Instance;
            if (gameApp != null && gameApp.dataManager != null)
            {
                return gameApp.dataManager;
            }
            return null;
        }
    }
}