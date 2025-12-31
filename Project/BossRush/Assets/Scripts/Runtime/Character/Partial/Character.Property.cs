using System.Collections.Generic;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        public int Level { get; protected set; } = 1;

        public SID SID => MyVital.SID;

        public bool IsPlayer => this is PlayerCharacter;

        public bool IsBoss => this is BossCharacter;

        // Target

        public virtual Transform Target => null;

        public Character TargetCharacter { get; protected set; }

        // Vital

        public int CurrentHealth => MyVital.CurrentHealth;

        public int MaxHealth => MyVital.MaxHealth;

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

        public bool IgnoreCrowdControl { get; set; }

        public bool BlockDropSpawn { get; set; }

        public bool BlockDropEXP { get; set; }

        public HashSet<int> AnimatorParameters { get; set; } = new HashSet<int>();

        public Vector3 DamageDirection { get; protected set; }

        //

        public bool IsCrowdControl
        {
            get
            {
                if (ConditionState.CurrentState is CharacterConditions.Stunned)
                {
                    return true;
                }

                return false;
            }
        }

        public VProfile ProfileInfo => GameApp.GetSelectedProfile();

        public virtual LogTags LogTag => LogTags.Character;
    }
}