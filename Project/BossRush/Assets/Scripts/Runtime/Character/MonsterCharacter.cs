using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        public override Transform Target => null;

        public override LogTags LogTag => LogTags.Monster;

        public override void Initialize()
        {
            base.Initialize();

            CharacterManager.Instance.Register(this);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();

            CharacterManager.Instance.Unregister(this);
        }

        //

        public override void SetTarget(Vital targetVital)
        {
            if (targetVital == null)
            {
                return;
            }

            if (targetVital.Owner == null)
            {
                return;
            }

            TargetCharacter = targetVital.Owner;
        }

        public override void SetTarget(Character targetCharacter)
        {
            TargetCharacter = targetCharacter;
        }

        //
        public override void AddCharacterStats()
        {
            Log.Warning("몬스터의 능력치를 불러올 수 없습니다.");
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.Unregister(this);
            transform.SetParent(null);
            CharacterAnimator?.PlayDeathAnimation();

            GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_DEATH, this);
        }
    }
}