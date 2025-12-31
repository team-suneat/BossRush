using UnityEngine;

namespace TeamSuneat
{
    public class BossAttackAbility : CharacterAbility
    {
        private const float BASE_ATTACK_INTERVAL = 3f;

        private BossCharacter _boss;
        private CharacterManager _characterManager;
        private PlayerCharacter _player;
        private float _attackInterval;
        private float _lastAttackTime;

        #region Unity Lifecycle

        public override void Initialization()
        {
            base.Initialization();

            EnsureReferences();
            RegisterStatEvents();
            InitializeAttackInterval();
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            UnregisterStatEvents();
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            TryAttackPlayer();
        }

        #endregion Unity Lifecycle

        #region Public Methods

        public bool CanAttackPlayer()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            return IsPlayerValid(_player);
        }

        public bool TryAttackPlayer()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            if (IsStunned())
            {
                return false;
            }

            if (!CanAttackPlayer())
            {
                return false;
            }

            if (!IsAttackIntervalElapsed())
            {
                return false;
            }

            if (ExecuteAttack())
            {
                UpdateLastAttackTime();
                return true;
            }

            return false;
        }

        #endregion Public Methods

        #region Private Methods - Attack

        private bool ExecuteAttack()
        {
            if (_player == null || _player.MyVital == null)
            {
                return false;
            }

            HitmarkNames hitmark = ResolveHitmark();
            if (hitmark == HitmarkNames.None)
            {
                Log.Warning(LogTags.Boss, "보스 기본 공격 히트마크가 설정되지 않았습니다.");
                return false;
            }

            _boss.SetTarget(_player);
            _boss.CharacterAnimator.PlayAttackAnimation();
            return true;
        }

        private HitmarkNames ResolveHitmark()
        {
            if (_boss != null && _boss.Attack != null && _boss.Attack.BasicAttackHitmark != HitmarkNames.None)
            {
                return _boss.Attack.BasicAttackHitmark;
            }

            return HitmarkNames.None;
        }

        private bool IsStunned()
        {
            if (_boss.ConditionState.Compare(CharacterConditions.Stunned))
            {
                Log.Progress(LogTags.Boss, "기절 상태로 인해 공격할 수 없습니다.");
                return true;
            }

            return false;
        }

        private bool IsAttackIntervalElapsed()
        {
            float currentTime = Time.time;
            return currentTime - _lastAttackTime >= _attackInterval;
        }

        private void UpdateLastAttackTime()
        {
            _lastAttackTime = Time.time;
        }

        #endregion Private Methods - Attack

        #region Private Methods - Target

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

        #endregion Private Methods - Target

        #region Private Methods - Stat

        private void RegisterStatEvents()
        {
            if (_boss != null)
            {
                _boss.Stat.RegisterOnRefresh(OnStatRefresh);
            }
        }

        private void UnregisterStatEvents()
        {
            if (_boss != null)
            {
                _boss.Stat.UnregisterOnRefresh(OnStatRefresh);
            }
        }

        private void InitializeAttackInterval()
        {
            if (_boss != null)
            {
                float attackSpeed = _boss.Stat.FindValueOrDefault(StatNames.AttackSpeed);
                UpdateAttackInterval(attackSpeed);
            }
        }

        private void OnStatRefresh(StatNames statName, float newValue)
        {
            if (statName == StatNames.AttackSpeed)
            {
                Log.Info(LogTags.Boss, "공격속도 새로고침: {0}", newValue);
                UpdateAttackInterval(newValue);
            }
        }

        private void UpdateAttackInterval(float attackSpeed)
        {
            if (attackSpeed <= 0.0f)
            {
                _attackInterval = BASE_ATTACK_INTERVAL;
            }
            else
            {
                // 공격 속도가 높을수록 간격은 짧아짐 (역수 관계)
                _attackInterval = BASE_ATTACK_INTERVAL.SafeDivide01(attackSpeed);
            }
        }

        #endregion Private Methods - Stat

        #region Private Methods - Reference

        private bool EnsureReferences()
        {
            _boss ??= Owner as BossCharacter ?? GetComponent<BossCharacter>();
            _characterManager ??= CharacterManager.Instance;
            _player ??= _characterManager != null ? _characterManager.Player : null;

            return _boss != null && _characterManager != null;
        }

        #endregion Private Methods - Reference
    }
}