using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class MonsterAttackAbility : CharacterAbility
    {
        [SerializeField]
        private HitmarkNames _hitmarkOverride = HitmarkNames.None;

        private CharacterManager _characterManager;
        private MonsterCharacter _monster;
        private PlayerCharacter _player;

        public override void Initialization()
        {
            base.Initialization();
            EnsureReferences();
        }

        public bool CanAttackPlayer()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            if (!IsPlayerValid(_player))
            {
                return false;
            }

            return true;
        }

        public bool TryAttackPlayer()
        {
            if (_monster != null && _monster.ConditionState.Compare(CharacterConditions.Stunned))
            {
                Log.Progress(LogTags.Monster, "기절 상태로 인해 공격할 수 없습니다.");
                return false;
            }

            if (!CanAttackPlayer())
            {
                return false;
            }

            HitmarkNames hitmark = ResolveHitmark();
            if (hitmark == HitmarkNames.None)
            {
                Log.Warning(LogTags.Monster, "몬스터 기본 공격 히트마크가 설정되지 않았습니다.");
                return false;
            }

            return ExecuteAttack(hitmark);
        }

        private bool ExecuteAttack(HitmarkNames hitmark)
        {
            if (_player == null || _player.MyVital == null)
            {
                return false;
            }

            _monster.SetTarget(_player);
            AttackEntity attackEntity = _monster.Attack.FindEntity(hitmark);
            if (attackEntity != null)
            {
                attackEntity.SetTarget(_player.MyVital);
            }

            _monster.Attack.Activate(hitmark);

            Log.Info(LogTags.Monster, "최하단 몬스터가 플레이어를 공격했습니다. Monster={0}", _monster.Name.ToLogString());
            return true;
        }

        private HitmarkNames ResolveHitmark()
        {
            if (_hitmarkOverride != HitmarkNames.None)
            {
                return _hitmarkOverride;
            }

            if (_monster != null && _monster.Attack != null && _monster.Attack.BasicAttackHitmark != HitmarkNames.None)
            {
                return _monster.Attack.BasicAttackHitmark;
            }

            return HitmarkNames.None;
        }

        private bool EnsureReferences()
        {
            _monster ??= Owner as MonsterCharacter ?? GetComponent<MonsterCharacter>();
            _characterManager ??= CharacterManager.Instance;
            _player ??= _characterManager != null ? _characterManager.Player : null;

            return _monster != null && _characterManager != null;
        }

        private bool IsPlayerValid(PlayerCharacter player)
        {
            if (player == null)
            {
                return false;
            }

            if (!player.IsAlive)
            {
                return false;
            }

            if (player.MyVital == null)
            {
                return false;
            }

            return true;
        }
    }
}