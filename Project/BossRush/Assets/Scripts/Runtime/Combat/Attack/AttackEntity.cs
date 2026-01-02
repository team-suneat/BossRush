using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Passive;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        //----------------------------------------------------------------------------------------

        [FoldoutGroup("#AttackEntity")]
        public HitmarkNames Name;

        [FoldoutGroup("#AttackEntity")]
        public string NameString;

        [FoldoutGroup("#AttackEntity")]
        public Character Owner;

        [FoldoutGroup("#AttackEntity")]
        public Projectile OwnerProjectile;

        [FoldoutGroup("#AttackEntity")]
        public Vital Vital;

        [FoldoutGroup("#AttackEntity")]
        public int Index;

        [FoldoutGroup("#AttackEntity")]
        [SuffixLabel("타겟을 오너 캐릭터의 타겟으로 자동 설정합니다.")]
        public bool AutoSetTargetToCharacterTarget;

        [FoldoutGroup("#AttackEntity")]
        [SuffixLabel("타겟을 플레이어 캐릭터로 자동 설정합니다.")]
        public bool AutoSetTargetToPlayer;

        //----------------------------------------------------------------------------------------

        protected DamageCalculator _damageInfo = new();

        public HitmarkAssetData AssetData { get; private set; }

        public SkillAssetData SkillAssetData { get; private set; }

        public Vital TargetVital { get; private set; }

        public bool IsActive { get; private set; }

        public int Level => _damageInfo.Level;

        public AttackEntityTypes EntityType
        {
            get
            {
                if (this is AttackTargetEntity)
                {
                    return AttackEntityTypes.Target;
                }
                else if (this is AttackAreaEntity)
                {
                    return AttackEntityTypes.Area;
                }
                else if (this is AttackProjectileEntity)
                {
                    return AttackEntityTypes.Projectile;
                }

                return AttackEntityTypes.None;
            }
        }

        // 공격 적용 횟수
        protected int ApplyCount;

        //----------------------------------------------------------------------------------------

        public virtual void Initialization()
        {
            LogInfo("공격 독립체를 초기화합니다.");

            InitializeFeedbacks();
            LoadAssetData();

            _damageInfo.HitmarkAssetData = AssetData;
            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("버프의 피해 정보에서 히트마크 정보를 읽어올 수 없습니다:{0}", Name.ToLogString());
            }

            _damageInfo.SetAttacker(Owner);
            _damageInfo.AttackEntity = this;
            _damageInfo.AttackProjectile = OwnerProjectile;

            ApplyCount = 0;
        }

        private void LoadAssetData()
        {
            if (Name != HitmarkNames.None)
            {
                AssetData = ScriptableDataManager.Instance.FindHitmarkClone(Name);
                if (AssetData.IsValid())
                {
                    SkillAssetData = ScriptableDataManager.Instance.FindSkill(AssetData.SkillName);
                    LogErrorOnLoadAssetData();
                }
                else
                {
                    Log.Error("공격 독립체의 히트마크 에셋이 설정되지 않았습니다. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
                }
            }
            else
            {
                Log.Error("공격 독립체의 히트마크 이름이 설정되지 않았습니다. {0}", this.GetHierarchyPath());
            }
        }

        private void LogErrorOnLoadAssetData()
        {
            switch (AssetData.EntityType)
            {
                case AttackEntityTypes.Target:
                    if (this is AttackAreaEntity)
                    {
                        Log.Error("영역(Area) 공격 독립체의 독립체 타입이 에셋 데이터에 올바르게 설정되지 않았습니다. EntityType: {0}, Name: {1}, ", AssetData.EntityType, Name.ToLogString());
                    }
                    else if (this is AttackProjectileEntity)
                    {
                        Log.Error("발사체(Projectile) 공격 독립체의 독립체 타입이 에셋 데이터에 올바르게 설정되지 않았습니다. EntityType: {0}, Name: {1}, ", AssetData.EntityType, Name.ToLogString());
                    }
                    break;

                case AttackEntityTypes.Area:
                    if (this is AttackTargetEntity)
                    {
                        Log.Error("목표(Target) 공격 독립체의 독립체 타입이 에셋 데이터에 올바르게 설정되지 않았습니다. EntityType: {0}, Name: {1}, ", AssetData.EntityType, Name.ToLogString());
                    }
                    else if (this is AttackProjectileEntity)
                    {
                        Log.Error("발사체(Projectile) 공격 독립체의 독립체 타입이 에셋 데이터에 올바르게 설정되지 않았습니다. EntityType: {0}, Name: {1}, ", AssetData.EntityType, Name.ToLogString());
                    }
                    break;
            }
        }

        public virtual void OnBattleReady()
        {
            // 자식 클래스에서 구현합니다.
        }

        //----------------------------------------------------------------------------------------

        public virtual void SetOwner(Character ownerCharacter)
        {
            Owner = ownerCharacter;
        }

        public virtual void SetLevel(int level)
        {
            _damageInfo?.SetLevel(level);

            LogInfo("공격 독립체의 레벨을 설정합니다. 레벨:{0} ", level);
        }

        public virtual void SetStack(int stack)
        {
            _damageInfo?.SetStack(stack);

            LogInfo("공격 독립체의 스택을 설정합니다. 스택:{0} ", stack);
        }

        public virtual void SetReferenceValues(PassiveTrigger triggerInfo)
        {
            if (triggerInfo == null)
            {
                return;
            }

            _damageInfo.SetDamageRefrenceValue(triggerInfo.GetDamageValueToInt());

            if (triggerInfo.SkillName != SkillNames.None)
            {
                SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(triggerInfo.SkillName);
                if (skillData.IsValid())
                {
                    int resourceCost = skillData.GetResourceCost(Owner.MyVital, triggerInfo.Level);
                    _damageInfo.SetResourceCostRefrenceValue(resourceCost);

                    SkillEntity skillEntity = Owner.Skill.Find(triggerInfo.SkillName);
                    if (skillEntity != null)
                    {
                        _damageInfo.SetCooldownRefrenceValue(skillEntity.CooldownTime);
                    }
                }
            }

            LogInfo("{0} 패시브의 피해량 {1}을 공격 독립체의 참조값으로 설정합니다. ", triggerInfo.Name.ToLogString(), triggerInfo.DamageValue);
        }

        public virtual void SetTarget(Vital targetVital)
        {
            TargetVital = targetVital;

            LogInfo("공격 독립체의 타겟 바이탈을 설정합니다. {0}", targetVital.GetHierarchyName());
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

        public virtual bool DetermineActivate()
        {
            // 'Determine'은 실제 처리를 하지 않고 여부에 대한 결과만을 반환합니다.
            return true;
        }

        public virtual void Activate()
        {
            LogInfo("공격 독립체를 활성화합니다.");

            AutoSetTarget();
            TriggerAttackStartFeedback();

            IsActive = true;
        }

        protected void OnActivate()
        {
            if (AssetData.IsValid())
            {
                if (AssetData.UseResourceOnActivate)
                {
                    StartUseAndRestoreResource();
                }
            }
        }

        //----------------------------------------------------------------------------------------

        public virtual void Deactivate()
        {
            LogInfo("공격 독립체를 비활성화합니다.");

            if (IsActive)
            {
                TriggerAttackStopFeedback();
            }

            StopAttackStartFeedback();

            if (AssetData.IsValid())
            {
                if (AssetData.UseResourceOnDeactive)
                {
                    StartUseAndRestoreResource();
                }
            }

            IsActive = false;

            StopUseAndRestoreResource();
        }

        //----------------------------------------------------------------------------------------

        public virtual void Apply()
        {
            TriggerAttackUsedFeedback();
            StartSendExecuteAttackGlobalEvent();
            OnApply();

            ApplyCount += 1;
        }

        protected void OnAttack(bool isAttackSuccessed)
        {
            if (AssetData.IsValid())
            {
                if (AssetData.UseResourceOnAttack)
                {
                    StartUseAndRestoreResource();
                }
                else if (AssetData.UseResourceOnAttackSuccessed && isAttackSuccessed)
                {
                    StartUseAndRestoreResource();
                }
                else if (AssetData.UseResourceOnAttackFailed && !isAttackSuccessed)
                {
                    StartUseAndRestoreResource();
                }
            }
        }

        protected void OnApply()
        {
            if (AssetData.IsValid())
            {
                if (AssetData.UseResourceOnApply)
                {
                    StartUseAndRestoreResource();
                }
            }
        }

        //----------------------------------------------------------------------------------------

        /// <summary> 소유자가 사망했을 시 </summary>
        public virtual void OnOwnerDeath()
        {
        }
    }
}