using UnityEngine;

namespace TeamSuneat
{
    public class PlayerAttackAbility : CharacterAbility
    {
        private const float BASE_ATTACK_INTERVAL = 0.3f;

        private PlayerCharacter _player;
        private float _attackInterval;
        private float _lastAttackTime;

        private int _attackAnimationParameter;

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

            TryAttackTarget();
        }

        #endregion Unity Lifecycle

        #region Public Methods

        public bool CanAttackTarget()
        {
            if (IsTargetValid(_player.TargetCharacter))
            {
                return true;
            }

            return TryFindAndSetTarget();
        }

        public bool TryAttackTarget()
        {
            if (!EnsureReferences())
            {
                return false;
            }

            if (IsStunned())
            {
                return false;
            }

            if (!CanAttackTarget())
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
            Character targetCharacter = _player.TargetCharacter;
            if (targetCharacter == null)
            {
                return false;
            }

            _player.SetTarget(targetCharacter);
            _player.CharacterAnimator.PlayAttackAnimationByHitmark(HitmarkNames.PlayerAttack);
            return true;
        }

        private bool IsStunned()
        {
            if (_player.ConditionState.Compare(CharacterConditions.Stunned))
            {
                Log.Progress(LogTags.Player, "기절 상태로 인해 공격할 수 없습니다.");
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

        private bool TryFindAndSetTarget()
        {
            Character firstMonster = CharacterManager.Instance?.FindFirstDamageableMonster();
            if (firstMonster == null)
            {
                return false;
            }

            _player.SetTarget(firstMonster);
            return true;
        }

        private bool IsTargetValid(Character targetCharacter)
        {
            if (targetCharacter == null)
            {
                return false;
            }

            if (!targetCharacter.IsAlive)
            {
                return false;
            }

            if (targetCharacter.MyVital == null)
            {
                return false;
            }

            return true;
        }

        #endregion Private Methods - Target

        #region Private Methods - Stat

        private void RegisterStatEvents()
        {
            if (_player != null)
            {
                _player.Stat.RegisterOnRefresh(OnStatRefresh);
            }
        }

        private void UnregisterStatEvents()
        {
            if (_player != null)
            {
                _player.Stat.UnregisterOnRefresh(OnStatRefresh);
            }
        }

        private void InitializeAttackInterval()
        {
            if (_player != null)
            {
                float attackSpeed = _player.Stat.FindValueOrDefault(StatNames.AttackSpeed);
                UpdateAttackInterval(attackSpeed);
            }
        }

        private void OnStatRefresh(StatNames statName, float newValue)
        {
            if (statName == StatNames.AttackSpeed)
            {
                Log.Info(LogTags.Player, "공격속도 새로고침: {0}", newValue);
                UpdateAttackInterval(newValue);
            }
        }

        private void UpdateAttackInterval(float attackSpeed)
        {
            if (attackSpeed <= 0.0f)
            {
                return;
            }

            // 공격 속도가 높을수록 간격은 짧아짐 (역수 관계)
            _attackInterval = BASE_ATTACK_INTERVAL.SafeDivide01(attackSpeed);
        }

        #endregion Private Methods - Stat

        #region Private Methods - Reference

        private bool EnsureReferences()
        {
            _player ??= Owner as PlayerCharacter ?? GetComponent<PlayerCharacter>();

            return _player != null;
        }

        #endregion Private Methods - Reference

        protected override void InitializeAnimatorParameters()
        {
            base.InitializeAnimatorParameters();

            AddAnimatorParameterIfExists(HitmarkNames.PlayerAttack);
        }

        private void AddAnimatorParameterIfExists(HitmarkNames hitmark)
        {
            if (Owner == null)
            {
                return;
            }

            if (Owner.CharacterAnimator == null)
            {
                return;
            }

            Owner.CharacterAnimator.AddAnimatorParameterIfExists(
                hitmark.ToString(),
                out _attackAnimationParameter,
                AnimatorControllerParameterType.Bool);
        }
    }
}