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

        [InfoBox("전역 쿨타임 시간 (초). 어떤 공격을 사용하든 다음 공격까지 대기하는 시간입니다.")]
        [SerializeField]
        private float _globalCooldownTime = 0f;

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

            // Inspector에서 설정한 전역 쿨타임을 MonsterAttackCooldown에 등록
            if (_globalCooldownTime > 0f)
            {
                _attackCooldown.SetGlobalCooldown(_globalCooldownTime);
            }
        }

        //──────────────────────────────────────────────────────────────────────────────────────────
        // 쿨타임 관리 (몬스터 전용)

        public override bool CheckAttackCooldown(int attackOrder)
        {
            // 개별 공격 쿨타임 체크
            bool isIndividualCooldown = _attackCooldown.CheckCooldown(attackOrder);
            
            // 전역 쿨타임 체크
            bool isGlobalCooldown = _attackCooldown.CheckGlobalCooldown();
            
            // 둘 중 하나라도 쿨타임 중이면 공격 불가
            return isIndividualCooldown || isGlobalCooldown;
        }

        public override bool CheckGlobalCooldown()
        {
            return _attackCooldown.CheckGlobalCooldown();
        }

        public override void StartAttackCooldown(int attackOrder)
        {
            // 개별 공격 쿨타임 시작
            _attackCooldown.StartCooldown(attackOrder);
            
            // 전역 쿨타임 시작
            _attackCooldown.StartGlobalCooldown();
        }

        public override void SetAttackCooldown(int attackOrder, float cooldownTime)
        {
            _attackCooldown.SetCooldown(attackOrder, cooldownTime);
        }

        public void SetGlobalCooldown(float cooldownTime)
        {
            _globalCooldownTime = cooldownTime;
            _attackCooldown.SetGlobalCooldown(cooldownTime);
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