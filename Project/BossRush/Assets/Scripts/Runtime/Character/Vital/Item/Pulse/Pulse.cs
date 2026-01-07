using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 패링 게이지를 관리하는 클래스입니다. </summary>
    public class Pulse : VitalResource
    {
        #region Field

        [Title("#Pulse")]
        [FoldoutGroup("#Parry")]
        [SuffixLabel("패링 시전 시 소모량 (0.5칸 = 1 단위)")]
        public int ParryCostOnUse = 1;

        [FoldoutGroup("#Parry")]
        [SuffixLabel("패링 성공 시 회복량 (1칸 = 2 단위)")]
        public int ParryRewardOnSuccess = 2;

        [FoldoutGroup("#Parry")]
        [SuffixLabel("공격 히트당 획득량 (최대치의 %)")]
        public float AttackHitGaugeGainPercent = 10f;

        [FoldoutGroup("#Parry")]
        [SuffixLabel("패링 판정 윈도우 지속 시간 (초)")]
        public float ParryWindowDuration = 0.2f;

        [FoldoutGroup("#Empty")]
        [SuffixLabel("Empty 상태 고정 지속 시간 (초)")]
        public float EmptyLockDuration = 1f;

        [FoldoutGroup("#Empty")]
        [SuffixLabel("Empty 상태 자동 회복 지속 시간 (초)")]
        public float EmptyRegenDuration = 5f;

        [ReadOnly]
        [FoldoutGroup("#State")]
        [SuffixLabel("Empty 상태 여부")]
        public bool IsEmpty;

        [ReadOnly]
        [FoldoutGroup("#State")]
        [SuffixLabel("Empty Lock 상태 여부")]
        public bool IsEmptyLocked;

        [ReadOnly]
        [FoldoutGroup("#State")]
        [SuffixLabel("자동 회복 중 여부")]
        public bool IsRegenerating;

        private Coroutine _emptyRegenCoroutine;

        #endregion Field

        #region Parameter

        public override VitalResourceTypes Type => VitalResourceTypes.Pulse;

        /// <summary> 실제 칸 수 (0.0 ~ MaxSlots). 0.5칸 단위로 표시됩니다. </summary>
        public float GaugeInSlots => Current / 2f;

        /// <summary> 최대 칸 수. 기본 3칸 = 6 단위. </summary>
        public float MaxSlots => Max / 2f;

        /// <summary> 패링 사용 가능 여부 (0.5칸 이상). </summary>
        public bool CanParry => Current >= ParryCostOnUse;

        #endregion Parameter

        protected override void Awake()
        {
            base.Awake();

            // 기본값 설정: 3칸 = 6 단위
            if (Max == 0)
            {
                Max = 6; // 3칸
                Current = 6; // 전투 시작 시 풀 게이지
            }
        }

        public override void Initialize()
        {
            LogInfo("패링 게이지를 초기화합니다.");

            base.Initialize();

            // 전투 시작 시 항상 풀 게이지
            LoadCurrentValue();

            IsEmpty = false;
            IsEmptyLocked = false;
            IsRegenerating = false;
        }

        public override void LoadCurrentValue()
        {
            Current = Max;
            LogInfo("현재 패링 게이지를 최대 게이지 값으로 불러옵니다. {0}/{1} ({2:F1}칸)", Current, Max, GaugeInSlots);
        }

        public override void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            if (Vital == null || Vital.Owner == null || Vital.Owner.Stat == null)
            {
                LogWarning("최대 펄스를 불러올 수 없습니다. 바이탈, 소유 캐릭터, 능력치 시스템 중 최소 하나가 없습니다.");
                return;
            }

            float statValue = Vital.Owner.Stat.FindValueOrDefault(StatNames.Pulse);
            int maxPulseByStat = Mathf.RoundToInt(statValue);
            if (maxPulseByStat > 0)
            {
                int previousMax = Max;
                Max = maxPulseByStat;

                LogInfo("캐릭터의 능력치에 따라 최대 펄스를 갱신합니다. {0}/{1} ({2:F1}칸)", Current, Max, MaxSlots);

                if (shouldAddExcessToCurrent && Max > previousMax)
                {
                    Current += Max - previousMax;
                }
                if (Current > Max)
                {
                    Current = Max;

                    LogInfo("캐릭터의 남은 펄스가 최대 펄스보다 크다면, 최대 펄스로 설정합니다. {0}/{1} ({2:F1}칸)", Current, Max, MaxSlots);
                }
            }
        }

        /// <summary> 패링 시전 시 게이지 소모. </summary>
        public bool TryUseParry()
        {
            if (!CanParry)
            {
                LogWarning("패링 게이지가 부족합니다. 필요: {0}, 현재: {1}", ParryCostOnUse, Current);
                return false;
            }

            if (UseCurrentValue(ParryCostOnUse))
            {
                LogInfo("패링 게이지를 소모합니다. -{0}, {1}/{2} ({3:F1}칸)", ParryCostOnUse, Current, Max, GaugeInSlots);

                // Empty 상태 체크
                CheckEmptyState();

                return true;
            }

            return false;
        }

        /// <summary> 패링 성공 시 게이지 회복. </summary>
        public void OnParrySuccess()
        {
            if (AddCurrentValue(ParryRewardOnSuccess))
            {
                LogInfo("패링 성공! 게이지를 회복합니다. +{0}, {1}/{2} ({3:F1}칸)", ParryRewardOnSuccess, Current, Max, GaugeInSlots);

                // Empty 상태 해제 (회복으로 인해)
                if (IsEmpty)
                {
                    ExitEmptyState();
                }
            }
        }

        /// <summary> 공격 히트 시 게이지 획득. </summary>
        public void OnAttackHit()
        {
            // 최대치의 일정 비율만큼 획득
            int gainAmount = Mathf.RoundToInt(Max * (AttackHitGaugeGainPercent / 100f));

            if (gainAmount > 0 && AddCurrentValue(gainAmount))
            {
                LogInfo("공격 히트로 게이지를 획득합니다. +{0}, {1}/{2} ({3:F1}칸)", gainAmount, Current, Max, GaugeInSlots);

                // Empty 상태 해제 (회복으로 인해)
                if (IsEmpty)
                {
                    ExitEmptyState();
                }
            }
        }

        /// <summary> Empty 상태 체크 및 진입. </summary>
        private void CheckEmptyState()
        {
            if (Current <= 0 && !IsEmpty)
            {
                EnterEmptyState();
            }
        }

        /// <summary> Empty 상태 진입. </summary>
        private void EnterEmptyState()
        {
            IsEmpty = true;
            IsEmptyLocked = true;
            IsRegenerating = false;

            LogWarning("패링 게이지가 0에 도달하여 Empty 상태로 진입합니다.");

            // Empty Lock Duration 후 자동 회복 시작
            if (_emptyRegenCoroutine != null)
            {
                StopCoroutine(_emptyRegenCoroutine);
            }
            _emptyRegenCoroutine = StartCoroutine(EmptyRegenCoroutine());
        }

        /// <summary> Empty 상태 해제. </summary>
        private void ExitEmptyState()
        {
            if (!IsEmpty)
            {
                return;
            }

            IsEmpty = false;
            IsEmptyLocked = false;
            IsRegenerating = false;

            if (_emptyRegenCoroutine != null)
            {
                StopCoroutine(_emptyRegenCoroutine);
                _emptyRegenCoroutine = null;
            }

            LogInfo("Empty 상태를 해제합니다. 현재 게이지: {0}/{1} ({2:F1}칸)", Current, Max, GaugeInSlots);
        }

        /// <summary> Empty 상태 자동 회복 코루틴. </summary>
        private IEnumerator EmptyRegenCoroutine()
        {
            // EmptyLockDuration 동안 대기
            yield return new WaitForSeconds(EmptyLockDuration);

            IsEmptyLocked = false;
            IsRegenerating = true;

            LogInfo("Empty 상태 자동 회복을 시작합니다. {0}초 동안 회복됩니다.", EmptyRegenDuration);

            // EmptyRegenDuration 동안 0 → Max까지 회복
            float elapsedTime = 0f;
            int startValue = Current;

            while (elapsedTime < EmptyRegenDuration && IsEmpty)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / EmptyRegenDuration;

                int targetValue = Mathf.RoundToInt(Mathf.Lerp(startValue, Max, progress));
                Current = targetValue;

                // 최대치에 도달하면 종료
                if (Current >= Max)
                {
                    Current = Max;
                    ExitEmptyState();
                    yield break;
                }

                yield return null;
            }

            // 시간 초과 시 강제로 최대치 설정
            if (IsEmpty)
            {
                Current = Max;
                ExitEmptyState();
            }
        }

        protected override void OnUseCurrencyValue(int value)
        {
            base.OnUseCurrencyValue(value);

            // Empty 상태 체크
            CheckEmptyState();
        }

        protected override void OnAddCurrentValue(int value)
        {
            base.OnAddCurrentValue(value);

            // Empty 상태 해제 체크
            if (IsEmpty && Current > 0)
            {
                ExitEmptyState();
            }
        }
    }
}

