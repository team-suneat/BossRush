using UnityEngine;

namespace TeamSuneat.Passive
{
    /// <summary>
    /// 영역 공격 실행 트리거를 처리하는 클래스
    /// 영역 공격(AttackAreaEntity)에서만 발동됩니다.
    /// </summary>
    public class PassiveTriggerExecuteAttackArea : PassiveTriggerReceiver
    {
        private int _attackIndex;
        private HitmarkNames _hitmarkName;
        private Vector3 _attackPosition;
        private float _damageValue;

        protected override PassiveTriggers Trigger => PassiveTriggers.ExecuteAttackArea;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<DamageResult, int>.Register(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_SUCCESS, OnGlobalEvent);
            GlobalEvent<HitmarkNames, int, Vector3, AttackEntityTypes>.Register(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_FAILED, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<DamageResult, int>.Unregister(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_SUCCESS, OnGlobalEvent);
            GlobalEvent<HitmarkNames, int, Vector3, AttackEntityTypes>.Unregister(GlobalEventType.PLAYER_CHARACTER_EXECUTE_ATTACK_FAILED, OnGlobalEvent);
        }

        private void OnGlobalEvent(HitmarkNames hitmarkName, int attackIndex, Vector3 attackPosition, AttackEntityTypes attackEntityType)
        {
            // 영역 공격이 아닌 경우 무시
            if (!CheckAreaAttack(attackEntityType))
            {
                return;
            }

            _hitmarkName = hitmarkName;
            _attackIndex = attackIndex;
            _attackPosition = attackPosition;
            _damageValue = 0;

            if (TryExecute())
            {
                if (Entity != null)
                {
                    TriggerInfo.HitmarkName = _hitmarkName;
                    TriggerInfo.AttackIndex = _attackIndex;
                    TriggerInfo.AttackPosition = _attackPosition;
                }

                Execute();
            }
        }

        private void OnGlobalEvent(DamageResult damageResult, int attackIndex)
        {
            // 영역 공격이 아닌 경우 무시
            if (!CheckAreaAttack(damageResult.EntityType))
            {
                return;
            }

            _hitmarkName = damageResult.HitmarkName;
            _attackIndex = attackIndex;
            _attackPosition = damageResult.AttackPosition;
            _damageValue = damageResult.DamageValue;

            if (TryExecute(damageResult))
            {
                if (Entity != null)
                {
                    TriggerInfo.AttackIndex = _attackIndex;
                }

                ExecuteWithDamage(damageResult);
            }
        }

        /// <summary>
        /// 해당 히트마크가 영역 공격인지 확인합니다.
        /// </summary>
        /// <param name="hitmarkName">히트마크 이름</param>
        /// <returns>영역 공격 여부</returns>
        private bool CheckAreaAttack(AttackEntityTypes entityType)
        {
            return entityType == AttackEntityTypes.Area;
        }

        public override bool TryExecute()
        {
            if (!base.TryExecute())
            {
                return false;
            }

            if (!Checker.CheckTriggerHitmark(_hitmarkName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerCount(Entity.TriggerCount))
            {
                Entity.AddTriggerCount();
                return false;
            }

            return CheckConditions();
        }

        public override bool TryExecute(DamageResult damageResult)
        {
            if (!base.TryExecute(damageResult))
            {
                return false;
            }

            if (!Checker.CheckTriggerHitmark(_hitmarkName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerCount(Entity.TriggerCount))
            {
                Entity.AddTriggerCount();
                return false;
            }

            return CheckConditions();
        }
    }
}