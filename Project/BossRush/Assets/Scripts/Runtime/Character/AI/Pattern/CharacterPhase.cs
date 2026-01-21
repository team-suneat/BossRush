using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [System.Serializable]
    public class OnUnlockCharacterPhase : UnityEvent
    {
    }

    public class CharacterPhase : XBehaviour
    {
        public int Index;

        [Range(0f, 1f)]
        [SuffixLabel("조건 체력 비율")]
        public float ConditionHealthRate = 1f;

        public bool IsLocked = true;

        [SuffixLabel("다음 페이즈로 넘어가면 사용하지 않음")]
        public bool NotUseOnNextPhase;

        [FoldoutGroup("#Event")]
        public OnUnlockCharacterPhase OnUnlockCallback;

        private int _maxPhase;
        public BossCharacter Boss { get; private set; }

        public CharacterPattern[] Patterns { get; private set; }
        public int PatternLength => Patterns != null ? Patterns.Length : 0;

#if UNITY_EDITOR

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (Patterns != null)
            {
                for (int i = 0; i < Patterns.Length; i++)
                {
                    Patterns[i].AutoSetting();
                }
            }
        }

        public override void AutoNaming()
        {
            base.AutoNaming();

            SetGameObjectName(string.Format("Phase ({0})", Index));
        }

#endif

        private void Awake()
        {
            Boss = this.FindFirstParentComponent<BossCharacter>();
            Patterns = GetComponentsInChildren<CharacterPattern>();
        }

        public void SetMaxPhase(int currentPhase)
        {
            _maxPhase = currentPhase;
        }

        public bool CheckConditionHealthRate(Character owner)
        {
            if (owner == null)
            {
                return false;
            }

            if (owner.MyVital == null)
            {
                return false;
            }

            if (owner.MyVital.GetRate(VitalResourceTypes.Life) > ConditionHealthRate)
            {
                return false;
            }

            return true;
        }

        public bool TryLock()
        {
            if (NotUseOnNextPhase && Index < _maxPhase)
            {
                return true;
            }

            return false;
        }

        public bool TryUnlock(Character owner)
        {
            if (!IsLocked)
            {
                return false;
            }

            if (owner == null || owner.MyVital == null)
            {
                return false;
            }

            if (owner.MyVital.GetRate(VitalResourceTypes.Life) > ConditionHealthRate)
            {
                return false;
            }

            return true;
        }

        public void Lock()
        {
            IsLocked = true;
            Log.Info(LogTags.Pattern, "패턴 페이즈를 잠금합니다. {0}", Index);
        }

        public void Unlock()
        {
            if (!IsLocked)
            {
                return;
            }

            IsLocked = false;

            OnUnlockCallback?.Invoke();

            SendGlobalEvent();

            Log.Info(LogTags.Pattern, "패턴 페이즈를 잠금해제합니다. {0}", Index);
        }

        public void SendGlobalEvent()
        {
            if (Boss == null || _maxPhase != Index)
            {
                return;
            }

            if (Mathf.Approximately(ConditionHealthRate, 1f))
            {
                return;
            }

            // GlobalEvent<BossCharacter>.Send(GlobalEventType.BOSS_CHARACTER_PATTERN_PHASE_CHANGED, Boss);
        }
    }
}