using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        public override Transform Target => null;

        public override LogTags LogTag => LogTags.Monster;

        public override void Initialize()
        {
            SetupLevel();

            base.Initialize();

            PlaySpawnAnimation();
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
            MonsterStatConfigAsset monsterStatConfigAsset = ScriptableDataManager.Instance.GetMonsterStatConfigAsset();
            if (monsterStatConfigAsset.IsValid())
            {
                int health = monsterStatConfigAsset.GetHealth(Level, IsBoss);
                int attack = monsterStatConfigAsset.GetAttack(Level, IsBoss);

                Stat.AddWithSourceInfo(StatNames.Health, health, this, NameString, "CharacterBase");
                Stat.AddWithSourceInfo(StatNames.Attack, attack, this, NameString, "CharacterBase");
                Stat.AddWithSourceInfo(StatNames.AttackSpeed, 1f, this, NameString, "CharacterBase");
            }
            else
            {
                Log.Error("몬스터의 능력치를 불러올 수 없습니다.");
            }
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.Unregister(this);
            transform.SetParent(null);
            CharacterAnimator?.PlayDeathAnimation();
            AddDropItems();

            if (IsBoss)
            {
                GlobalEvent<Character>.Send(GlobalEventType.BOSS_CHARACTER_DEATH, this);
            }
            else
            {
                GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_DEATH, this);
            }
        }

        private void AddDropItems()
        {
            if (ProfileInfo == null) return;

            MonsterDropConfigAsset config = ScriptableDataManager.Instance.GetMonsterDropConfigAsset();
            if (config == null) return;

            int gold = config.GetGoldDrop(Level, IsBoss);
            ProfileInfo.Currency.Add(CurrencyNames.Gold, gold);

            if (config.TryDropEnhancementCube())
            {
                int cube = config.GetCubeDrop(Level, IsBoss);
                ProfileInfo.Currency.Add(CurrencyNames.EnhancementCube, cube);
            }

            int exp = config.GetExpDrop(Level, IsBoss);
            ProfileInfo.Level.AddExperience(exp);
        }
    }
}