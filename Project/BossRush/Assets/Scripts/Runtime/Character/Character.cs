using Lean.Pool;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Character : XBehaviour, IPoolable
    {
        protected override void OnRelease()
        {
            base.OnRelease();

            if (MyVital != null)
            {
                if (MyVital.Health != null)
                {
                    LogInfo("캐릭터의 생명(Health)의 피격/부활/죽음/죽임 이벤트를 모두 해제합니다.");

                    MyVital.Health.UnregisterOnDamageEvent(OnDamage);
                    MyVital.Health.UnregisterOnReviveEvent(OnRevive);
                    MyVital.Health.UnregisterOnDeathEvent(OnDeath);
                    MyVital.Health.UnregisterOnKilledEvent(OnKilled);
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
                if (MyVital.Health != null)
                {
                    LogInfo("캐릭터의 생명(Health)의 데미지/부활/죽음/킬 이벤트를 등록합니다.");

                    MyVital.Health.RegisterOnDamageEvent(OnDamage);
                    MyVital.Health.RegisterOnReviveEvent(OnRevive);
                    MyVital.Health.RegisterOnDeathEvent(OnDeath);
                    MyVital.Health.RegisterOnKilledEvent(OnKilled);
                }
            }
        }

        #region Initialization

        public virtual void Initialize()
        {
            LogInfo("새로운 캐릭터의 상태를 초기화합니다.");

            AddDefaultStats();
            AddCharacterStats();
            InitializeStateMachines();

            AssignAnimator();
            InitializeAbilities();
            ChangeConditionState(CharacterConditions.Normal);

            UpdateAnimators();

            Attack?.Initialize();
            CharacterRenderer?.ResetRenderer();

            _ = CoroutineNextFrame(BattleReady);
        }

        private void InitializeStateMachines()
        {
            ConditionState = new StateMachine<CharacterConditions>(SendStateChangeEvents);

            LogInfo("캐릭터의 상태를 초기화합니다. (Condition)");
        }

        public virtual void BattleReady()
        {
            LogInfo("전투를 준비합니다.");

            Attack?.OnBattleReady();
            MyVital?.OnBattleReady();
            MyVital?.SetHUD();
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
            Passive?.Clear();
            Buff?.Clear();
            Stat?.Clear();

            ResetAbilities();
            ChangeConditionState(CharacterConditions.Dead);
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
            EarlyProcessAbilities();

            if (!Time.timeScale.IsZero())
            {
                ProcessAbilities();
                LateProcessAbilities();
            }

            UpdateAnimators();

            Passive?.LogicUpdate();
        }

        public virtual void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            PhysicsProcessAbilities();
        }

        #endregion Update



        protected virtual void RefreshGameLayer()
        {
            tag = GameTags.Character.ToString();

            LogInfo("캐릭터의 게임레이어 태그 레이어 마스크를 설정합니다.");
        }

        public virtual void SetupLevel()
        {
            // 아무것도 구현되지 않았습니다.
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

        #region 상태 (State) : 조건

        public void ChangeConditionState(CharacterConditions conditionState)
        {
            if (ConditionState.CurrentState != conditionState)
            {
                LogInfo("캐릭터의 조건 상태를 변경합니다. {0} 에서 {1}", ConditionState.CurrentState.ToString(), conditionState.ToString());

                ConditionState.ChangeState(conditionState);
            }
        }

        public bool CompareConditionState(CharacterConditions conditionState)
        {
            return ConditionState.CurrentState == conditionState;
        }

        public bool CompareConditionState(CharacterConditions[] conditionStates)
        {
            foreach (CharacterConditions conditionState in conditionStates)
            {
                return ConditionState.CurrentState == conditionState;
            }

            return false;
        }

        public virtual void ExitCrwodControlToState()
        {
        }

        #endregion 상태 (State) : 조건

        #region 데미지 시 (On Damage)

        public virtual void ApplyDamageFV(DamageResult damageResult)
        {
        }

        public virtual void ApplyDamageBuff(DamageResult damageResult)
        {
        }

        #endregion 데미지 시 (On Damage)
    }
}