using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.Passive
{
    /// <summary>
    /// 패시브가 적용한 효과들의 플래그입니다.
    /// </summary>
    [Flags]
    public enum AppliedEffects
    {
        None = 0,
        BuffAdd = 1 << 0,      // 효과 추가 (버프/디버프 등)
        BuffRemove = 1 << 1,   // 효과 제거
        Cooldown = 1 << 2,
        Attack = 1 << 3,
        Duration = 1 << 4,
        Reward = 1 << 5,
        ForceVelocity = 1 << 6,
        All = BuffAdd | BuffRemove | Cooldown | Attack | Duration | Reward | ForceVelocity
    }

    public partial class PassiveEntity : XBehaviour, IPoolable
    {
        [Title("#PassiveEntity")]
        public PassiveNames Name;

        public string NameString;

        [NonSerialized] public PassiveAsset Asset;
        public PassiveTriggerSettings TriggerSettings => Asset.TriggerSettings;
        public PassiveConditionSettings ConditionSettings => Asset.ConditionSettings;
        public PassiveEffectSettings EffectSettings => Asset.EffectSettings;

        [Tooltip("생성 시 패시브 활성화 여부를 결정합니다.")]
        public bool InitActivate;

        public Character Owner;

        [Title("#PassiveEntity", "Attack")]
        [Tooltip("무기 인덱스에 따라 공격 독립체를 검색합니다.")]
        public bool SearchAttackEntityByWeaponIndex;

        [Tooltip("캐릭터의 상태에 따라 공격 독립체를 검색합니다.")]
        public bool SearchAttackEntityByCharacterState;

        private bool _isSpawned;
        private ForceVelocityAsset _forceVelocityAsset;
        private HitmarkAssetData[] _hitmarkAssetDatas;
        private BuffAssetData[] _buffAssetDatas;

        /// <summary>
        /// 패시브가 현재 RestTime 상태인지 확인합니다.
        /// PassiveSystem의 RestTimeController를 통해 관리됩니다.
        /// </summary>
        public bool IsResting => Owner.Passive.IsPassiveResting(Name);

        public int Level { get; private set; }

        /// <summary>
        /// 패시브가 적용한 효과들의 플래그입니다.
        /// </summary>
        public AppliedEffects AppliedEffects { get; private set; }

        private VProfile ProfileInfo => GameApp.GetSelectedProfile();

        // 패시브의 생성 및 삭제

        public void OnSpawn()
        {
            _isSpawned = true;
        }

        public void OnDespawn()
        {
            _isSpawned = false;

            // RestTime은 PassiveSystem에서 관리되므로 여기서는 제거하지 않음
            // StopRestTimer();
        }

        public void Despawn()
        {
            Deactivate();
            RemoveApplyBuff(EffectSettings);

            if (!_isSpawned)
            {
                return;
            }

            if (IsDestroyed)
            {
                return;
            }

            ResourcesManager.Despawn(gameObject);
        }

        // 패시브 값 설정

        public void SetOwner(Character owner)
        {
            Owner = owner;
        }

        public void SetLevel(int level)
        {
            Level = level;
            SetBuffLevel(level);
            LogSetLevel();
        }

        public void SetAsset(PassiveAsset asset)
        {
            if (asset.IsValid())
            {
                Asset = asset;

                if (TriggerSettings == null)
                {
                    Log.Error("패시브의 발동(Trigger) 에셋 데이터를 찾을 수 없습니다: {0}", Name.ToLogString());
                }

                if (EffectSettings == null)
                {
                    Log.Error("패시브의 효과(Effect) 에셋 데이터를 찾을 수 없습니다: {0}", Name.ToLogString());
                }

                LoadBuffAsset();
                LoadHitmarkAsset();
                LoadForceVelocity();
            }
            else
            {
                LogNotValidAsset();
            }
        }

        // 패시브 활성화

        public void Activate()
        {
            LogActivate();

            // 패시브의 트리거를 1 프레임 뒤에 등록합니다.
            StartXCoroutine(RegisterTriggers());
        }

        // 패시브 비활성화

        public void Deactivate()
        {
            LogDeactivate();

            // 패시브 매니저에서 자기 자신을 제거
            PassiveManager.Instance.RemoveExecutionsByEntity(this);

            ResetBuffAssets();
            ResetHitmarkAsset();
            ResetForceVelocity();

            ResetTriggerCount();
            UnregisterTriggers();
        }

        // 패시브 실행

        public bool TryExecute(PassiveTrigger triggerInfo)
        {
            if (IsResting)
            {
                LogProgress("패시브 작동을 실패했습니다. 패시브가 유휴상태입니다.");
                return false;
            }
            if (IsFullApplyCount)
            {
                LogProgress("패시브 작동을 실패했습니다. 패시브 적용 횟수가 최대에 도달했습니다.");
                return false;
            }
            if (!Owner.IsAlive)
            {
                LogProgress("패시브 작동을 실패했습니다. 오너 캐릭터가 사망했습니다.");
                return false;
            }
           

            return true;
        }

        public void Execute(float delayTime, PassiveTrigger triggerInfo)
        {
            PassiveManager.Instance.RegisterExecute(delayTime, triggerInfo.Clone(), this);
        }

        public void ExecuteExtern(PassiveTrigger triggerInfo)
        {
            Execute(triggerInfo);
        }

        private void Execute(PassiveTrigger triggerInfo)
        {
            // 1. 효과 적용 여부 초기화
            AppliedEffects = AppliedEffects.None;

            // 2. 실행 초기화
            InitializeExecution(triggerInfo);

            // 3. 대상 캐릭터 결정
            Character targetCharacter = GetTargetCharacterForExecute(triggerInfo);

            // 4. 효과 적용
            ApplyEffects(EffectSettings, targetCharacter, triggerInfo);

            // 5. 보상 및 기타 효과
            ApplyRewardsAndUtilities(EffectSettings, triggerInfo);

            // 6. 실행 후처리
            FinalizeExecution(targetCharacter, triggerInfo);
        }

        #region Execute Methods

        /// <summary>
        /// 패시브 실행을 초기화합니다.
        /// </summary>
        /// <param name="triggerInfo">트리거 정보</param>
        private void InitializeExecution(PassiveTrigger triggerInfo)
        {
            LogExecute(triggerInfo);
            StartRestTimer(EffectSettings);
        }

        /// <summary>
        /// 패시브의 효과를 적용합니다.
        /// </summary>
        /// <param name="effectSettings">효과 설정</param>
        /// <param name="targetCharacter">대상 캐릭터</param>
        /// <param name="triggerInfo">트리거 정보</param>
        private void ApplyEffects(PassiveEffectSettings effectSettings, Character targetCharacter, PassiveTrigger triggerInfo)
        {
            // 버프 적용/제거
            AddBuff(effectSettings, targetCharacter, triggerInfo.DamagePosition);
            RemoveBuff(effectSettings, targetCharacter);

            // 데미지 오버타임 적용
            ApplyDamageOverTimeAtOnce(effectSettings, targetCharacter);

            // 공격 실행
            ExecuteAttacks(effectSettings, triggerInfo);

            // 강제 이동 적용
            ApplyForceVelocity();
        }

        /// <summary>
        /// 보상 및 기타 효과를 적용합니다.
        /// </summary>
        /// <param name="effectSettings">효과 설정</param>
        /// <param name="triggerInfo">트리거 정보</param>
        private void ApplyRewardsAndUtilities(PassiveEffectSettings effectSettings, PassiveTrigger triggerInfo)
        {
            // 쿨다운 감소
            ReduceCooldown(effectSettings, triggerInfo);

            // 보상 획득
            EarnReward(effectSettings);

        }

        /// <summary>
        /// 패시브 실행을 완료합니다.
        /// </summary>
        /// <param name="targetCharacter">대상 캐릭터</param>
        private void FinalizeExecution(Character targetCharacter, PassiveTrigger triggerInfo)
        {
            // 적용 횟수 관리
            if (TryAddApplyCount(EffectSettings))
            {
                // 최대 적용 횟수 도달시,
                ResetApplyCount(EffectSettings);
                StartRestApplyMaxCount(EffectSettings.ResetTimeApplyMaxCount);
            }

            // 트리거 카운트 리셋
            ResetTriggerCount();

            // 지속시간 적용
            ApplyAddDuration(targetCharacter);


            // 글로벌 이벤트 전송
            SendExecuteGlobalEvent();

            // 실행 결과 확인 및 처리
            if (AppliedEffects != AppliedEffects.None)
            {
                LogAppliedEffects();
            }
            else
            {
                Log.Error("패시브를 발동하였으나 실제 적용하지 못했습니다. {0}", Name.ToLogString());
            }
        }

        #endregion Execute Methods

        //

        private void SendExecuteGlobalEvent()
        {
            GlobalEvent<PassiveNames>.Send(GlobalEventType.PLAYER_CHARACTER_EXECUTE_PASSIVE, Name);
        }

        /// <summary>
        /// 적용된 효과들을 로깅합니다.
        /// </summary>
        private void LogAppliedEffects()
        {
            if (AppliedEffects == AppliedEffects.None)
            {
                return;
            }

            List<string> effects = new();

            if ((AppliedEffects & AppliedEffects.BuffAdd) != 0) { effects.Add("버프 추가"); }
            if ((AppliedEffects & AppliedEffects.BuffRemove) != 0) { effects.Add("버프 제거"); }
            if ((AppliedEffects & AppliedEffects.Cooldown) != 0) { effects.Add("재사용 대기 시간 감소"); }
            if ((AppliedEffects & AppliedEffects.Attack) != 0) { effects.Add("공격"); }
            if ((AppliedEffects & AppliedEffects.Duration) != 0) { effects.Add("지속시간 증가"); }
            if ((AppliedEffects & AppliedEffects.Reward) != 0) { effects.Add("보상"); }
            if ((AppliedEffects & AppliedEffects.ForceVelocity) != 0) { effects.Add("강제 이동"); }

            LogInfo("패시브가 적용한 효과: {0}", string.Join(", ", effects));
        }

    
    }
}