using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        [Title("BuffEntity")]
        public BuffNames Name;

        public string NameString;

        [FoldoutGroup("#Component")] public Character Owner;
        [FoldoutGroup("#Component")] public Character Caster;
        [FoldoutGroup("#Component")] public AttackEntity Attack;

        //───────────────────────────────────────────────────────────────────────────────────────────────

        private BuffStack _buffStackRenderer;
        private Coroutine _durationCoroutine;
        private Coroutine _repetitiveCoroutine;
        private CharacterStun _characterStun;
        private Coroutine _removeStackCoroutine;
        private float _additionalDuration;

        private readonly DamageCalculator _damageInfo = new();

        //───────────────────────────────────────────────────────────────────────────────────────────────

        public int Level { get; private set; }

        public int Tick { get; private set; }

        public int Stack { get; private set; }

        public int MaxStack { get; private set; }

        public float Duration { get; private set; }

        public float IntervalTime { get; private set; }

        public float ElapsedTime { get; private set; }

        public BuffAssetData AssetData { get; set; }

        public BuffTypes Type
        {
            get
            {
                if (AssetData != null)
                {
                    return AssetData.Type;
                }

                return BuffTypes.None;
            }
        }

        public StateEffects StateEffect
        {
            get
            {
                if (AssetData != null)
                {
                    return AssetData.StateEffect;
                }

                return StateEffects.None;
            }
        }

        public StateEffects IncompatibleStateEffect
        {
            get
            {
                if (AssetData != null)
                {
                    return AssetData.IncompatibleStateEffect;
                }

                return StateEffects.None;
            }
        }

        private readonly StatNames _selectActivateStat;

        protected VProfile ProfileInfo => GameApp.GetSelectedProfile();

        public bool CallRemoveGlobalEvent { get; set; }

        private CharacterStun StunAbility => _characterStun ??= Owner.FindAbility<CharacterStun>();

        //

        //───────────────────────────────────────────────────────────────────────────────────────────────

        public void OnSpawn()
        {
            ResetValues();
        }

        private void ResetValues()
        {
            // 필드 초기화
            Level = 0;
            Tick = 0;
            Stack = 0;
            MaxStack = 0;
            IntervalTime = 0f;
            ElapsedTime = 0f;

            Duration = 0f;
            _additionalDuration = 0f;

            if (_durationCoroutine != null)
            {
                Log.Warning("이전에 사용하던 버프 지속 타이머가 중단되지 않았습니다. {0}", Name);
                _durationCoroutine = null;
            }
            if (_repetitiveCoroutine != null)
            {
                Log.Warning("이전에 사용하던 버프 반복 타이머가 중단되지 않았습니다. {0}", Name);
                _repetitiveCoroutine = null;
            }
            if (_removeStackCoroutine != null)
            {
                Log.Warning("이전에 사용하던 버프 스택 삭제 타이머가 중단되지 않았습니다. {0}", Name);
                _removeStackCoroutine = null;
            }
            if (_visualEffectOfState.Count > 0)
            {
                Log.Warning("이전에 사용하던 버프 상태 VFXObject가 초기화되지 않았습니다. {0}", Name);
                DespawnStateVFX();
            }
            if (_audioObject != null)
            {
                Log.Warning("이전에 사용하던 버프 오디오가 삭제되지 않았습니다. {0}", Name);
                _audioObject = null;
            }
            if (_buffStackRenderer != null)
            {
                Log.Warning("이전에 사용하던 버프 스택 UI가 삭제되지 않았습니다. {0}", Name);
                _buffStackRenderer = null;
            }
            ResetPreviousLevelAndStack();
            ResetAbilities();
        }

        public void OnDespawn()
        {
            StopDurationTimer();
            StopRepetitiveTimer();
            StopRemoveStackTimer();

            // 버프 독립체가 삭제될 때 스택과 지난 시간을 초기화합니다.
            Stack = 0;
            ElapsedTime = 0;

            ResetAbilities();
            DespawnStateVFX();
        }

        public void Despawn()
        {
            LogInfo("버프를 디스폰합니다.");

            // 스택 아이콘 랜더러를 비활성화합니다.
            DeactivateStackRenderers();

            // 버프의 오디오 효과를 삭제합니다.
            DespawnBuffSFX();

            // 버프 상태의 비주얼 효과를 삭제합니다.
            DespawnStateVFX();

            // 해당 버프가 삭제될 때 등록된 버프를 추가합니다.
            AddBuffOnRelease();

            // 해당 버프가 삭제될 때 등록된 Deactive 버프를 삭제합니다.
            RemoveDeactiveOrderBuffs();

            CallRemoveGlobalEvent = false;

            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject);
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary> 버프의 정보를 설정합니다. </summary>
        public void Setup(BuffAssetData assetData, Character owner, Character caster, int level)
        {
            Name = assetData.Name;
            AssetData = assetData;
            Owner = owner;
            Caster = caster;

            SetLevel(level);
            InitializeFeedbacks();

            AutoNaming();
        }

        public void SetLevel(int level)
        {
            if (level > AssetData.MaxLevel)
            {
                level = AssetData.MaxLevel;
                LogSetLevel(level);
            }

            if (Level != level)
            {
                Level = level;
            }
        }

        /// <summary> 버프를 활성화합니다. </summary>
        public void Activate()
        {
            LoadBuffMaxStackCount();            

            LogInfo("버프를 활성화합니다. 레벨: {0}, 스택: {1}/{2}", Level, Stack, MaxStack);

            SetupDuration();
            SetupIntervalTime();

            if (Tick > 0)
            {
                DeactivateTick();
            }

            if (Stack == 0 && MaxStack > 0)
            {
                LogProgress("현재 스택이 설정되지 않았다면 버프 활성화 시 스택을 추가합니다.");
                _ = AddStackCount();
            }

            if (Level == 0 && Stack == 0)
            {
                LogError("버프의 레벨과 스택이 모두 0입니다. 능력치를 갱신할 수 없습니다.");
            }

            RefreshStats();

            StartTimer();
            SpawnBuffSFX();
            ApplyAnimation();
            RemoveIncompatible();

            PlayActivateFeedbacks();
        }

        /// <summary> 버프를 비활성화 합니다. </summary>
        public void Deactivate()
        {
            LogInfo("버프를 비활성화합니다.");

            RemoveStats();
            ResetPreviousLevelAndStack();
            StartRestTimer();
            StopDurationTimer();
            StopRepetitiveTimer();
            ReleaseAnimation();
            ReleaseAbility();
            ReleaseInvulnerable();
            DeactivateAttack();

            StopActivateFeedbacks();
            StopApplyFeedbacks();
            StopOverlapFeedbacks();
            PlayDeactivateFeedbacks();
        }

        /// <summary> 버프를 적용합니다. </summary>
        public void Apply()
        {
            LogInfo("버프를 적용합니다. 버프 타입: {0}", AssetData.Type.ToLogString());
            try
            {
                switch (AssetData.Type)
                {
                    case BuffTypes.InstantDamage: // 즉각적인 피해를 줍니다.
                        {
                            if (!TryAttack())
                            {
                                ApplyDamage();
                            }
                        }
                        break;

                    case BuffTypes.StateEffect: // 상태이상을 적용합니다.
                        {
                            ApplyStateEffect();
                        }
                        break;

                    case BuffTypes.InstantHeal:  // 생명력이 즉시 회복합니다.
                    case BuffTypes.HealOverTime: // 생명력이 일정시간마다 회복합니다.
                        {
                            if (!TryAttack())
                            {
                                ApplyHealHealth();
                            }
                        }
                        break;

                    case BuffTypes.IntervalLevel:
                        {
                            int nextLevel = Level + 1;
                            if (AssetData.MaxLevel < nextLevel)
                            {
                                nextLevel = AssetData.MaxLevel;
                            }

                            SetLevel(nextLevel);
                            RefreshStats();
                        }
                        break;

                    case BuffTypes.Stat:
                        {
                            if (AssetData.SetStackByElapsedTimeOnApply)
                            {
                                if (SetStackCount(Mathf.FloorToInt(ElapsedTime)))
                                {
                                    RefreshStats();
                                }
                            }
                        }
                        break;
                }

                PlayApplyFeedbacks();
            }
            catch (Exception ex)
            {
                Log.Error($"{Name}.Apply 예외 발생: {ex.Message}");
            }
        }

        /// <summary>
        /// 반복되는 버프를 한 번에 적용합니다.
        /// </summary>
        public void ApplyAtOnce()
        {
            if (_repetitiveCoroutine != null)
            {
                float intervalTime = IntervalTime;
                float lastTime = Duration - ElapsedTime;
                float count = Mathf.FloorToInt(lastTime.SafeDivide(intervalTime));

                LogProgress("반복되는 버프의 적용을 한 번에 적용합니다. 남은 횟수: {0}", count);

                for (int i = 0; i < count; i++)
                {
                    Apply();
                }

                StopRepetitiveTimer();

                OnCompleteTimer();
            }
        }

        /// <summary>
        /// 상태이상을 적용합니다.
        /// </summary>
        private void ApplyStateEffect()
        {
            switch (AssetData.StateEffect)
            {
                case StateEffects.Burning:
                case StateEffects.Jolted:
                case StateEffects.Poisoning:
                case StateEffects.Bleeding:
                    {
                        IncreaseTick();

                        if (!TryAttack())
                        {
                            ApplyDamage();
                        }
                    }
                    break;

                case StateEffects.Stun:
                case StateEffects.Paralysis:
                case StateEffects.Freeze:
                case StateEffects.ElectricShock:
                    {
                        ApplyStunAbility();
                    }
                    break;
            }
        }

        private void IncreaseTick()
        {
            Tick += 1;

            LogProgress("{0} 의 지속피해 발생 횟수 :{1} ", AssetData.StateEffect.ToLogString(), Tick);
        }

        //────────────────────────────────────────────────────────────────────────────────

        private void ApplyStunAbility()
        {
            if (StunAbility != null)
            {
                StunAbility.StartStun(Duration);
            }
        }

        public void ReleaseStunAbility()
        {
            if (StunAbility != null)
            {
                if (StunAbility.CheckStunning())
                {
                    StunAbility.ExitStun();
                }
            }
        }

        public void ReleaseAbility()
        {
            LogProgress("버프의 상태이상({0})을 해제합니다.", AssetData.StateEffect);

            if (AssetData.StateEffect.IsCrowdControl())
            {
                StunAbility?.ExitStun();
            }
        }

        private void ResetAbilities()
        {
            _characterStun = null;
        }

        //───────────────────────────────────────────────────────────────────────────
        /// <summary>
        /// 해당 버프가 삭제될 때 등록된 버프를 추가합니다.
        /// </summary>
        private void AddBuffOnRelease()
        {
            if (AssetData.BuffOnRelease != BuffNames.None)
            {
                BuffAsset asset = ScriptableDataManager.Instance.FindBuff(AssetData.BuffOnRelease);
                if (asset != null)
                {
                    LogProgress("버프가 끝났을 때 새롭게 버프를 추가합니다. BuffOnRelease: {0}", AssetData.BuffOnRelease.ToErrorString());

                    Owner.Buff.Add(asset.Data, 1, Owner);
                }
            }
        }

        /// <summary>
        /// 해당 버프가 삭제될 때 등록된 Deactive 버프를 삭제합니다.
        /// </summary>
        private void RemoveDeactiveOrderBuffs()
        {
            if (AssetData.DeactiveBuffs.IsValidArray())
            {
                for (int i = 0; i < AssetData.DeactiveBuffs.Length; i++)
                {
                    if (Owner.Buff.ContainsKey(AssetData.DeactiveBuffs[i]))
                    {
                        Owner.Buff.Remove(AssetData.DeactiveBuffs[i]);

                        LogProgress("버프가 끝났을 때 추가로 버프를 제거합니다. BuffOnRelease: {0}", AssetData.DeactiveBuffs[i].ToErrorString());
                    }
                }
            }

            if (AssetData.DeactiveStateEffects.IsValidArray())
            {
                for (int i = 0; i < AssetData.DeactiveStateEffects.Length; i++)
                {
                    if (Owner.Buff.ContainsStateEffect(AssetData.DeactiveStateEffects[i]))
                    {
                        Owner.Buff.Remove(AssetData.DeactiveStateEffects[i]);

                        LogProgress("버프가 끝났을 때 추가로 상태이상을 제거합니다. DeactiveStateEffects: {0}", AssetData.DeactiveStateEffects[i].ToErrorString());
                    }
                }
            }
        }

        /// <summary>
        /// 호환 불가 버프를 삭제합니다.
        /// </summary>
        private void RemoveIncompatible()
        {
            if (AssetData.Incompatible != BuffNames.None)
            {
                LogProgress("해당 버프의 호환불가 버프를 삭제합니다. Incompatible: {0}", AssetData.Incompatible.ToErrorString());

                Owner.Buff.Remove(AssetData.Incompatible);
            }

            if (AssetData.IncompatibleStateEffect != StateEffects.None)
            {
                LogProgress("해당 버프의 호환불가 버프 타입에 해당하는 모든 버프를 삭제합니다. IncompatibleType: {0}", AssetData.IncompatibleStateEffect.ToErrorString());

                Owner.Buff.Remove(AssetData.IncompatibleStateEffect);
            }
        }

        #region Log

        private void LogSetLevel(int level)
        {
            if (Log.LevelProgress)
            {
                LogProgress($"버프의 레벨을 설정합니다. 레벨: {Level} ▶ {level.ToSelectString(Level)}");
            }
        }

        #endregion Log
    }
}