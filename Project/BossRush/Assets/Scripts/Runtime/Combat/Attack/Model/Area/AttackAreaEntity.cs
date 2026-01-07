using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackAreaEntity : AttackEntity
    {
        #region 토글 (Toggle)

        [FoldoutGroup("#AttackAreaEntity-Toggle")]
        [LabelText("오직 애니메이션 공격만 활성화")]
        public bool LateDamageHit = false;

        [FoldoutGroup("#AttackAreaEntity-Toggle")]
        [LabelText("공격 후 충돌체 비활성화")]
        public bool DisableColliderOnHit;

        [FoldoutGroup("#AttackAreaEntity-Toggle")]
        [LabelText("초기화 시 충돌체 활성화")]
        public bool UseActiveColliderOnInit;

        #endregion 토글 (Toggle)

        #region 목표 (Target)

        [FoldoutGroup("#AttackAreaEntity-Targets")]
        public LayerMask TargetLayerMask;

        [FoldoutGroup("#AttackAreaEntity-Targets")]
        public bool UseIgnoreHitTarget = true;

        #endregion 목표 (Target)

        #region 시간 (Time)

        public enum Times
        {
            FixedFrame,
            Duration,
        }

        [FoldoutGroup("#AttackAreaEntity-Times")]
        public Times AreaTime;

        [FoldoutGroup("#AttackAreaEntity-Times")]
        public float InitialApplyDelay = 0f;

        [FoldoutGroup("#AttackAreaEntity-Times")]
        [EnableIf("AreaTime", Times.FixedFrame)]
        [SuffixLabel("충돌체 활성화 프레임 수")]
        public int ActiveFrameCount = 1;

        [FoldoutGroup("#AttackAreaEntity-Times")]
        [EnableIf("AreaTime", Times.Duration)]
        [SuffixLabel("충돌체 활성화 시간 / 간격 활성화 사용시 전체 활성화 지속시간")]
        public float ActiveDuration = 0.02f;

        [FoldoutGroup("#AttackAreaEntity-Times")]
        public bool UseIntervalActive;

        [FoldoutGroup("#AttackAreaEntity-Times/Interval")]
        [EnableIf("UseIntervalActive")]
        [SuffixLabel("충돌체 활성화 시간")]
        public float ActiveTime;

        [FoldoutGroup("#AttackAreaEntity-Times/Interval")]
        [EnableIf("UseIntervalActive")]
        [SuffixLabel("충돌체 활성화 후 비활성화 시간")]
        public float ActiveInterval = 0.02f;

        #endregion 시간 (Time)

        #region 능력치 (Stat)

        [FoldoutGroup("#AttackAreaEntity-Stat")]
        [Tooltip("영역 공격의 영역을 배율로 늘려주는 능력치 이름입니다. 캐릭터의 능력치에 따라 영역이 늘어납니다.")]
        [SuffixLabel("추가 영역 배율 능력치 이름*")]
        public StatNames StatNameOfSize;

        [FoldoutGroup("#AttackAreaEntity-Stat")]
        public string StatNameOfSizeString;

        [FoldoutGroup("#AttackAreaEntity-Stat")]
        [Tooltip("영역 공격의 지속시간을 추가하는 능력치 이름입니다. 캐릭터의 능력치에 따라 지속시간이 늘어납니다.")]
        [SuffixLabel("추가 지속시간 능력치 이름*")]
        public StatNames StatNameOfActiveDuration;

        [FoldoutGroup("#AttackAreaEntity-Stat")]
        public string StatNameOfActiveDurationString;

        #endregion 능력치 (Stat)

        #region 이벤트 (Event)

        public delegate void OnHitDelegate(Vector3 feedbackPosition);

        [FoldoutGroup("#AttackAreaEntity-Events")]
        public OnHitDelegate OnHitDamageable;

        [FoldoutGroup("#AttackAreaEntity-Events")]
        public OnHitDelegate OnHitNonDamageable;

        [FoldoutGroup("#AttackAreaEntity-Events")]
        public OnHitDelegate OnKill;

        #endregion 이벤트 (Event)

        #region 피해 점감(Decrescence Damage)

        [FoldoutGroup("#AttackTargetEntity-Decrescence Damage")]
        public int AdditionalApplyCount;

        #endregion 피해 점감(Decrescence Damage)

        private Collider2D _attackCollider; // 공격 충돌체
        private Collider2D _collidingCollider; // 충돌한 충돌체
        private Vital _colliderVital; // 충돌한 충돌체의 바이탈
        private List<GameObject> _ignoredGameObjects; // 충돌을 무시하는 게임 오브젝트
        private float _activeDuration; // 충돌체 활성화 지속 시간
        private Coroutine _timerCoroutine; // 지속 타이머
        private int _hitCountAtTime; // 활성화시 1회에 공격한 횟수

        //

        private void Awake()
        {
            _attackCollider = GetComponent<Collider2D>();
            if (_attackCollider != null)
            {
                if (!_attackCollider.isTrigger) { _attackCollider.isTrigger = true; }
            }
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            DeactivateCollider();
            RefreshArea();
            RefreshDuration();

            if (StatNameOfSize != StatNames.None || StatNameOfActiveDuration != StatNames.None)
            {
                Owner.Stat.RegisterOnRefresh(OnRefreshStat);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            _timerCoroutine = null; // StopCoroutine 호출 없이 핸들만 무효화

            ClearTargetsOfChainLightning();

            if (StatNameOfSize != StatNames.None || StatNameOfActiveDuration != StatNames.None)
            {
                Owner.Stat.RegisterOnRefresh(OnRefreshStat);
            }
        }

        public override void Initialization()
        {
            base.Initialization();

            if (UseActiveColliderOnInit)
            {
                ActivateCollider();
            }
        }

        public override void Activate()
        {
            if (DetermineActivate())
            {
                base.Activate();

                ClearIgnoringObjects();
                ClearTargetsOfChainLightning();

                StartTimer();

                base.OnActivate();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();

            if (_timerCoroutine == null && _attackCollider != null && !_attackCollider.enabled)
            {
                LogProgress("공격 지속 타이머가 없고, 피해를 주는 충돌체가 활성화가 아닐 때는 이 독립체가 비활성화 상태라 판단합니다. 영역 공격 독립체를 비활성화할 수 없습니다.");
                return;
            }

            LogInfo("영역 공격 독립체를 비활성화합니다.");

            DeactivateCollider();
            ClearIgnoringObjects();
            StopTimer();
        }

        public override void OnOwnerDeath()
        {
            base.OnOwnerDeath();

            Deactivate();
        }

        public override void Apply()
        {
            LogInfo("영역 공격 독립체를 적용합니다. 레벨:{0} ", Level);

            base.Apply();

            ActivateCollider();
            ActivateChainLightning();

            if (_hitCountAtTime != 0)
            {
                _hitCountAtTime = 0;
            }
        }

        public override void SetOwner(Character ownerCharacter)
        {
            base.SetOwner(ownerCharacter);

            _damageCaculator?.SetAttacker(ownerCharacter);
        }

        public override void SetTarget(Vital targetVital)
        {
            if (targetVital != null)
            {
                base.SetTarget(targetVital);

                _damageCaculator?.SetTargetVital(targetVital);
            }
        }

        #region Collider

        private void ActivateCollider()
        {
            LogInfo("영역 공격의 충돌체를 활성화합니다.");

            if (_attackCollider != null)
            {
                _attackCollider.enabled = true;
            }
        }

        private void DeactivateCollider()
        {
            LogInfo("영역 공격의 충돌체를 비활성화합니다.");

            if (_attackCollider != null)
            {
                _attackCollider.enabled = false;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnEnterArea(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_collidingCollider == collision)
            {
                _colliderVital = null;
                _collidingCollider = null;
            }
        }

        public void OnEnterArea(Collider2D collision)
        {
            _collidingCollider = collision;

            if (TryColliding())
            {
                if (TrySetColliderVital())
                {
                    AddIgnoreGameObject(collision.gameObject);

                    OnCollideWithDamageable();
                }
                else
                {
                    OnCollideWithNonDamageable();
                }
            }
        }

        private bool TryColliding()
        {
            if (_collidingCollider == null)
            {
                LogWarning("충돌체가 null입니다.");
                return false;
            }

            if (!AssetData.IsValid())
            {
                LogError("AssetData가 유효하지 않습니다.");
                return false;
            }

            if (ContainsIgnoreGameObject(_collidingCollider.gameObject))
            {
                // 이미 공격한 바이탈이면서, 무시하는 바이탈에 등록되어있다면
                return false;
            }
            else if (!_collidingCollider.CompareTag(GameTags.Vital))
            {
                // 충돌체가 바이탈 태그를 가지지 않았다면
                return false;
            }
            else if (!LayerEx.IsInMask(_collidingCollider.gameObject.layer, TargetLayerMask))
            {
                // 충돌체가 목표 레이어를 가지지 않았다면
                return false;
            }

            return true;
        }

        private bool TrySetColliderVital()
        {
            _colliderVital = VitalManager.Instance.Find(_collidingCollider);
            if (_colliderVital == null)
            {
                return false;
            }
            if (_colliderVital.Life == null)
            {
                return false;
            }
            if (!_colliderVital.IsAlive)
            {
                return false;
            }
            if (_colliderVital.Life.CheckInvulnerable())
            {
                return false;
            }

            return true;
        }

        protected void OnCollideWithDamageable()
        {
            LogInfo("부딪힌 적을 공격합니다: {0}", _colliderVital.GetHierarchyPath());

            _hitCountAtTime += 1;

            // 영역 밖으로 벗어나면 함수 중간에 할당된 Vital과 Collider가 초기화됩니다. 따로 저장해둡니다.
            Vital colliderVital = _colliderVital;
            Collider2D collidingCollider = _collidingCollider;

            if (ApplyToTargetVital(colliderVital))
            {
                if (colliderVital.IsAlive)
                {
                    TriggerAttackOnHitDamageableFeedback(collidingCollider.transform.position);
                }

                AddTargetOfChainLightning(collidingCollider.transform);

                if (!colliderVital.IsAlive)
                {
                    TriggerAttackOnKillFeedback(collidingCollider.transform.position);
                }
            }

            OnAttack(true);
        }

        protected void OnCollideWithNonDamageable()
        {
            LogInfo("부딪힌 적에게 피해를 입히지 못합니다.");

            TriggerAttackOnHitNonDamageableFeedback(_collidingCollider.transform.position);

            OnAttack(false);
        }

        private bool ApplyToTargetVital(Vital targetVital)
        {
            _damageCaculator.SetTargetVital(targetVital);
            _damageCaculator.Execute();

            DamageResult damageResult;
            bool _isApplyAnyDamage = false;

            if (_damageCaculator.DamageResults.IsValid())
            {
                for (int i = 0; i < _damageCaculator.DamageResults.Count; i++)
                {
                    damageResult = _damageCaculator.DamageResults[i];
                    if (damageResult.DamageType.IsHeal())
                    {
                        _colliderVital.Heal(damageResult.DamageValueToInt);
                        _isApplyAnyDamage = true;
                    }
                    else if (!_colliderVital.CheckDamageImmunity(damageResult))
                    {
                        if (_colliderVital.TakeDamage(damageResult))
                        {
                            _isApplyAnyDamage = true;
                        }
                    }
                }
            }
            else
            {
                LogError("공격 독립체의 피해 결과값이 설정되지 않았습니다. {0}", Name.ToLogString());
            }

            return _isApplyAnyDamage;
        }

        #endregion Collider

        #region Ignore

        private bool ContainsIgnoreGameObject(GameObject target)
        {
            if (UseIgnoreHitTarget)
            {
                if (_ignoredGameObjects != null)
                {
                    if (_ignoredGameObjects.Contains(target))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void AddIgnoreGameObject(GameObject target)
        {
            if (UseIgnoreHitTarget)
            {
                _ignoredGameObjects ??= new List<GameObject>();
                _ignoredGameObjects.Add(target);
            }
        }

        public void RemoveIgnoringObject(GameObject target)
        {
            if (UseIgnoreHitTarget)
            {
                _ = (_ignoredGameObjects?.Remove(target));
            }
        }

        public void ClearIgnoringObjects()
        {
            _ignoredGameObjects?.Clear();
        }

        #endregion Ignore

        #region Timer

        private void StartTimer()
        {
            if (_timerCoroutine == null)
            {
                _timerCoroutine = StartXCoroutine(ProcessArea());

                LogProgress("영역 공격의 타이머를 시작합니다.");
            }
            else
            {
                LogWarning("영역 공격의 타이머를 시작할 수 없습니다. 이미 진행중인 공격 타이머가 존재합니다.");
            }
        }

        private void StopTimer()
        {
            if (_timerCoroutine != null)
            {
                StopCoroutine(_timerCoroutine);
                _timerCoroutine = null;
                LogProgress("영역 공격의 타이머를 취소합니다. {0}");
            }
        }

        private IEnumerator ProcessArea()
        {
            if (InitialApplyDelay > 0)
            {
                yield return new WaitForSeconds(InitialApplyDelay);
            }

            Apply();

            if (AreaTime == Times.FixedFrame)
            {
                yield return new WaitForSeconds(Time.fixedDeltaTime * (ActiveFrameCount + 1));
            }
            else if (AreaTime == Times.Duration)
            {
                if (UseIntervalActive)
                {
                    InfiniteLoopDetector.Reset();

                    float elapsedTime = 0f;
                    while (_activeDuration < 0 || _activeDuration > elapsedTime)
                    {
                        yield return new WaitForSeconds(ActiveTime);
                        elapsedTime += ActiveTime;

                        DeactivateCollider();

                        yield return new WaitForSeconds(ActiveInterval);
                        elapsedTime += ActiveInterval;

                        Apply();

                        LogInfo("{0}초마다 {1}초동안 피해를 줍니다.", ActiveInterval, ActiveTime);
                        InfiniteLoopDetector.Run();
                    }
                }
                else if (_activeDuration >= 0)
                {
                    yield return new WaitForSeconds(_activeDuration);
                }
                else
                {
                    LogInfo("지속시간이 음수라면 비활성화하지 않습니다.");
                    yield break;
                }
            }

            Deactivate();
        }

        #endregion Timer

        #region Stat

        private void OnRefreshStat(StatNames statName, float changedValue)
        {
            if (StatNameOfSize == statName)
            {
                RefreshArea();
            }
            if (StatNameOfActiveDuration == statName)
            {
                RefreshDuration();
            }
        }

        private void RefreshArea()
        {
            if (Owner != null)
            {
                float areaMultiplier = 1 + Owner.Stat.FindValueOrDefault(StatNameOfSize);

                if (_attackCollider is BoxCollider2D)
                {
                    BoxCollider2D boxCollider = _attackCollider as BoxCollider2D;
                    boxCollider.size *= areaMultiplier;
                }
                else if (_attackCollider is CircleCollider2D)
                {
                    CircleCollider2D circleCollider = _attackCollider as CircleCollider2D;
                    circleCollider.radius *= areaMultiplier;
                }
            }
        }

        private void RefreshDuration()
        {
            if (ActiveDuration < 0)
            {
                LogInfo("지속시간이 음수라면 영원히 지속합니다.");
                _activeDuration = -1;

                return;
            }

            _activeDuration = ActiveDuration;
            if (Owner != null)
            {
                _activeDuration += Owner.Stat.FindValueOrDefault(StatNameOfActiveDuration);
            }
        }

        #endregion Stat
    }
}