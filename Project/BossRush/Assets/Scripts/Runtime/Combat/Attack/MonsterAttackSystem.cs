using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackSystem : AttackSystem
    {
        [SerializeField]
        private MonsterAttackableArea[] _attackableAreas;

        //──────────────────────────────────────────────────────────────────────────────────────────
        // 공격 가능 영역(몬스터 전용)

        public override bool CheckTargetInAttackableArea()
        {
            return CheckTargetInAttackableArea(0);
        }

        public override bool CheckTargetInAttackableArea(int index)
        {
            if (_attackableAreas == null || _attackableAreas.Length == 0)
            {
                return false;
            }

            if (index < 0 || index >= _attackableAreas.Length)
            {
                return false;
            }

            if (_attackableAreas[index] == null)
            {
                return false;
            }

            if (_attackableAreas[index].CheckTargetInArea())
            {
                return true;
            }

            return false;
        }
    }
}