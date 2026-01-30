using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    public class CharacterPattern : XBehaviour
    {
        public string Name;

        [Title("#Time")]
        [InfoBox("패턴 재사용 대기 시간을 설정합니다.\n일정 값을 설정하여 같은 패턴을 사용하지 않도록 합니다.")]
        public float CooldownTime;

        [InfoBox("패턴 사용 후 다음 패턴 대기 시간을 설정합니다.\n0일 경우 즉시 다음 패턴으로 넘어갑니다.")]
        public float WaitDuration;

        [ReadOnly] public bool IsCooldown;
        [ReadOnly] public bool IsWait;

        [Title("#Probability")]
        [Range(0f, 1f)]
        public float ProbabilityToPicked = 1f;

        private CharacterPatternStep[] _patternSteps;
        private Order _patternOrder = new Order();
        private Coroutine _cooldownCoroutine;
        private Coroutine _waitCoroutine;

        private void Awake()
        {
            _patternSteps = GetComponentsInChildren<CharacterPatternStep>();
        }

        public CharacterPatternStep GetStep()
        {
            if (_patternSteps != null)
            {
                if (_patternSteps.Length > _patternOrder.Current)
                {
                    return _patternSteps[_patternOrder.Current];
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}",
                        Name.ToSelectString(), _patternOrder.Current, _patternSteps.Length);
                }
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps가 null입니다. Step을 가져올 수 없습니다.", Name.ToSelectString());
            }

            return null;
        }

        public PatternStepNames GetStepName()
        {
            if (_patternSteps != null)
            {
                if (_patternSteps.Length > _patternOrder.Current)
                {
                    return _patternSteps[_patternOrder.Current].StepName;
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}",
                        Name.ToSelectString(), _patternOrder.Current, _patternSteps.Length);
                }
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps가 null입니다. StepName을 가져올 수 없습니다.", Name.ToSelectString());
            }

            return PatternStepNames.None;
        }

        public int GetStepOrder()
        {
            if (_patternSteps != null)
            {
                int currentOrder = _patternOrder.Current;

                if (_patternSteps.Length > currentOrder)
                {
                    if (_patternSteps[currentOrder].UseRandomOrder)
                    {
                        int randomOrder = RandomEx.Range(0, _patternSteps[currentOrder].OrderMaxIndex);
                        Log.Info(LogTags.Pattern, "{0}, 랜덤 순서를 사용합니다. 범위: 0~{1}, 선택된 값: {2}",
                            Name.ToSelectString(), _patternSteps[currentOrder].OrderMaxIndex, randomOrder);
                        return randomOrder;
                    }
                    else
                    {
                        return _patternSteps[currentOrder].OrderIndex;
                    }
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}",
                        Name.ToSelectString(), currentOrder, _patternSteps.Length);
                }
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps가 null입니다. StepOrder를 가져올 수 없습니다.", Name.ToSelectString());
            }

            return 0;
        }

        public void FirstStep()
        {
            Log.Info(LogTags.Pattern, "{0}, 첫 번째 스텝으로 이동합니다.", Name.ToSelectString());
            _patternOrder.First();
        }

        public void NextStep()
        {
            bool hasNext = _patternOrder.Next();
            Log.Info(LogTags.Pattern, "{0}, 다음 스텝으로 이동합니다. Current: {1}, HasNext: {2}",
                Name.ToSelectString(), _patternOrder.Current, hasNext);
        }

        public void RefreshOrderMax()
        {
            if (_patternSteps != null && _patternSteps.Length > 0)
            {
                _patternOrder.SetMax(_patternSteps.Length - 1);
                Log.Info(LogTags.Pattern, "{0}, Order 최대값을 갱신합니다. Max: {1}", Name.ToSelectString(), _patternSteps.Length - 1);
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps가 null이거나 비어있습니다. Order 최대값을 갱신할 수 없습니다.", Name.ToSelectString());
            }
        }

        public void StartPatternCooldownTime()
        {
            if (CooldownTime > 0f)
            {
                if (_cooldownCoroutine == null)
                {
                    _cooldownCoroutine = StartXCoroutine(ProcessPatternCooldownTime());
                }
                else
                {
                    Log.Error("{0}, 패턴 재사용 대기를 시작할 수 없습니다. 이미 패턴 재사용 대기를 하고 있습니다.", Name.ToSelectString());
                }
            }
        }

        public void StopPatternCooldownTime()
        {
            if (_cooldownCoroutine != null)
            {
                Log.Info(LogTags.Pattern, "{0}, 패턴의 재사용 대기를 중단합니다.", Name.ToSelectString());
            }

            StopXCoroutine(ref _cooldownCoroutine);
        }

        private IEnumerator ProcessPatternCooldownTime()
        {
            Log.Info(LogTags.Pattern, "{0}, 패턴의 재사용 대기를 시작합니다. 대기 시간: {1}", Name.ToSelectString(), CooldownTime.ToSelectString());

            IsCooldown = true;

            yield return new WaitForSeconds(CooldownTime);

            Log.Info(LogTags.Pattern, "{0}, 패턴의 재사용 대기를 종료합니다. 대기 시간: {1}", Name.ToSelectString(), CooldownTime.ToSelectString());

            IsCooldown = false;

            _cooldownCoroutine = null;
        }

        public void StartPatternWaitTime(UnityAction OnCompleted)
        {
            if (WaitDuration.IsZero())
            {
                Log.Info(LogTags.Pattern, "{0}, 대기 시간이 0이므로 즉시 완료 콜백을 실행합니다.", Name.ToSelectString());
                OnCompleted?.Invoke();
                return;
            }

            _waitCoroutine = StartXCoroutine(ProcessPatternWaitTime(OnCompleted));
        }

        private void StopPatternWaitTime()
        {
            StopXCoroutine(ref _waitCoroutine);
        }

        private IEnumerator ProcessPatternWaitTime(UnityAction OnCompleted)
        {
            Log.Info(LogTags.Pattern, "{0}, 패턴 사용 후 다음 패턴 대기를 시작합니다. 대기 시간: {1}", Name.ToSelectString(), WaitDuration.ToSelectString());

            IsWait = true;

            yield return new WaitForSeconds(WaitDuration);

            IsWait = false;

            Log.Info(LogTags.Pattern, "{0}, 패턴 사용 후 다음 패턴 대기를 종료합니다. 대기 시간: {1}", Name.ToSelectString(), WaitDuration.ToSelectString());

            OnCompleted?.Invoke();

            _waitCoroutine = null;
        }
    }
}