using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 생명력을 관리하는 클래스입니다. </summary>
    public partial class Life : VitalResource
    {
        #region Field

        [Title("#Life")]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해를 입은 후 피해를 입지 않는 시간")]
        [SerializeField] private float _invincibilityDurationOnDamage = 0.5f;

        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해를 입지 않음")]
        [SerializeField] private bool _invulnerable;

        [ReadOnly]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("잠시 피해를 입지 않음")]
        private List<Component> _temporarilyInvulnerable = new();

        public IReadOnlyList<Component> TemporarilyInvulnerable => _temporarilyInvulnerable;

        [ReadOnly]
        [FoldoutGroup("#Toggle")]
        [SuffixLabel("피해 후 잠시 피해를 입지 않음")]
        public bool PostDamageInvulnerable;

        [FoldoutGroup("#Toggle")]
        [SuffixLabel("영구 피해 면역")]
        [SerializeField] private bool _immuneToDamage;

        [FoldoutGroup("#Toggle")]
        [Tooltip("이것이 사실이면 피해 시 Lit 타입의 매터리얼을 사용하여 히트이펙트를 보여지게합니다.")]
        [SuffixLabel("피해시 Lit 타입 매터리얼 히트이펙트")]
        [SerializeField] private bool _drawFlashLitOnDamage;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이 아니라면 객체는 죽은 후에도 그곳에 남아있을 것입니다")]
        [SuffixLabel("사망 후 개체 삭제")]
        [SerializeField] private bool _destroyOnDeath = true;

        [FoldoutGroup("#Death")]
        [Tooltip("캐릭터가 파괴되거나 비활성화되기까지의 시간(초)")]
        [SuffixLabel("사망 후 개체 삭제 지연 시간")]
        [SerializeField] private float _delayBeforeDestruction = 5f;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이면 캐릭터가 죽을 때 충돌이 꺼집니다")]
        [SuffixLabel("사망 후 충돌 무시")]
        [SerializeField] private bool _collisionsOffOnDeath;

        [FoldoutGroup("#Death")]
        [Tooltip("이것이 사실이라면 죽음에 중력이 꺼질 것입니다")]
        [SuffixLabel("사망 후 중력 무시")]
        [SerializeField] private bool _gravityOffOnDeath;

        [FoldoutGroup("#Death")]
        [Tooltip("false로 설정하면 캐릭터가 사망한 위치에서 부활하고, 그렇지 않으면 초기 위치(장면이 시작될 때)로 이동합니다")]
        [SuffixLabel("초기 위치 부활")]
        [SerializeField] private bool _respawnAtInitialLocation;

        #endregion Field

        #region Parameter

        public override VitalResourceTypes Type => VitalResourceTypes.Life;

        public bool IsDamaged { get; set; }

        public float InvincibilityDurationOnDamage => _invincibilityDurationOnDamage;
        public bool Invulnerable => _invulnerable;

        public bool ImmuneToDamage => _immuneToDamage;
        public bool DrawFlashLitOnDamage => _drawFlashLitOnDamage;
        public bool DestroyOnDeath => _destroyOnDeath;
        public float DelayBeforeDestruction => _delayBeforeDestruction;
        public bool CollisionsOffOnDeath => _collisionsOffOnDeath;
        public bool GravityOffOnDeath => _gravityOffOnDeath;
        public bool RespawnAtInitialLocation => _respawnAtInitialLocation;

        #endregion Parameter

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            GetFeedbackComponents();
        }

        protected override void Awake()
        {
            base.Awake();

            if (Max == 0)
            {
                Current = 1;
                Max = 1;
            }
        }

        public override void Initialize()
        {
            LogInfo("생명력을 초기화합니다.");

            base.Initialize();

            IsDamaged = false;

            ClearTemporarilyInvulnerable();
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.Stat == null)
            {
                LogWarning("최대 생명력을 불러올 수 없습니다. 바이탈, 소유 캐릭터, 능력치 시스템 중 최소 하나가 없습니다.");
                return;
            }

            float statValue = Vital.Owner.Stat.FindValueOrDefault(StatNames.Life);
            int maxLifeByStat = Mathf.RoundToInt(statValue);
            if (maxLifeByStat > 0)
            {
                int previousMax = Max;
                Max = Mathf.RoundToInt(maxLifeByStat);

                LogInfo("캐릭터의 능력치에 따라 최대 생명력을 갱신합니다. {0}/{1}", Current, Max);

                if (shouldAddExcessToCurrent && Max > previousMax)
                {
                    Current += Max - previousMax;
                }
                if (Current > Max)
                {
                    Current = Max;

                    LogInfo("캐릭터의 남은 생명력이 최대 생명력보다 크다면, 최대 생명력으로 설정합니다. {0}/{1}", Current, Max);
                }
            }
        }

        public bool CheckInvulnerable()
        {
            return _temporarilyInvulnerable.Count > 0 || _invulnerable || _immuneToDamage || PostDamageInvulnerable;
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);

            PlayHealFeedbacks(value);

            SendGlobalEventValueChanged();
            OnLifeValueChanged?.Invoke(Current, Max);

            SendGlobalEventHeal();
        }

        public void Heal(int healValue, bool useFloatyText = false)
        {
            if (Current <= 0)
            {
                LogWarning("남은 생명력이 0에 도달했다면 회복할 수 없습니다.");
                return;
            }

            if (healValue <= 0)
            {
                LogWarning("생명력 회복량이 0과 같거나 적다면 회복할 수 없습니다.");
                return;
            }

            int calculatedHealValue = CalculateHealingValue(healValue);
            if (AddCurrentValue(calculatedHealValue))
            {
                OnHeal(calculatedHealValue, useFloatyText);
            }
        }

        private void OnHeal(int healValue, bool useFloatyText)
        {
            PlayHealFeedbacks(healValue);

            SendGlobalEventValueChanged();
            OnLifeValueChanged?.Invoke(Current, Max);

            SendGlobalEventHeal();

            if (useFloatyText)
            {
                OnLifeHealRequested?.Invoke(healValue);
            }
        }

        private int CalculateHealingValue(int baseValue)
        {
            return baseValue;
        }

        public override bool UseCurrentValue(int damageValue, DamageResult damageResult)
        {
            if (ApplyCheatRule(damageValue, damageResult))
            {
                return false;
            }

            if (ApplyDotRule(damageValue, damageResult))
            {
                return false;
            }

            SendFullLifeAttackEvent(damageResult);

            return base.UseCurrentValue(damageValue, damageResult);
        }

        private bool ApplyCheatRule(int damageValue, DamageResult damageResult)
        {
            if (Vital == null || Vital.Owner == null || !Vital.Owner.IsPlayer)
            {
                return false;
            }

            if (Current <= damageValue && GameSetting.Instance.Cheat.IsNotDead)
            {
                LogInfo("죽지 않는 치트를 사용한다면 생명력을 1 남깁니다.");
                int value = Current - 1;
                Current = 1;
                OnUseCurrencyValue(value);
                return true;
            }

            return false;
        }

        private bool ApplyDotRule(int damageValue, DamageResult damageResult)
        {
            if (Vital == null || Vital.Owner == null || !Vital.Owner.IsPlayer)
            {
                return false;
            }

            if (Current <= damageValue && damageResult != null && damageResult.DamageType.IsDamageOverTime())
            {
                LogInfo("지속 피해에는 죽지 않고 생명력을 1 남깁니다.");
                int value = Current - 1;
                Current = 1;
                OnUseCurrencyValue(value);
                return true;
            }

            return false;
        }

        private void SendFullLifeAttackEvent(DamageResult damageResult)
        {
            if (damageResult != null && Current == Max)
            {
                GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_ATTACK_MONSTER_FULL_LIFE_TARGET);
            }
        }

        public void HandleDamageZero()
        {
            OnDamageZero?.Invoke();
            DamageZeroFeedbacks?.PlayFeedbacks();
        }

        public void OnDamageBlocked(DamageResult damageResult)
        {
            OnDamageZero?.Invoke();
            DamageZeroFeedbacks?.PlayFeedbacks();

            if (damageResult.DamageType.IsInstantDamage())
            {
                BlockDamageFeedbacks?.PlayFeedbacks();
            }
        }

        public void Use(int damageValue, Character attacker, bool ignoreDeath)
        {
            if (CheckInvulnerable())
            {
                return;
            }

            if (!UseCurrentValue(damageValue))
            {
                return;
            }

            if (Current < 1 && ignoreDeath)
            {
                LogInfo("해당 피해가 캐릭터를 사망시킬 수 없는 피해로 설정되었다면 생명력을 1 남깁니다.");

                Current = 1;
            }

            bool isDead = Current < 1 && !ignoreDeath;
            ProcessDamageResult(attacker, null, isDead);

            PlayDamageFeedbacks(damageValue);
            SendGlobalEventValueChanged();
            OnLifeValueChanged?.Invoke(Current, Max);
        }

        public void TakeDamage(DamageResult damageResult, Character attacker)
        {
            if (CheckInvulnerable())
            {
                return;
            }

            if (!UseCurrentValue(damageResult.DamageValueToInt, damageResult))
            {
                return;
            }

            IsDamaged = true;

            OnDamage?.Invoke(damageResult);

            OnDamageFlicker(damageResult);

            bool isDead = Current <= 0;
            if (isDead)
            {
                Current = 0;
                // 죽음 관련 외부 이벤트 및 회복 처리
                SendKillGlobalEvent(damageResult);
                RestoreOnKill(attacker);
            }

            // Life 내부 죽음 처리 (Suicide/Killed + 애니메이션/Despawn)
            ProcessDamageResult(attacker, damageResult, isDead);

            if (damageResult.DamageType.IsInstantDamage())
            {
                PlayDamageFeedbacks(damageResult.DamageValue);
            }

            SendGlobalEventValueChanged();
            OnLifeValueChanged?.Invoke(Current, Max);
        }

        private void ProcessDamageResult(Character attacker, DamageResult damageResult, bool isDead)
        {
            if (isDead)
            {
                if (Vital.Owner == attacker)
                {
                    Suicide();
                }
                else
                {
                    if (damageResult != null)
                    {
                        Killed(damageResult);
                    }
                    else
                    {
                        Killed(attacker);
                    }
                }
            }
            else
            {
                SetTargetAttacker(attacker);

                // damageResult가 null인 경우(Use 경로)에는 애니메이션을 재생하지 않음
                if (Vital.Owner != null && damageResult != null)
                {
                    TryPlayDamageAnimation(damageResult);
                }

                EnablePostDamageInvulnerability(_invincibilityDurationOnDamage);
            }
        }

        private void SetTargetAttacker(Character attacker)
        {
            if (Vital.Owner == null)
            {
                return;
            }

            Vital.Owner.SetTarget(attacker);
        }

        private bool TryPlayDamageAnimation(DamageResult damageResult)
        {
            if (Vital.Owner?.CharacterAnimator == null)
            {
                return false;
            }

            if (damageResult.Asset.NotPlayDamageAnimation)
            {
                return false;
            }

            if (damageResult.DamageType.IsDamageOverTime())
            {
                return false;
            }

            return Vital.Owner.CharacterAnimator.PlayDamageAnimation();
        }

        private void OnDamageFlicker(DamageResult damageResult)
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.CharacterRenderer == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                Vital.Owner.CharacterRenderer.StartFlickerCoroutine(RendererFlickerNames.Damage, _invincibilityDurationOnDamage);
            }
            else
            {
                Vital.Owner.CharacterRenderer.StartFlickerCoroutine(RendererFlickerNames.Damage);
            }
        }

        public void Killed(Character attacker)
        {
            DamageResult damageResult = new DamageResult()
            {
                Attacker = attacker,
                TargetVital = Vital,
            };
            ProcessKilled(attacker, damageResult);
        }

        public void Killed(DamageResult damageResult)
        {
            Character attacker = damageResult?.Attacker;
            ProcessKilled(attacker, damageResult);
        }

        private void ProcessKilled(Character attacker, DamageResult damageResult)
        {
            Vital.Life.ResetTemporarilyInvulnerable(this);

            PlayDeathFeedback();
            PlayKilledFeedbacks();

            if (attacker != null)
            {
                LogInfo("캐릭터가 {0}에게 죽습니다.", attacker.Name.ToLogString());
            }
            OnKilled?.Invoke(attacker);

            if (Vital.Owner != null)
            {
                OnDeath?.Invoke(damageResult);
            }

            PlayDeathAnimation();

            if (TryDespawn())
            {
                Despawn();
            }
        }

        public void Suicide()
        {
            LogInfo("캐릭터가 스스로 죽음에 이릅니다.");
            Current = 0;
            Vital.Life.ResetTemporarilyInvulnerable(this);

            PlayDeathFeedback();
            PlaySuicideFeedbacks();

            if (Vital.Owner != null)
            {
                OnDeath?.Invoke(new DamageResult()
                {
                    Attacker = Vital.Owner,
                    TargetVital = Vital
                });
            }

            PlayDeathAnimation();

            if (TryDespawn())
            {
                Despawn();
            }
        }

        private bool TryDespawn()
        {
            if (Vital.Owner != null)
            {
                if (Vital.Owner.IsPlayer && _destroyOnDeath)
                {
                    return true;
                }
            }

            return false;
        }

        private void PlayDeathAnimation()
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.CharacterAnimator == null)
            {
                return;
            }

            Vital.Owner.CharacterAnimator.PlayDeathAnimation();
        }

        private void SendKillGlobalEvent(DamageResult damageResult)
        {
            if (Vital.Owner == null || !Vital.Owner.IsPlayer)
            {
                return;
            }

            if (damageResult == null || damageResult.Attacker == null)
            {
                return;
            }

            if (damageResult.Attacker.IsPlayer)
            {
                _ = GlobalEvent<DamageResult>.Send(GlobalEventType.PLAYER_CHARACTER_KILL_MONSTER, damageResult);
            }
        }

        private void Despawn()
        {
            if (Vital != null)
            {
                _ = CoroutineNextTimer(_delayBeforeDestruction, Vital.Despawn);
            }
        }

        private void SendGlobalEventValueChanged()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                if (Rate >= 0.5f)
                {
                    GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_MORE_HALF_LIFE);
                }
                else
                {
                    GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_LESS_HALF_LIFE);
                }
            }
        }

        public void SendGlobalEventHeal()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_HEAL, Current, Max);
            }
            else
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.MONSTER_CHARACTER_HEAL, Current, Max);
            }
        }

        private void RestoreOnKill(Character attacker)
        {
            if (attacker != null && attacker.MyVital != null)
            {
            }
        }

        private void SendGlobalEventOfRefresh()
        {
            if (Vital.Owner == null)
            {
                return;
            }

            if (Vital.Owner.IsPlayer)
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_REFRESH_LIFE, Current, Max);
            }
            else
            {
                _ = GlobalEvent<int, int>.Send(GlobalEventType.MONSTER_CHARACTER_REFRESH_LIFE, Current, Max);
            }
        }
    }
}