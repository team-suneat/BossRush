using Sirenix.OdinInspector;
using System;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> 캐릭터의 주요 리소스를 관리하는 기본 클래스입니다. </summary>
    public class VitalResource : XBehaviour
    {
        [FoldoutGroup("#VitalResource")]
        public Vital Vital;

        [FoldoutGroup("#VitalResource")]
        public int Current;

        [FoldoutGroup("#VitalResource")]
        public int Max;

        [FoldoutGroup("#VitalResource")]
        public Transform DamageTextPoint;

        public event Action<int, int> OnValueChanged;

        public virtual VitalResourceTypes Type => VitalResourceTypes.None;

        public float Rate => Current.SafeDivide(Max);

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            DamageTextPoint = this.FindTransform("Point-DamageText");
            Vital = GetComponent<Vital>();
        }

        protected virtual void Awake()
        {
            if (DamageTextPoint == null)
            {
                DamageTextPoint = transform;
            }
        }

        public virtual void Initialize()
        {
            LoadMaxValue();
            LoadCurrentValue();
        }

        public virtual void RefreshMaxValue(bool shouldAddExcessToCurrent = false)
        {
            LogMaxValueError();
        }

        public void SetCurrentValueByPercentage(float percentageOfMax)
        {
            Current = Mathf.RoundToInt(Max * Mathf.Clamp01(percentageOfMax));

            LogCurrentValueSet(Type, Current, Max);
        }

        public virtual bool AddCurrentValue(int value)
        {
            if (Current != Max)
            {
                if (value != 0)
                {
                    if (Current + value > Max)
                    {
                        Current = Max;
                    }
                    else
                    {
                        Current += value;
                    }

                    LogCurrentValueAdded(Type, value, Current, Max);
                    NotifyValueChanged();
                    SendGlobalEventOfChange();
                    return true;
                }
            }

            return false;
        }

        protected virtual void OnAddCurrentValue(int value)
        {
        }

        public virtual bool UseCurrentValue(int value)
        {
            return UseCurrentValue(value, null);
        }

        public virtual bool UseCurrentValue(int value, DamageResult damageResult)
        {
            if (Current > 0 && value > 0)
            {
                if (Current >= value)
                {
                    Current -= value;
                }
                else
                {
                    Current = 0;
                }

                OnUseCurrencyValue(value);
                return true;
            }

            LogResourceUsageFailure(value);
            return false;
        }

        protected void OnUseCurrencyValue(int value)
        {
            LogResourceUsage(Type, value, Current, Max);
            NotifyValueChanged();
            SendGlobalEventOfChange();
            SendGlobalEventOfUse(value);
        }

        protected void SendGlobalEventOfUse(int useValue)
        {
            if (Vital.Owner != null)
            {
                if (Vital.Owner.IsPlayer)
                {
                    GlobalEvent<int>.Send(GlobalEventType.PLAYER_CHARACTER_USE_VITAL_RESOURCE, useValue);
                }
            }
        }

        protected void SendGlobalEventOfChange()
        {
            // HUD 등에서 직접 구독하는 경우도 있으므로 글로벌 이벤트와 별도로 통지
            NotifyValueChanged();

            if (Vital.Owner != null)
            {
                if (Vital.Owner.IsPlayer)
                {
                    GlobalEvent<int, int>.Send(GlobalEventType.PLAYER_CHARACTER_CHANGE_VITAL_RESOURCE, Current, Max);
                }
                else
                {
                    GlobalEvent<SID, float>.Send(GlobalEventType.MONSTER_CHARACTER_RESTORE_VITAL_RESOURCE, Vital.Owner.SID, Rate);
                }
            }
        }

        public virtual void LoadMaxValue()
        {
            RefreshMaxValue();
        }

        public virtual void LoadCurrentValue()
        {
            Current = Max;
            LogInfo("현재 생명력을 최대 생명력 값으로 불러옵니다. {0}/{1}", Current, Max);
        }

        public virtual void Regenerate(int value)
        {
            if (AddCurrentValue(value))
            {
                OnAddCurrentValue(value);
            }
        }

        public virtual void Recovery(int value)
        {
            if (AddCurrentValue(value))
            {
                OnAddCurrentValue(value);
            }
        }

        protected float FindStatValueByOwner(StatNames statName)
        {
            if (Vital.Owner != null && Vital.Owner.Stat != null)
            {
                return Vital.Owner.Stat.FindValueOrDefault(statName);
            }

            return 0f;
        }

        #region Floating Text

        public UIFloatyText SpawnFloatyText(string content, Transform parent, UIFloatyMoveNames moveType)
        {
            if (GameSetting.Instance.Play.UseDamageText)
            {
                if (moveType == UIFloatyMoveNames.Execution)
                {
                    content = JsonDataManager.FindStringClone(StringDataLabels.FLOATY_EXECUTION);
                }
                else if (content == int.MaxValue.ToString())
                {
                    // 강제 처치 판정 시 플로팅 텍스트를 생성하지 않습니다.
                    return null;
                }

                return ResourcesManager.SpawnFloatyText(content, moveType, parent);
            }

            return null;
        }

        #endregion Floating Text

        #region Log

        protected void LogResourceUsage(VitalResourceTypes type, int value, int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("전투자원({0})을 사용합니다. -{1}, {2}/{3}", type, value.ToErrorString(), current, max);
            }
        }

        protected void LogResourceUsageFailure(int value)
        {
            if (Current <= 0)
            {
                if (Log.LevelWarning)
                {
                    LogWarning("전투자원({0})을 사용할 수 없습니다. 현재 값이 부족합니다: {1}/{2}", Type, Current, Max);
                }
            }
            else if (value <= 0)
            {
                if (Log.LevelError)
                {
                    LogError("전투자원({0})을 사용할 수 없습니다. 잘못된 값이 입력되었습니다: {1}", Type, value);
                }
            }
        }

        protected void LogCurrentValueSet(VitalResourceTypes type, int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("전투자원({0})을 설정합니다. {1}/{2}", type, current, max);
            }
        }

        protected void LogCurrentValueAdded(VitalResourceTypes type, int value, int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("전투자원({0})을 추가합니다. +{1}, {2}/{3}", type, value.ToSelectString(), current, max);
            }
        }

        private void NotifyValueChanged()
        {
            OnValueChanged?.Invoke(Current, Max);
        }

        protected void LogMaxValueRefreshed(int current, int max)
        {
            if (Log.LevelInfo)
            {
                LogInfo("캐릭터의 최대 자원의 능력치를 불러와 적용합니다. {0}/{1}", current, max);
            }
        }

        protected void LogMaxValueError()
        {
            if (Log.LevelError)
            {
                LogError("캐릭터의 최대 자원의 능력치를 불러올 수 없습니다. 바이탈의 캐릭터 또는 능력치 정보를 불러올 수 없습니다.");
            }
        }

        protected void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Info(LogTags.BattleResource, addString + content);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Info(LogTags.BattleResource, addString + content);
                }
            }
        }

        protected void LogInfo(string format, params object[] args)
        {
            if (Log.LevelInfo)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Info(LogTags.BattleResource, addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Info(LogTags.BattleResource, addString + format, args);
                }
            }
        }

        protected void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Warning(LogTags.BattleResource, addString + content);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Warning(LogTags.BattleResource, addString + content);
                }
            }
        }

        protected void LogWarning(string format, params object[] args)
        {
            if (Log.LevelWarning)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Warning(LogTags.BattleResource, addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Warning(LogTags.BattleResource, addString + format, args);
                }
            }
        }

        protected void LogError(string content)
        {
            if (Log.LevelError)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Error(addString + content);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Error(addString + content);
                }
            }
        }

        protected void LogError(string format, params object[] args)
        {
            if (Log.LevelError)
            {
                if (Vital.Owner != null)
                {
                    string addString = $"{Vital.Owner.Name.ToLogString()}({Vital.SID}), ";
                    Log.Error(addString + format, args);
                }
                else
                {
                    string addString = this.GetHierarchyPath() + ", ";
                    Log.Error(addString + format, args);
                }
            }
        }

        #endregion Log
    }
}