using TeamSuneat.Data;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        public Character Owner;
        public HitmarkNames Name;
        public string NameString;

        protected DamageCalculator _damageInfo = new DamageCalculator();
        protected HitmarkAssetData AssetData;

        public Vital TargetVital { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();

            AutoGetFeedbackComponents();
        }

        private void OnValidate()
        {
            Validate();
        }

        protected virtual void Validate()
        {
            EnumEx.ConvertTo(ref Name, NameString);
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            NameString = Name.ToString();
        }

        public override void AutoNaming()
        {
            SetGameObjectName(NameString);
        }

        public virtual void Initialize()
        {
            LogInfo("공격 독립체를 초기화합니다.");

            InitializeFeedbacks();
            LoadAndValidateAssetData();
            SetupDamageCalculator();
        }

        private void SetupDamageCalculator()
        {
            _damageInfo.HitmarkAssetData = AssetData;
            _damageInfo.SetAttacker(Owner);
            _damageInfo.AttackEntity = this;
        }

        private void LoadAndValidateAssetData()
        {
            if (!ValidateHitmarkName())
            {
                return;
            }

            LoadHitmarkAsset();
            ValidateAssetData();
        }

        private bool ValidateHitmarkName()
        {
            if (Name == HitmarkNames.None)
            {
                Log.Error("공격 독립체의 히트마크 이름이 설정되지 않았습니다. {0}", this.GetHierarchyPath());
                return false;
            }
            return true;
        }

        private void LoadHitmarkAsset()
        {
            AssetData = ScriptableDataManager.Instance.FindHitmarkClone(Name);
        }

        private void ValidateAssetData()
        {
            if (!AssetData.IsValid())
            {
                Log.Error("공격 독립체의 히트마크 에셋이 설정되지 않았습니다. {0}, {1}", Name.ToLogString(), this.GetHierarchyPath());
                return;
            }
        }

        public virtual void OnBattleReady()
        {
        }

        public virtual void OnOwnerDeath()
        {
        }

        public virtual void Activate()
        {
            LogInfo("공격을 활성화합니다.");

            TriggerAttackStartFeedback();
        }

        public virtual void Deactivate()
        {
            LogInfo("공격을 비활성화합니다.");
        }

        public virtual void Execute()
        {
            LogInfo("공격을 실행합니다.");
        }

        protected void OnExecute(bool isAttackSucceeded)
        {               
            // 모든 타겟 처리 후 WeaponDamageOverride 초기화
            _damageInfo.WeaponDamageOverride = null;
        }

        public virtual void SetOwner(Character caster)
        {
            if (caster != null)
            {
                Owner = caster;
            }
        }

        public virtual void SetTarget(Vital targetVital)
        {
            TargetVital = targetVital;
        }

        public void SetWeaponDamageOverride(float? weaponDamage)
        {
            _damageInfo.WeaponDamageOverride = weaponDamage;
        }

        public virtual Vital GetTargetVital()
        {
            return TargetVital;
        }

        public virtual Vital GetTargetVital(int index)
        {
            return TargetVital;
        }

        protected virtual bool CheckDamageableVital(Vital targetVital)
        {
            if (targetVital == null)
            {
                return false;
            }
            else if (!targetVital.IsAlive)
            {
                return false;
            }
            else if (targetVital.Health.CheckInvulnerable())
            {
                return false;
            }

            return true;
        }

        protected virtual bool ValidateAttackConditions()
        {
            if (_damageInfo.TargetVital == null)
            {
                LogWarning("공격 독립체의 목표 바이탈이 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            if (!_damageInfo.HitmarkAssetData.IsValid())
            {
                LogError("피해량 정보의 히트마크 에셋이 올바르지 않습니다. Hitmark:{0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            return true;
        }

        protected virtual bool ProcessDamageResults()
        {
            if (!_damageInfo.DamageResults.IsValid())
            {
                LogWarning("공격 독립체의 피해 결과가 설정되지 않았습니다. Hitmark: {0}, Entity: {1}", _damageInfo.HitmarkAssetData.Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            bool isAttackSuccessed = false;
            for (int i = 0; i < _damageInfo.DamageResults.Count; i++)
            {
                if (ProcessSingleDamageResult(_damageInfo.DamageResults[i]))
                {
                    isAttackSuccessed = true;
                }
            }

            return isAttackSuccessed;
        }

        protected virtual bool ProcessSingleDamageResult(DamageResult damageResult)
        {
            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    return ProcessHealDamage(damageResult);

                case DamageTypes.RestoreMana:
                case DamageTypes.RestoreManaOverTime:
                    return ProcessRestoreManaDamage(damageResult);

                case DamageTypes.ChargeShield:
                    return ProcessChargeDamage(damageResult);

                default:
                    return ProcessRegularDamage(damageResult);
            }
        }

        protected virtual bool ProcessHealDamage(DamageResult damageResult)
        {
            _damageInfo.TargetVital.Heal(damageResult.DamageValueToInt);            
            return true;
        }

        protected virtual bool ProcessRestoreManaDamage(DamageResult damageResult)
        {
            _damageInfo.TargetVital.RestoreMana(damageResult.DamageValueToInt);            
            return true;
        }

        protected virtual bool ProcessChargeDamage(DamageResult damageResult)
        {
            _damageInfo.TargetVital.Charge(damageResult.DamageValueToInt);            
            return true;
        }

        protected virtual bool ProcessRegularDamage(DamageResult damageResult)
        {
            if (_damageInfo.TargetVital.CheckDamageImmunity(damageResult))
            {
                return false;
            }

            if (_damageInfo.TargetVital.TakeDamage(damageResult))
            {
                TriggerDamageFeedback();
                return true;
            }

            return false;
        }

        protected virtual void TriggerDamageFeedback()
        {
            if (_damageInfo.TargetVital.IsAlive)
            {
                TriggerAttackOnHitDamageableFeedback(_damageInfo.TargetVital.position);
            }
            else
            {
                TriggerAttackOnKillFeedback(_damageInfo.TargetVital.position);
            }
        }
    }
}