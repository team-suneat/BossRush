namespace TeamSuneat.Setting
{
    public class GameCheat
    {
        private bool _isInitialized;
        private bool _isInfinityDamage;
        private bool _isPercentDamage;
        private bool _isOneDamageAttack;

        private bool _isNotCostResource;
        private bool _isNotCostPulse;
        private bool _isNoCooldownTime;
        private bool _isReceiveDamageOnlyOne;
        private bool _isNotDead;
        private bool _isNotCrowdControl;

        //

        public bool IsInfinityDamage
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return false;
                }

                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isInfinityDamage;
            }
            set
            {
                _isInfinityDamage = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_INFINITY_DAMAGE, value);
            }
        }

        public bool IsPercentDamage
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
                {
                    return false;
                }

                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isPercentDamage;
            }
            set
            {
                _isPercentDamage = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_PERCENT_DAMAGE, value);
            }
        }

        public bool IsOneDamageAttack
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isOneDamageAttack;
            }
            set
            {
                _isOneDamageAttack = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_ONE_DAMAGE_ATTACK, value);
            }
        }

        //

        public bool IsNoCooldownTime
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isNoCooldownTime;
            }
            set
            {
                _isNoCooldownTime = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NO_COOLDOWN_TIME, value);
            }
        }

        public bool IsNotCostResource
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isNotCostResource;
            }
            set
            {
                _isNotCostResource = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_RESOURCE, value);
            }
        }

        public bool IsNotCostPulse
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isNotCostPulse;
            }
            set
            {
                _isNotCostPulse = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_PULSE, value);
            }
        }

        public bool IsReceiveDamageOnlyOne
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isReceiveDamageOnlyOne;
            }
            set
            {
                _isReceiveDamageOnlyOne = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_RECEIVE_DAMAGE_ONLY_ONE, value);
            }
        }

        public bool IsNotDead
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isNotDead;
            }
            set
            {
                _isNotDead = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_DEAD, value);
            }
        }

        public bool IsNotCrowdControl
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _isNotCrowdControl;
            }
            set
            {
                _isNotCrowdControl = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_CROWD_CONTROL, value);
            }
        }

        private void Initialize()
        {
            _isInfinityDamage = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_INFINITY_DAMAGE);
            _isPercentDamage = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_PERCENT_DAMAGE);
            _isOneDamageAttack = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_ONE_DAMAGE_ATTACK);

            _isNotCostResource = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_RESOURCE);
            _isNotCostPulse = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_PULSE);
            _isNoCooldownTime = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NO_COOLDOWN_TIME);

            _isReceiveDamageOnlyOne = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_RECEIVE_DAMAGE_ONLY_ONE);
            _isNotDead = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_DEAD);
            _isNotCrowdControl = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_CROWD_CONTROL);

            _isInitialized = true;
        }
    }
}