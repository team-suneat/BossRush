using Lean.Pool;
using System.Collections;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        public void ActivateStackRenderers(BuffStack buffStackRenderer)
        {
            _buffStackRenderer = buffStackRenderer;
        }

        public void DeactivateStackRenderers()
        {
            if (_buffStackRenderer != null)
            {
                _buffStackRenderer.DeactivateStacks();
                _buffStackRenderer = null;
            }
        }

        public bool CheckRemovingStackSequentially()
        {
            return _removeStackCoroutine != null;
        }

        public void StartRemoveStackSequentially()
        {
            LogProgress("버프의 스택을 순차적으로 삭제합니다.");

            if (Stack > 0 && AssetData.ReleaseTimeByStack > 0)
            {
                StopDurationTimer();
                StopRepetitiveTimer();
                DeactivateStackRenderers();

                if (_removeStackCoroutine == null)
                {
                    _removeStackCoroutine = StartXCoroutine(ProcessRemoveStackSequentially());
                }
            }
            else
            {
                Owner.Buff.Remove(Name);
            }
        }

        private void StopRemoveStackTimer()
        {
            StopXCoroutine(ref _removeStackCoroutine);
        }

        private IEnumerator ProcessRemoveStackSequentially()
        {
            while (Stack > 0)
            {
                yield return new WaitForSeconds(AssetData.ReleaseTimeByStack);
                RemoveStackCount();
            }

            _removeStackCoroutine = null;
            Owner.Buff.Remove(Name);
        }

        private void SetBuffStackIcon()
        {
            _buffStackRenderer?.ActivateStacks(Stack);
        }

        private void LoadBuffMaxStackCount()
        {
            // 능력치 기반 최대 스택 계산
            if (TrySetMaxStackByStat())
            {
                return;
            }

            // 기본 레벨 기반 최대 스택 계산
            SetMaxStackByLevel();
        }

        private bool TrySetMaxStackByStat()
        {
            if (AssetData.MaxStackByStat == StatNames.None || !Owner.Stat.ContainsKey(AssetData.MaxStackByStat))
            {
                return false;
            }

            if (AssetData.IsAddMaxStackByStat)
            {
                int baseStack = StatEx.GetValueByLevel(AssetData.MaxStack, AssetData.MaxStackByLevel, Level);
                int bonus = Mathf.Max(0, Owner.Stat.FindValueOrDefaultToInt(AssetData.MaxStackByStat));
                MaxStack = baseStack + bonus;
            }
            else
            {
                MaxStack = Mathf.Max(0, Owner.Stat.FindValueOrDefaultToInt(AssetData.MaxStackByStat));
            }

            if (MaxStack == 0)
            {
                Log.Warning("능력치({0})에 따른 버프({1})의 최대 스택이 0입니다.", AssetData.MaxStackByStat, Name);
            }

            LogProgress("버프의 최대 스택을 설정합니다. {0}", MaxStack.ToSelectString(0));
            return true;
        }

        private void SetMaxStackByLevel()
        {
            MaxStack = StatEx.GetValueByLevel(AssetData.MaxStack, AssetData.MaxStackByLevel, Level);
            LogProgress("버프의 최대 스택을 설정합니다. {0}", MaxStack.ToSelectString(0));
        }

        public bool AddStackCount(int addStack = 1)
        {
            if (MaxStack <= 0)
            {
                return false;
            }

            if (MaxStack <= Stack)
            {
                return HandleMaxStackReached();
            }

            return ProcessStackAddition(addStack);
        }

        private bool HandleMaxStackReached()
        {
            if (MaxStack < Stack)
            {
                Stack = MaxStack;
            }

            if (AssetData.RemoveBuffOnMaxStack)
            {
                LogProgress("최대 스택에 도달한 버프를 삭제합니다.");
                Owner.Buff.Remove(this);
            }

            return false;
        }

        private bool ProcessStackAddition(int addStack)
        {
            Stack = Mathf.Min(Stack + addStack, MaxStack);

            UpdateStackData();

            if (AssetData.DurationByStack > 0)
            {
                SetupDuration();
            }

            if (Log.LevelProgress)
            {
                LogProgress("버프의 스택을 {2} 추가합니다. 결과: {0}/{1}", Stack.ToValueString(), MaxStack.ToValueString(), addStack.ToSelectString(0));
            }

            SpawnSoliloquyForStat(+1);
            StartXCoroutine(SendAddedBuffStackGlobalEvent(Name, Stack, MaxStack));

            return true;
        }

        private void UpdateStackData()
        {
            SetBuffStackIcon();
        }

        public bool SetStackCount(int stack)
        {
            if (Stack == stack || MaxStack <= 0 || MaxStack < stack)
            {
                return false;
            }

            Stack = stack;
            UpdateStackData();

            LogProgress("버프의 스택을 설정합니다. {0}/{1}", Stack, MaxStack);

            if (MaxStack == stack && AssetData.RemoveBuffOnMaxStack)
            {
                LogProgress("최대 스택에 도달한 버프를 삭제합니다.");
                Owner.Buff.Remove(this);
            }

            StartXCoroutine(SendAddedBuffStackGlobalEvent(Name, Stack, MaxStack));
            return true;
        }

        public bool RemoveStackCount()
        {
            if (Stack <= 0)
            {
                return false;
            }

            Stack--;
            SpawnSoliloquyForStat(-1);
            LogProgress("버프의 스택을 삭제합니다. {0}/{1}", Stack, MaxStack);

            return true;
        }

        private IEnumerator SendAddedBuffStackGlobalEvent(BuffNames buffName, int buffStack, int buffMaxStack)
        {
            yield return null;

            if (Owner.IsPlayer)
            {
                GlobalEvent<BuffNames, int, int>.Send(GlobalEventType.PLAYER_CHARACTER_ADD_BUFF_STACK, buffName, buffStack, buffMaxStack);
            }
            else
            {
                GlobalEvent<BuffNames, int, int, SID>.Send(GlobalEventType.MONSTER_CHARACTER_ADD_BUFF_STACK, buffName, buffStack, buffMaxStack, Owner.SID);
            }
        }
    }
}