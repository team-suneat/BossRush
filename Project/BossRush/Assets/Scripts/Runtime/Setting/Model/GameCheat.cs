using TeamSuneat;
using TeamSuneat.Data.Game;

namespace TeamSuneat.Setting
{
    public class GameCheat
    {
        public enum CriticalTypes
        {
            None,
            Critical,
            NoCritical,
        }

        public enum TriggerChanceTypes
        {
            None,
            Pass,
            Ignore,
        }

        private bool _isInitialized;
        private bool _infinityDamage;
        private bool _percentDamage;
        private bool _oneDamageAttack;

        private CriticalTypes _criticalType;
        private TriggerChanceTypes _triggerChanceType;

        private bool _notCostResoures;
        private bool _noCooldownTime;
        private bool _receiveDamageOnlyOne;
        private bool _notDead;
        private bool _notCrowdControl;
        private bool _dontDropItem;

        private bool _useItemOptionMaxStat;
        private GradeNames _customRelicGrade;

        //

        public bool InfinityDamage
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

                return _infinityDamage;
            }
            set
            {
                _infinityDamage = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_INFINITY_DAMAGE, value);
            }
        }

        public bool PercentDamage
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

                return _percentDamage;
            }
            set
            {
                _percentDamage = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_PERCENT_DAMAGE, value);
            }
        }

        public bool OneDamageAttack
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _oneDamageAttack;
            }
            set
            {
                _oneDamageAttack = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_ONE_DAMAGE_ATTACK, value);
            }
        }

        //

        public CriticalTypes CriticalType
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return CriticalTypes.None; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _criticalType;
            }
            set
            {
                _criticalType = value;
                GamePrefs.SetInt(GamePrefTypes.GAME_CHEAT_PASSIVE_TRIGGER_CHANCE_TYPE, _criticalType.ToInt());
            }
        }

        public TriggerChanceTypes TriggerChanceType
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return TriggerChanceTypes.None; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _triggerChanceType;
            }
            set
            {
                _triggerChanceType = value;
                GamePrefs.SetInt(GamePrefTypes.GAME_CHEAT_PASSIVE_TRIGGER_CHANCE_TYPE, _triggerChanceType.ToInt());
            }
        }

        //

        public bool NoCooldownTime
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _noCooldownTime;
            }
            set
            {
                _noCooldownTime = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NO_COOLDOWN_TIME, value);
            }
        }

        public bool NotCostResoure
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _notCostResoures;
            }
            set
            {
                _notCostResoures = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_RESOURCE, value);
            }
        }

        public bool ReceiveDamageOnlyOne
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _receiveDamageOnlyOne;
            }
            set
            {
                _receiveDamageOnlyOne = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_RECEIVE_DAMAGE_ONLY_ONE, value);
            }
        }

        public bool NotDead
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _notDead;
            }
            set
            {
                _notDead = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_DEAD, value);
            }
        }

        public bool NotCrowdControl
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _notCrowdControl;
            }
            set
            {
                _notCrowdControl = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_NOT_CROWD_CONTROL, value);
            }
        }

        public bool DontDropItem
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _dontDropItem;
            }
            set
            {
                _dontDropItem = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_DONT_DROP_ITEM, value);
            }
        }

        public bool UseItemOptionMaxStat
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return false; }
                if (!_isInitialized)
                {
                    Initialize();
                }
                return _useItemOptionMaxStat;
            }
            set
            {
                _useItemOptionMaxStat = value;
                GamePrefs.SetBool(GamePrefTypes.GAME_CHEAT_ITEM_OPTION_MAX_STAT, value);
            }
        }

        public GradeNames CustomRelicGrade
        {
            get
            {
                if (!GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD) { return GradeNames.None; }
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _customRelicGrade;
            }
            set
            {
                _customRelicGrade = value;
                GamePrefs.SetInt(GamePrefTypes.GAME_CHEAT_ITEM_OPTION_MAX_STAT, _customRelicGrade.ToInt());
            }
        }

        private void Initialize()
        {
            _infinityDamage = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_INFINITY_DAMAGE);
            _percentDamage = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_PERCENT_DAMAGE);
            _oneDamageAttack = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_ONE_DAMAGE_ATTACK);

            _criticalType = GamePrefs.GetInt(GamePrefTypes.GAME_CHEAT_CRITICAL_TYPE).ToEnum<CriticalTypes>();
            _triggerChanceType = GamePrefs.GetInt(GamePrefTypes.GAME_CHEAT_PASSIVE_TRIGGER_CHANCE_TYPE).ToEnum<TriggerChanceTypes>();

            _notCostResoures = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_COST_RESOURCE);
            _noCooldownTime = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NO_COOLDOWN_TIME);

            _receiveDamageOnlyOne = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_RECEIVE_DAMAGE_ONLY_ONE);
            _notDead = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_DEAD);
            _notCrowdControl = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_NOT_CROWD_CONTROL);

            _dontDropItem = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_DONT_DROP_ITEM);
            _useItemOptionMaxStat = GamePrefs.GetBool(GamePrefTypes.GAME_CHEAT_ITEM_OPTION_MAX_STAT);
            _customRelicGrade = GamePrefs.GetInt(GamePrefTypes.GAME_CHEAT_CUSTOM_RELIC_GRADE).ToEnum<GradeNames>();

            _isInitialized = true;
        }
    }
}