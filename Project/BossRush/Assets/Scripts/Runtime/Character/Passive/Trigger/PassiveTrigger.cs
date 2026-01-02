
using UnityEngine;

namespace TeamSuneat.Passive
{
    public class PassiveTrigger
    {
        // 기본 정보
        public PassiveNames Name { get; private set; }

        public int Level { get; private set; }
        public int Stack { get; internal set; }
        public int AttackIndex { get; internal set; }

        // 피해량 관련

        public float DamageValue { get; internal set; }

        // 위치 정보

        public Vector3 AttackPosition { get; internal set; }
        public Vector3 DamagePosition { get; internal set; }

        // 스킬/버프/히트마크 정보

        public SkillNames SkillName { get; internal set; }
        public BuffNames BuffName { get; internal set; }
        public HitmarkNames HitmarkName { get; internal set; }

        // 캐릭터/타겟 정보

        public Character Attacker { get; internal set; }
        public Collider2D TargetVitalCollider { get; internal set; }
        public Vital TargetVital { get; internal set; }
        public Character TargetCharacter { get; internal set; }

        // 아이템 정보

        public string ItemName { get; internal set; }

        // 생성자를 private으로 설정하고 팩토리 메서드 패턴 사용
        private PassiveTrigger()
        { }

        // 기본 패시브 트리거 생성
        public static PassiveTrigger Create(PassiveNames name, int level)
        {
            return new PassiveTrigger
            {
                Name = name,
                Level = level
            };
        }

        // 데미지 관련 메서드
        public int GetDamageValueToInt()
        {
            return DamageValue.IsZero() ? 0 : Mathf.RoundToInt(DamageValue);
        }

        // 깊은 복사 메서드
        public PassiveTrigger Clone()
        {
            return new PassiveTrigger
            {
                Name = this.Name,
                Level = this.Level,
                Stack = this.Stack,
                AttackIndex = this.AttackIndex,
                DamageValue = this.DamageValue,
                AttackPosition = this.AttackPosition,
                DamagePosition = this.DamagePosition,
                SkillName = this.SkillName,
                BuffName = this.BuffName,
                HitmarkName = this.HitmarkName,
                Attacker = this.Attacker,
                TargetVitalCollider = this.TargetVitalCollider,
                TargetVital = this.TargetVital,
                TargetCharacter = this.TargetCharacter,
                ItemName = this.ItemName,
            };
        }

        // 데이터 일괄 업데이트 메서드 추가
        internal void UpdateFromDamageResult(DamageResult damageResult)
        {
            Attacker = damageResult.Attacker;
            TargetVital = damageResult.TargetVital;
            TargetVitalCollider = damageResult.TargetVitalCollider;
            TargetCharacter = damageResult.TargetCharacter;
            DamageValue = damageResult.DamageValue;
            SkillName = damageResult.Skill;
            HitmarkName = damageResult.HitmarkName;
            AttackPosition = damageResult.AttackPosition;
            DamagePosition = damageResult.DamagePosition;
        }
    }
}