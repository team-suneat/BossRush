using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackCooldown
    {
        // 공격 순서별 쿨타임 종료 시간 저장
        private readonly Dictionary<int, float> _cooldownEndTimes = new();

        // 공격 순서별 쿨타임 시간 설정 저장 (기본값 관리용)
        private readonly Dictionary<int, float> _cooldownTimes = new();

        public void SetCooldown(int attackOrder, float cooldownTime)
        {
            if (cooldownTime <= 0f)
            {
                // 쿨타임 시간이 0 이하면 설정에서 제거
                _cooldownTimes.Remove(attackOrder);
                return;
            }

            _cooldownTimes[attackOrder] = cooldownTime;
        }

        public bool CheckCooldown(int attackOrder)
        {
            // 쿨타임이 설정되지 않았으면 쿨타임중이 아님
            if (!_cooldownEndTimes.ContainsKey(attackOrder))
            {
                return false;
            }

            // 쿨타임 종료 시간 체크
            float endTime = _cooldownEndTimes[attackOrder];
            return Time.time < endTime; // 쿨타임 여부 반환
        }

        public void StartCooldown(int attackOrder)
        {
            // 쿨타임 시간이 설정되어 있지 않으면 시작하지 않음
            if (!_cooldownTimes.TryGetValue(attackOrder, out float cooldownTime))
            {
                return;
            }

            if (cooldownTime <= 0f)
            {
                return;
            }

            // 쿨타임 시작 (현재 시간 + 쿨타임 시간)
            _cooldownEndTimes[attackOrder] = Time.time + cooldownTime;
        }

        public void ClearCooldown(int attackOrder)
        {
            _cooldownEndTimes.Remove(attackOrder);
        }

        public void ClearAllCooldowns()
        {
            _cooldownEndTimes.Clear();
        }

        public bool HasCooldown(int attackOrder)
        {
            return _cooldownTimes.ContainsKey(attackOrder);
        }

        public void Initialize()
        {
            _cooldownEndTimes.Clear();
            // _cooldownTimes는 유지 (설정된 쿨타임 시간 보존)
        }
    }
}
