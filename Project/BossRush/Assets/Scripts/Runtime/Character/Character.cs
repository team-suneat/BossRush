using Lean.Pool;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character : XBehaviour, IPoolable
    {
        protected virtual void Awake()
        {
            CharacterModel = this.FindGameObject("Model");
            Animator = this.FindComponent<Animator>("Model");
            CharacterAnimator = GetComponentInChildren<CharacterAnimator>();
            CharacterRenderer = GetComponentInChildren<CharacterRenderer>();

            Physics = GetComponent<CharacterPhysics>();

            Attack = GetComponentInChildren<AttackSystem>();
            Stat = GetComponentInChildren<StatSystem>();
            MyVital = GetComponentInChildren<Vital>();

            StateMachine = GetComponent<CharacterStateMachine>();

            BarrierPoint = this.FindTransform("Point-Barrier");
            WarningTextPoint = this.FindTransform("Point-WarningText");
            MinimapPoint = this.FindTransform("Point-Minimap");
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            if (MyVital != null)
            {
                if (MyVital.Life != null)
                {
                    LogInfo("캐릭터의 생명(Life)의 피격/부활/죽음/죽임 이벤트를 모두 해제합니다.");

                    MyVital.Life.UnregisterOnDamageEvent(OnDamage);
                    MyVital.Life.UnregisterOnReviveEvent(OnRevive);
                    MyVital.Life.UnregisterOnDeathEvent(OnDeath);
                    MyVital.Life.UnregisterOnKilledEvent(OnKilled);
                }
            }
        }

        public virtual void OnSpawn()
        {
        }

        public virtual void OnDespawn()
        {
            ResetTarget();

            if (MyVital != null)
            {
                MyVital.UnregisterVital();
            }

            IsBattleReady = false;
        }

        public void Despawn(float delayTime = 0)
        {
            LogInfo("(SID:{0}) 지연된 캐릭터를 제거합니다.", SID.ToSelectString());

            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject, delayTime);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            RegisterVitalEvents();
        }

        protected virtual void RegisterVitalEvents()
        {
            if (MyVital != null)
            {
                if (MyVital.Life != null)
                {
                    LogInfo("캐릭터의 생명(Life)의 데미지/부활/죽음/킬 이벤트를 등록합니다.");

                    MyVital.Life.RegisterOnDamageEvent(OnDamage);
                    MyVital.Life.RegisterOnReviveEvent(OnRevive);
                    MyVital.Life.RegisterOnDeathEvent(OnDeath);
                    MyVital.Life.RegisterOnKilledEvent(OnKilled);
                }
            }
        }

        #region Initialization

        public virtual void Initialize()
        {
            LogInfo("새로운 캐릭터의 상태를 초기화합니다.");

            LoadCharacterData();

            AddDefaultStats();
            AddCharacterStats();
            InitializeStateMachines();

            AssignAnimator();
            UpdateAnimators();
            PlaySpawnAnimation();

            UnlockFlip();
            Attack?.Initialize();
            CharacterRenderer?.ResetRenderer();

            _ = CoroutineNextFrame(BattleReady);
        }

        private void LoadCharacterData()
        {
            AssetData = ScriptableDataManager.Instance.FindCharacterClone(Name);
            if (!AssetData.IsValid())
            {
                Log.Error("캐릭터 데이터를 찾을 수 없습니다. {0}", Name.ToString());
                return;
            }
        }

        private void InitializeStateMachines()
        {
            // CharacterStateMachine은 하위 클래스에서 초기화
            LogInfo("캐릭터의 상태머신을 초기화합니다.");
        }

        public virtual void BattleReady()
        {
            LogInfo("전투를 준비합니다.");

            Attack?.OnBattleReady();
            MyVital?.OnBattleReady();
            OnBattleFeedbacks?.PlayFeedbacks();
        }

        #endregion Initialization

        protected virtual void OnDamage(DamageResult damageResult)
        {
        }

        protected virtual void OnDeath(DamageResult damageResult)
        {
            LogInfo("캐릭터의 현재 생명력이 0이 되었습니다.");

            Attack?.OnDeath();
            Stat?.Clear();

            if (StateMachine != null)
            {
                StateMachine.ChangeState(CharacterState.Dead);
            }
        }

        protected virtual void OnKilled(Character attacker)
        {
        }

        protected virtual void OnRevive()
        {
        }

        #region Update

        public virtual void LogicUpdate()
        {
            if (!Time.timeScale.IsZero())
            {
            }

            UpdateAnimators();
        }

        public virtual void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            Physics?.PhysicsTick();
        }

        #endregion Update

        protected virtual void RefreshGameLayer()
        {
            tag = GameTags.Character.ToString();

            LogInfo("캐릭터의 게임레이어 태그 레이어 마스크를 설정합니다.");
        }

        public virtual void AddCharacterStats()
        { }

        private void AddDefaultStats()
        {
            StatNames[] statNames = EnumEx.GetValues<StatNames>();
            StatData statData;
            for (int i = 1; i < statNames.Length; i++)
            {
                statData = JsonDataManager.FindStatData(statNames[i]);
                if (!statData.IsValid()) { continue; }

                Stat.AddWithSourceInfo(statNames[i], 0, this, NameString, "Character");
            }
        }

        #region 상태 (State)

        public void ChangeState(CharacterState newState)
        {
            if (StateMachine != null)
            {
                StateMachine.ChangeState(newState);
            }
        }

        public bool CompareState(CharacterState state)
        {
            return StateMachine != null && StateMachine.CurrentState == state;
        }

        public bool CompareState(CharacterState[] states)
        {
            if (StateMachine == null) return false;

            foreach (CharacterState state in states)
            {
                if (StateMachine.CurrentState == state)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void ExitCrwodControlToState()
        {
            if (StateMachine != null && StateMachine.CurrentState == CharacterState.Stunned)
            {
                StateMachine.ChangeState(CharacterState.Idle);
            }
        }

        #endregion 상태 (State)

        #region 데미지 시 (On Damage)

        public virtual void ApplyDamageFV(DamageResult damageResult)
        {
        }

        #endregion 데미지 시 (On Damage)
    }
}