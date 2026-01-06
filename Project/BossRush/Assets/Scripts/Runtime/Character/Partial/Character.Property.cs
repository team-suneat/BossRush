using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        // Component

        public GameObject CharacterModel { get; set; }
        public Animator Animator { get; set; }
        public CharacterAnimator CharacterAnimator { get; set; }
        public CharacterRenderer CharacterRenderer { get; set; }
        public CharacterPhysics Physics { get; set; }
        public AttackSystem Attack { get; set; }
        public StatSystem Stat { get; set; }
        public Vital MyVital { get; set; }
        public CharacterStateMachine StateMachine { get; set; }

        public Transform BarrierPoint { get; set; }
        public Transform WarningTextPoint { get; set; }
        public Transform MinimapPoint { get; set; }

        //

        public SID SID => MyVital.SID;

        public bool IsPlayer => this is PlayerCharacter;

        public bool IsBoss => this is MonsterCharacter monsterCharacter && monsterCharacter.IsBoss;

        // Target

        public virtual Transform Target => null;

        public Character TargetCharacter { get; protected set; }

        // Vital

        public int CurrentLife => MyVital.CurrentLife;

        public int MaxLife => MyVital.MaxLife;

        public int CurrentShield => MyVital.CurrentShield;

        public int MaxShield => MyVital.MaxShield;

        public bool IsAlive => MyVital != null && MyVital.IsAlive;

        //

        private bool _isBattleReady;

        public bool IsBattleReady
        {
            get
            {
                return _isBattleReady;
            }
            set
            {
                if (_isBattleReady != value)
                {
                    _isBattleReady = value;
                }

                if (value)
                {
                    if (IsPlayer)
                    {
                        GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY);
                    }
                    else
                    {
                        GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_SPAWNED, this);
                    }
                }
            }
        }

        public HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

        public bool IsFacingRight
        {
            get
            {
                if (CharacterModel != null)
                {
                    return CharacterModel.transform.localScale.x > 0;
                }

                return localScale.x > 0;
            }
        }

        private bool _canFlip;

        public bool CanFlip
        {
            get => _canFlip;
            set
            {
                if (_canFlip != value)
                {
                    _canFlip = value;
                    if (value)
                    {
                        LogInfo("캐릭터가 반전할 수 있습니다. {0}", value.ToBoolString());
                    }
                    else
                    {
                        LogInfo("캐릭터가 반전할 수 없습니다. {0}", value.ToBoolString());
                    }
                }
            }
        }

        //
        public bool IsBlockInput { get; set; }

        public CharacterAssetData AssetData { get; protected set; }

        public virtual LogTags LogTag => LogTags.Character;
    }
}