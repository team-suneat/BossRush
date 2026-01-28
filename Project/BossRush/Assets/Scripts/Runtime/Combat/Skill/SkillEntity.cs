using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour
    {
        //----------------------------------------------------------------------------------------

        [FoldoutGroup("#SkillEntity")]
        public SkillName Name;

        [FoldoutGroup("#SkillEntity")]
        public string NameString;

        [FoldoutGroup("#SkillEntity")]
        [SuffixLabel("타겟을 오너 캐릭터의 타겟으로 자동 설정합니다.")]
        public bool AutoSetTargetToCharacterTarget;

        [FoldoutGroup("#SkillEntity")]
        [SuffixLabel("타겟을 플레이어 캐릭터로 자동 설정합니다.")]
        public bool AutoSetTargetToPlayer;

        //----------------------------------------------------------------------------------------

        public Character Owner { get; private set; }
        public Vital Vital { get; private set; }
        public SkillAssetData AssetData { get; private set; }
        public Vital TargetVital { get; private set; }

        public bool IsActive { get; private set; }
        public int Level { get; private set; }

        public bool IsOnCooldown => _cooldownTimer > 0f;

        private float _cooldownTimer;

        public override void AutoSetting()
        {
            base.AutoSetting();

            NameString = Name.ToString();
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoNaming()
        {
            if (Name != SkillName.None)
            {
                SetGameObjectName(Name.ToString());
            }
        }

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<Character>();
            if (Owner != null)
            {
                Vital = Owner.MyVital;
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void Initialization()
        {
            LogInfo("스킬 독립체를 초기화합니다.");

            InitializeFeedbacks();
            LoadAssetData();

            if (!AssetData.IsValid())
            {
                LogError("스킬 정보에서 스킬 데이터를 읽어올 수 없습니다.");
            }

            _cooldownTimer = 0f;
        }

        private void LoadAssetData()
        {
            if (Name != SkillName.None)
            {
                AssetData = ScriptableDataManager.Instance.FindSkillClone(Name);
                if (AssetData.IsValid())
                {
                    LogProgress("스킬 독립체의 스킬 에셋을 읽어왔습니다. {0}", AssetData.Name.ToLogString());
                }
                else
                {
                    LogError("스킬 독립체의 스킬 에셋이 설정되지 않았습니다. {0}", this.GetHierarchyPath());
                }
            }
            else
            {
                LogError("스킬 독립체의 스킬 이름이 설정되지 않았습니다. {0}", this.GetHierarchyPath());
            }
        }

        public virtual void OnBattleReady()
        {
            if (AssetData == null)
            {
                return;
            }

            if (AssetData.TriggerType == SkillTriggerType.Conditional)
            {
                RegisterConditionalTriggers();
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void SetOwner(Character ownerCharacter)
        {
            Owner = ownerCharacter;
            if (Owner != null)
            {
                Vital = Owner.MyVital;
            }
        }

        public virtual void SetLevel(int level)
        {
            Level = level;
            LogInfo("스킬 독립체의 레벨을 설정합니다. 레벨: {0}", level);
        }

        public virtual void SetTarget(Vital targetVital)
        {
            TargetVital = targetVital;
            LogInfo("스킬 독립체의 타겟 바이탈을 설정합니다. {0}", targetVital.GetHierarchyName());
        }

        public virtual void AutoSetTarget()
        {
            if (AutoSetTargetToPlayer)
            {
                if (CharacterManager.Instance.Player != null)
                {
                    TargetVital = CharacterManager.Instance.Player.MyVital;
                }
            }
            else if (AutoSetTargetToCharacterTarget)
            {
                if (Owner != null && Owner.TargetCharacter != null)
                {
                    TargetVital = Owner.TargetCharacter.MyVital;
                }
            }
        }

        //----------------------------------------------------------------------------------------

        private bool IsActiveType()
        {
            return AssetData != null && AssetData.Type == SkillType.Active;
        }

        private bool IsOnAcquireTrigger()
        {
            return AssetData != null && AssetData.TriggerType == SkillTriggerType.OnAcquire;
        }

        private bool IsConditionalTrigger()
        {
            return AssetData != null && AssetData.TriggerType == SkillTriggerType.Conditional;
        }

        public virtual void OnAcquire()
        {
            // OnAcquire 트리거 타입일 때 버프 및 패시브 적용
            if (IsOnAcquireTrigger() && AssetData != null)
            {
                ApplyOnAcquireBuff();
                ApplyOnAcquirePassive();
            }
        }

        public virtual void RegisterConditionalTriggers()
        {
            // 자식 클래스에서 구현합니다.
            // 조건부 발동 스킬의 이벤트 구독/등록을 처리합니다.
        }

        public virtual void UnregisterConditionalTriggers()
        {
            // 자식 클래스에서 구현합니다.
            // 조건부 발동 스킬의 이벤트 구독 해제를 처리합니다.
        }

        public virtual bool TryActivate()
        {
            // 패시브 스킬은 기본적으로 수동 활성화를 허용하지 않습니다.
            if (!IsActiveType())
            {
                LogProgress("스킬을 활성화할 수 없습니다. (패시브 스킬)");
                return false;
            }
            if (IsOnCooldown)
            {
                LogProgress("스킬을 활성화할 수 없습니다. (쿨타임 대기중)");
                return false;
            }
            if (AssetData != null && AssetData.UseResourceValue > 0f)
            {
                if (Owner == null || Owner.MyVital == null)
                {
                    LogProgress("스킬을 활성화할 수 없습니다. (소유자 또는 바이탈이 null)");
                    return false;
                }

                VitalConsumeTypes resourceType = AssetData.ResourceConsumeType;
                if (!Owner.MyVital.CanUseOrNotify(resourceType, AssetData.UseResourceValue))
                {
                    LogProgress("스킬을 활성화할 수 없습니다. (자원 부족)");
                    return false;
                }
            }

            return true;
        }

        public virtual void Activate()
        {
            LogInfo("스킬 독립체를 활성화합니다.");

            PlayCastAnimation();
            AutoSetTarget();
            TriggerSkillStartFeedback();
            ApplyForceVelocity();

            OnActivate();

            IsActive = true;
        }

        protected void OnActivate()
        {
            if (AssetData.IsValid())
            {
                if (AssetData.Type == SkillType.Active && AssetData.UseResourceOnActivate)
                {
                    StartUseAndRestoreResource();
                }
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void Deactivate()
        {
            LogInfo("스킬 독립체를 비활성화합니다.");

            if (IsActive)
            {
                TriggerSkillStopFeedback();
            }

            StopSkillStartFeedback();
            IsActive = false;

            StopUseAndRestoreResource();

            // 쿨타임 시작
            if (AssetData != null && AssetData.Type == SkillType.Active && AssetData.Cooldown > 0f)
            {
                _cooldownTimer = AssetData.Cooldown;
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void Apply()
        {
            TriggerSkillUsedFeedback();

            if (AssetData.IsValid())
            {
                if (AssetData.Type == SkillType.Active && AssetData.UseResourceOnApply)
                {
                    StartUseAndRestoreResource();
                }
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void LogicUpdate()
        {
            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer < 0f)
                {
                    _cooldownTimer = 0f;
                }
            }
        }

        /// <summary> 소유자가 사망했을 시 </summary>
        public virtual void OnOwnerDeath()
        {
            // ForceVelocity 중지
            if (Owner != null && Owner.Physics != null)
            {
                Owner.Physics.StopForceVelocity();
            }

            // 스킬 비활성화
            if (IsActive)
            {
                Deactivate();
            }
        }

        public virtual void Despawn()
        {
            if (IsActive)
            {
                Deactivate();
            }

            if (IsConditionalTrigger())
            {
                UnregisterConditionalTriggers();
            }

            // OnAcquire 트리거 타입일 때 버프 및 패시브 해제
            if (IsOnAcquireTrigger() && AssetData != null)
            {
                RemoveOnAcquireBuff();
                RemoveOnAcquirePassive();
            }

            ResourcesManager.Despawn(gameObject);
        }

        private void PlayCastAnimation()
        {
            if (Owner != null && Owner.CharacterAnimator != null)
            {
                Owner.CharacterAnimator.PlayCastAnimation(Name);
            }
        }

        private void ApplyForceVelocity()
        {
            if (!IsActiveType())
            {
                return;
            }

            if (Owner == null || Owner.Physics == null)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity를 적용할 수 없습니다. Owner 또는 Physics가 null입니다.");
                return;
            }

            if (AssetData == null || AssetData.SkillFVName == FVNames.None)
            {
                return;
            }

            // ForceVelocity 데이터 가져오기
            ForceVelocityAssetData forceVelocityData = ScriptableDataManager.Instance.FindForceVelocityClone(AssetData.SkillFVName);
            if (forceVelocityData == null)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity 데이터를 찾을 수 없습니다. {0}", AssetData.SkillFVName.ToLogString());
                return;
            }

            // 캐릭터가 바라보는 방향 확인
            bool isFacingRight = Owner.Physics != null && Owner.Physics.FacingDirection > 0;

            Log.Info(LogTags.Physics, "스킬에서 ForceVelocity를 적용합니다. {0}, 방향: {1}", forceVelocityData.Name.ToLogString(), isFacingRight ? "Right" : "Left");

            // ForceVelocity 적용
            Owner.Physics.StartForceVelocity(forceVelocityData, isFacingRight, this);
        }

        private void ApplyOnAcquireBuff()
        {
            if (AssetData == null || AssetData.BuffName == BuffName.None)
            {
                return;
            }

            if (Owner == null || Owner.Buff == null)
            {
                Log.Warning(LogTags.Skill, "버프를 적용할 수 없습니다. Owner 또는 Buff 시스템이 null입니다.");
                return;
            }

            Owner.Buff.Add(AssetData.BuffName, 1, Owner);
            Log.Info(LogTags.Skill, "{0}에게 스킬 버프를 적용했습니다: {1}", Owner.Name.ToLogString(), AssetData.BuffName.ToLogString());
        }

        private void RemoveOnAcquireBuff()
        {
            if (AssetData == null || AssetData.BuffName == BuffName.None)
            {
                return;
            }

            if (Owner == null || Owner.Buff == null)
            {
                return;
            }

            Owner.Buff.Remove(AssetData.BuffName);
            Log.Info(LogTags.Skill, "{0}에서 스킬 버프를 제거했습니다: {1}", Owner.Name.ToLogString(), AssetData.BuffName.ToLogString());
        }

        private void ApplyOnAcquirePassive()
        {
            if (AssetData == null || AssetData.PassiveName == PassiveName.None)
            {
                return;
            }

            Log.Info(LogTags.Skill, "{0}에게 스킬 패시브 적용 준비됨 (구현 예정): {1}", Owner?.Name.ToLogString() ?? "Unknown", AssetData.PassiveName.ToLogString());
        }

        private void RemoveOnAcquirePassive()
        {
            if (AssetData == null || AssetData.PassiveName == PassiveName.None)
            {
                return;
            }

            Log.Info(LogTags.Skill, "{0}에서 스킬 패시브 해제 준비됨 (구현 예정): {1}", Owner?.Name.ToLogString() ?? "Unknown", AssetData.PassiveName.ToLogString());
        }
    }
}