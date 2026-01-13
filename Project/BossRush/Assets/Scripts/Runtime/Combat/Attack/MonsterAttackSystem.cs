using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackSystem : AttackSystem
    {
        [InfoBox("공격 순서별 쿨타임 시간 (초). 인덱스 0 = Attack0, 인덱스 1 = Attack1, ...")]
        [SerializeField]
        private List<float> _cooldownTimes = new List<float>();

        [SerializeField]
        private MonsterAttackableArea _attackableArea;

        // 쿨타임 관리 (몬스터 전용)
        private readonly MonsterAttackCooldown _attackCooldown = new();

        public override void Initialize()
        {
            base.Initialize();

            _attackCooldown.Initialize();

            // Inspector에서 설정한 쿨타임을 MonsterAttackCooldown에 등록
            for (int i = 0; i < _cooldownTimes.Count; i++)
            {
                if (_cooldownTimes[i] > 0f)
                {
                    _attackCooldown.SetCooldown(i, _cooldownTimes[i]);
                }
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────
        // 쿨타임 관리 (몬스터 전용)

        public override bool CheckAttackCooldown(int attackOrder)
        {
            return _attackCooldown.CheckCooldown(attackOrder);
        }

        public override void StartAttackCooldown(int attackOrder)
        {
            _attackCooldown.StartCooldown(attackOrder);
        }

        public override void SetAttackCooldown(int attackOrder, float cooldownTime)
        {
            _attackCooldown.SetCooldown(attackOrder, cooldownTime);
        }

        //──────────────────────────────────────────────────────────────────────────────────────────
        // 공격 가능 영역(몬스터 전용)

        public override bool CheckTargetInAttackableArea()
        {
            if (_attackableArea.CheckTargetInArea())
            {
                return true;
            }

            return base.CheckTargetInAttackableArea();
        }
    }
}