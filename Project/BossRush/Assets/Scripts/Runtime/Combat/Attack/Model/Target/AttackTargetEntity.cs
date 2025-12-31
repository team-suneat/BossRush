namespace TeamSuneat
{
    public class AttackTargetEntity : AttackEntity
    {
        public override void AutoNaming()
        {
            SetGameObjectName(Name.ToString());
        }

        public override void SetTarget(Vital targetVital)
        {
            base.SetTarget(targetVital);

            if (targetVital != null)
            {
                _damageInfo.SetTargetVital(targetVital);
                UpdatePositionToTarget();
            }
        }

        public override void Activate()
        {
            // Activate에서 피드백이 호출되기 전에 위치를 설정해야 함
            UpdatePositionToTarget();

            base.Activate();

            Execute();
        }

        public override void Execute()
        {
            base.Execute();

            if (!CanExecuteAttack())
            {
                return;
            }

            RefreshTarget();
            UpdatePositionToTarget();
            ApplyAttack();
        }

        private bool CanExecuteAttack()
        {
            if (!AssetData.IsValid())
            {
                LogWarning("공격 에셋 데이터가 유효하지 않습니다. Hitmark: {0}, Entity: {1}", Name.ToLogString(), this.GetHierarchyPath());
                return false;
            }

            return true;
        }

        private void RefreshTarget()
        {
            if (_damageInfo.TargetVital != null && _damageInfo.TargetVital.IsAlive)
            {
                return;
            }

            Vital targetVital = GetTargetVital();
            if (CheckDamageableVital(targetVital))
            {
                _damageInfo.SetTargetVital(targetVital);
            }
        }

        private void UpdatePositionToTarget()
        {
            if (_damageInfo.TargetVital == null)
            {
                return;
            }

            transform.position = _damageInfo.TargetVital.position;
        }

        public override Vital GetTargetVital()
        {
            switch (AssetData.AttackTargetType)
            {
                case AttackTargetTypes.Owner:
                    return GetOwnerVital();

                case AttackTargetTypes.TargetOfOwner:
                    return GetOwnerTargetVital();

                default:
                    return null;
            }
        }

        private Vital GetOwnerVital()
        {
            return Owner?.MyVital;
        }

        private Vital GetOwnerTargetVital()
        {
            return Owner?.TargetCharacter?.MyVital;
        }

        private void ApplyAttack()
        {
            bool isAttackSucceeded = AttackToTarget();
            OnExecute(isAttackSucceeded);
        }

        private bool AttackToTarget()
        {
            if (!ValidateAttackConditions())
            {
                return false;
            }

            ExecuteDamageCalculation();
            return ProcessDamageResults();
        }

        private void ExecuteDamageCalculation()
        {
            _damageInfo?.Execute();
        }
    }
}