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
        public float CooldownTime;

        public float DelayTime;

        [ReadOnly] public bool IsCooldown;
        [ReadOnly] public bool IsWait;

        [Title("#Order")]
        public Order Order;

        [Title("#Probability")]
        public float ProbabilityToPicked = 1f;

        [Title("#Step")]
        public CharacterPatternStep[] Steps;

        private Coroutine _cooldownCoroutine;
        private Coroutine _waitCoroutine;

        public override void AutoSetting()
        {
            base.AutoSetting();

            RefreshOrderMax();

            if (Steps != null)
            {
                Log.Info(LogTags.Pattern, "{0}, AutoSetting 시작. Steps 개수: {1}", Name.ToSelectString(), Steps.Length);
                for (int i = 0; i < Steps.Length; i++)
                {
                    Steps[i].AutoSetting();
                }
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps가 null입니다. AutoSetting을 수행할 수 없습니다.", Name.ToSelectString());
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Steps = GetComponentsInChildren<CharacterPatternStep>();

            if (Steps != null)
            {
                Log.Info(LogTags.Pattern, "{0}, AutoGetComponents 완료. Steps 개수: {1}", Name.ToSelectString(), Steps.Length);
                for (int i = 0; i < Steps.Length; i++)
                {
                    Steps[i].AutoGetComponents();
                }
            }
            else
            {
                Log.Warning(LogTags.Pattern, "{0}, Steps를 찾을 수 없습니다. AutoGetComponents를 수행할 수 없습니다.", Name.ToSelectString());
            }
        }

        public CharacterPatternStep GetStep()
        {
            if (Steps != null)
            {
                if (Steps.Length > Order.Current)
                {
                    return Steps[Order.Current];
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}", 
                        Name.ToSelectString(), Order.Current, Steps.Length);
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
            if (Steps != null)
            {
                if (Steps.Length > Order.Current)
                {
                    return Steps[Order.Current].StepName;
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}", 
                        Name.ToSelectString(), Order.Current, Steps.Length);
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
            if (Steps != null)
            {
                int currentOrder = Order.Current;

                if (Steps.Length > currentOrder)
                {
                    if (Steps[currentOrder].UseRandomOrder)
                    {
                        int randomOrder = RandomEx.Range(0, Steps[currentOrder].OrderMaxIndex);
                        Log.Info(LogTags.Pattern, "{0}, 랜덤 순서를 사용합니다. 범위: 0~{1}, 선택된 값: {2}", 
                            Name.ToSelectString(), Steps[currentOrder].OrderMaxIndex, randomOrder);
                        return randomOrder;
                    }
                    else
                    {
                        return Steps[currentOrder].OrderIndex;
                    }
                }
                else
                {
                    Log.Warning(LogTags.Pattern, "{0}, Step 인덱스가 범위를 벗어났습니다. Current: {1}, Steps.Length: {2}", 
                        Name.ToSelectString(), currentOrder, Steps.Length);
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
            Order.First();
        }

        public void NextStep()
        {
            bool hasNext = Order.Next();
            Log.Info(LogTags.Pattern, "{0}, 다음 스텝으로 이동합니다. Current: {1}, HasNext: {2}", 
                Name.ToSelectString(), Order.Current, hasNext);
        }

        public void RefreshOrderMax()
        {
            if (Steps != null && Steps.Length > 0)
            {
                Order.SetMax(Steps.Length - 1);
                Log.Info(LogTags.Pattern, "{0}, Order 최대값을 갱신합니다. Max: {1}", Name.ToSelectString(), Steps.Length - 1);
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
            if (DelayTime.IsZero())
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
            Log.Info(LogTags.Pattern, "{0}, 패턴 사용 후 다음 패턴 대기를 시작합니다. 대기 시간: {1}", Name.ToSelectString(), DelayTime.ToSelectString());

            IsWait = true;

            yield return new WaitForSeconds(DelayTime);

            IsWait = false;

            Log.Info(LogTags.Pattern, "{0}, 패턴 사용 후 다음 패턴 대기를 종료합니다. 대기 시간: {1}", Name.ToSelectString(), DelayTime.ToSelectString());

            OnCompleted?.Invoke();

            _waitCoroutine = null;
        }
    }
}