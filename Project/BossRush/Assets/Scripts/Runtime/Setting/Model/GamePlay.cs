namespace TeamSuneat.Setting
{
    public class GamePlay
    {
        private bool _cameraShake;              // 카메라 흔들림
        private bool _vibration;                // 패드 진동
        private bool _useTutorial;              // 튜토리얼 사용
        private bool _useMonsterGauge;          // 몬스터 게이지 사용
        private bool _useDamageText;            // 피해량 텍스트 사용
        private bool _useStateEffectText;       // 상테이상 텍스트 사용
        private bool _showMonsterHealthText;      // 몬스터 생맹력 텍스트 사용
        private bool _showItemOptionRange;      // 아이템 옵션 범위 사용
        private bool _showItemOptionCompare;    // 아이템 옵션 비교 사용
        private bool _showAllBuffIcons;         // 모든 버프 아이콘 가시화 사용
        private bool _showStatusCalculations;   // 능력치 계산식 가시화 사용
        private bool _useGameplayTimer;         // 게임플레이 타이머 사용

        // For Developer

        private bool _showInvulnerableRenderer; // 플레이어 무적시 아웃라인 생성
        private bool _showMonsterAttackCooldown; // 몬스터의 공격 재사용 대기 표시
        private bool _isHideUserInterface; // 모든 ui를 숨김
        private bool _isHideNPCMarker; // 모든 NPC의 마커를 숨김

        //

        /// <summary> 카메라 쉐이크 사용 여부 </summary>
        public bool CameraShake
        {
            get => _cameraShake;
            set
            {
                if (_cameraShake != value)
                {
                    _cameraShake = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_CAMERA_SHAKE, value);
                }
            }
        }

        /// <summary> 패드 진동 사용 여부 </summary>
        public bool Vibration
        {
            get => _vibration;
            set
            {
                if (_vibration != value)
                {
                    _vibration = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_VIBRATION, value);
                }
            }
        }

        /// <summary> 몬스터의 생명력 텍스트를 표시합니다. </summary>
        public bool ShowMonsterHealthText
        {
            get => _showMonsterHealthText;
            set
            {
                if (_showMonsterHealthText != value)
                {
                    _showMonsterHealthText = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_MONSTER_LIFE_TEXT, value);
                    _ = GlobalEvent<bool>.Send(GlobalEventType.GAME_PLAY_SHOW_GAUGE_VALUE_TEXT, value);
                }
            }
        }

        /// <summary> 아이템의 옵션 범위를 표시합니다. </summary>
        public bool ShowItemOptionRange
        {
            get => _showItemOptionRange;
            set
            {
                if (_showItemOptionRange != value)
                {
                    _showItemOptionRange = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_RANGE, value);
                }
            }
        }

        /// <summary> 아이템의 옵션 비교를 표시합니다. </summary>
        public bool ShowItemOptionCompare
        {
            get => _showItemOptionCompare;
            set
            {
                if (_showItemOptionCompare != value)
                {
                    _showItemOptionCompare = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_COMPARE, value);
                }
            }
        }

        /// <summary> 튜토리얼 사용 </summary>
        public bool UseTutorial
        {
            get => _useTutorial;
            set
            {
                if (_useTutorial != value)
                {
                    _useTutorial = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_USE_TUTORIAL, value);
                }
            }
        }

        /// <summary> 머리 위 생명력 바 </summary>
        public bool UseMonsterGauge
        {
            get => _useMonsterGauge;
            set
            {
                if (_useMonsterGauge != value)
                {
                    _useMonsterGauge = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_USE_MONSTER_GAUGE, value);
                }
            }
        }

        /// <summary> 피해 텍스트 사용 </summary>
        public bool UseDamageText
        {
            get => _useDamageText;
            set
            {
                if (_useDamageText != value)
                {
                    _useDamageText = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_USE_DAMAGE_TEXT, value);
                }
            }
        }

        /// <summary> 상태이상 텍스트 사용 </summary>
        public bool UseStateEffectText
        {
            get => _useStateEffectText;
            set
            {
                if (_useStateEffectText != value)
                {
                    _useStateEffectText = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_USE_STATE_EFFECT_TEXT, value);
                }
            }
        }

        // For Developer

        /// <summary> 플레이어 무적시 랜더러에 표시합니다. </summary>
        public bool ShowInvulnerableRenderer
        {
            get
            {
                if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return _showInvulnerableRenderer;
                }
                return false;
            }
            set
            {
                if (_showInvulnerableRenderer != value)
                {
                    _showInvulnerableRenderer = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_INVULNERABLE_RENDERER, value);
                }
            }
        }

        /// <summary> 몬스터의 공격 재사용 대기를 봅니다. </summary>
        public bool ShowMonsterAttackCooldown
        {
            get
            {
                if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return _showMonsterAttackCooldown;
                }

                return false;
            }
            set
            {
                if (_showMonsterAttackCooldown != value)
                {
                    _showMonsterAttackCooldown = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_MONSTER_ATTACK_COOLDOWN, value);
                }
            }
        }

        /// <summary> 플레이어의 모든 버프를 Icon으로 보여줍니다. </summary>
        public bool ShowAllBuffIcons
        {
            get => _showAllBuffIcons;
            set
            {
                if (_showAllBuffIcons != value)
                {
                    _showAllBuffIcons = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_ALL_BUFF_ICONS, value);
                    _ = GlobalEvent<bool>.Send(GlobalEventType.GAME_PLAY_SHOW_ALL_BUFF_ICON, value);
                }
            }
        }

        public bool ShowStatusCalculations
        {
            get => _showStatusCalculations;
            set
            {
                if (_showStatusCalculations != value)
                {
                    _showStatusCalculations = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_SHOW_STATUS_CALCULATIONS, value);
                }
            }
        }

        /// <summary> 게임플레이 타이머 사용 </summary>
        public bool UseGameplayTimer
        {
            get => _useGameplayTimer;
            set
            {
                if (_useGameplayTimer != value)
                {
                    _useGameplayTimer = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_USE_GAMEPLAY_TIMER, value);
                }
            }
        }

        /// <summary> UI를 모두 숨김 </summary>
        public bool HideUserInterface
        {
            get => _isHideUserInterface;
            set
            {
                if (_isHideUserInterface != value)
                {
                    _isHideUserInterface = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_IS_HIDE_USER_INTERFACE, value);
                }
            }
        }

        public bool HideNPCMarker
        {
            get => _isHideNPCMarker;
            set
            {
                if (_isHideNPCMarker != value)
                {
                    _isHideNPCMarker = value;
                    GamePrefs.SetBool(GamePrefTypes.OPTION_IS_HIDE_NPC_MARKER, value);
                }
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────────────────

        public void Load()
        {
            if (GamePrefs.HasKey(GamePrefTypes.OPTION_CAMERA_SHAKE))
            {
                _cameraShake = GamePrefs.GetBool(GamePrefTypes.OPTION_CAMERA_SHAKE);
            }
            else
            {
                _cameraShake = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_VIBRATION))
            {
                _vibration = GamePrefs.GetBool(GamePrefTypes.OPTION_VIBRATION);
            }
            else
            {
                _vibration = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_MONSTER_LIFE_TEXT))
            {
                _showMonsterHealthText = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_MONSTER_LIFE_TEXT);
            }
            else
            {
                _showMonsterHealthText = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_MONSTER_ATTACK_COOLDOWN))
            {
                _showMonsterAttackCooldown = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_MONSTER_ATTACK_COOLDOWN);
            }
            else
            {
                _showMonsterAttackCooldown = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_ALL_BUFF_ICONS))
            {
                _showAllBuffIcons = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_ALL_BUFF_ICONS);
            }
            else
            {
                _showAllBuffIcons = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_STATUS_CALCULATIONS))
            {
                _showStatusCalculations = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_STATUS_CALCULATIONS);
            }
            else
            {
                _showStatusCalculations = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_INVULNERABLE_RENDERER))
            {
                _showInvulnerableRenderer = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_INVULNERABLE_RENDERER);
            }
            else
            {
                _showInvulnerableRenderer = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_RANGE))
            {
                _showItemOptionRange = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_RANGE);
            }
            else
            {
                _showItemOptionRange = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_COMPARE))
            {
                _showItemOptionCompare = GamePrefs.GetBool(GamePrefTypes.OPTION_SHOW_ITEM_OPTION_COMPARE);
            }
            else
            {
                _showItemOptionCompare = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_USE_TUTORIAL))
            {
                _useTutorial = GamePrefs.GetBool(GamePrefTypes.OPTION_USE_TUTORIAL);
            }
            else
            {
                _useTutorial = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_USE_MONSTER_GAUGE))
            {
                _useMonsterGauge = GamePrefs.GetBool(GamePrefTypes.OPTION_USE_MONSTER_GAUGE);
            }
            else
            {
                _useMonsterGauge = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_USE_DAMAGE_TEXT))
            {
                _useDamageText = GamePrefs.GetBool(GamePrefTypes.OPTION_USE_DAMAGE_TEXT);
            }
            else
            {
                _useDamageText = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_USE_STATE_EFFECT_TEXT))
            {
                _useStateEffectText = GamePrefs.GetBool(GamePrefTypes.OPTION_USE_STATE_EFFECT_TEXT);
            }
            else
            {
                _useStateEffectText = true;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_IS_HIDE_USER_INTERFACE))
            {
                _isHideUserInterface = GamePrefs.GetBool(GamePrefTypes.OPTION_IS_HIDE_USER_INTERFACE);
            }
            else
            {
                _isHideUserInterface = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_IS_HIDE_NPC_MARKER))
            {
                _isHideNPCMarker = GamePrefs.GetBool(GamePrefTypes.OPTION_IS_HIDE_NPC_MARKER);
            }
            else
            {
                _isHideNPCMarker = false;
            }

            if (GamePrefs.HasKey(GamePrefTypes.OPTION_USE_GAMEPLAY_TIMER))
            {
                _useGameplayTimer = GamePrefs.GetBool(GamePrefTypes.OPTION_USE_GAMEPLAY_TIMER);
            }
            else
            {
                _useGameplayTimer = false; // 기본값: 타이머 미사용
            }
        }
    }
}