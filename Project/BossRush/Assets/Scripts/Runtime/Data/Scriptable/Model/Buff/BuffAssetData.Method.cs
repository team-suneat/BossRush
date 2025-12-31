using System;
using System.Linq;

namespace TeamSuneat.Data
{
    public partial class BuffAssetData : ScriptableData<int>
    {
        public override int GetKey()
        {
            return BitConvert.Enum32ToInt(Name);
        }

        public void Validate()
        {
            if (IsChangingAsset)
            {
                return;
            }

            if (!EnumEx.ConvertTo(ref Target, TargetAsString))
            {
                Log.Error("Buff Asset 내 Target 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Application, ApplicationAsString))
            {
                Log.Error("Buff Asset 내 Application 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Type, TypeAsString))
            {
                Log.Error("Buff Asset 내 Type 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref StateEffect, StateEffectAsString))
            {
                Log.Error("Buff Asset 내 StateEffect 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Incompatible, IncompatibleAsString))
            {
                Log.Error("Buff Asset 내 Incompatible 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref IncompatibleStateEffect, IncompatibleStateEffectAsString))
            {
                Log.Error("Buff Asset 내 IncompatibleStateEffect 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref MaxStackByStat, MaxStackByStatAsString))
            {
                Log.Error("Buff Asset 내 MaxStackByStat 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref DurationByStat, DurationByStatAsString))
            {
                Log.Error("Buff Asset 내 DurationByStat 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Hitmark, HitmarkAsString))
            {
                Log.Error("Buff Asset 내 Hitmark 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref Stats, StatsAsString))
            {
                Log.Error("Buff Asset 내 Stats 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref LinkedBuffStatTypes, LinkedBuffStatTypesAsString))
            {
                Log.Error("Buff Asset 내 LinkedBuffStatTypes 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref BuffOnRelease, BuffOnReleaseAsString))
            {
                Log.Error("Buff Asset 내 BuffOnRelease 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref DeactiveBuffs, DeactiveBuffsAsString))
            {
                Log.Error("Buff Asset 내 DeactiveBuffs 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref DeactiveStateEffects, DeactiveStateEffectAsString))
            {
                Log.Error("Buff Asset 내 DeactiveStateEffects 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
            if (!EnumEx.ConvertTo(ref SFXName, SFXNameAsString))
            {
                Log.Error("Buff Asset 내 SFXName 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }

            if (!EnumEx.ConvertTo(ref TriggerType, TriggerAsString))
            {
                Log.Error("Buff Asset 내 TriggerAsString 변수 변환에 실패했습니다. {0}", Name.ToLogString());
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            TargetAsString = Target.ToString();
            ApplicationAsString = Application.ToString();
            TypeAsString = Type.ToString();
            StateEffectAsString = StateEffect.ToString();
            IncompatibleAsString = Incompatible.ToString();
            IncompatibleStateEffectAsString = IncompatibleStateEffect.ToString();

            MaxStackByStatAsString = MaxStackByStat.ToString();

            DurationByStatAsString = DurationByStat.ToString();
            HitmarkAsString = Hitmark.ToString();
            StatsAsString = Stats.ToStringArray();
            LinkedBuffStatTypesAsString = LinkedBuffStatTypes.ToStringArray();
            BuffOnReleaseAsString = BuffOnRelease.ToString();
            DeactiveBuffsAsString = DeactiveBuffs.ToStringArray();
            DeactiveStateEffectAsString = DeactiveStateEffects.ToStringArray();
            SFXNameAsString = SFXName.ToString();
            TriggerAsString = TriggerType.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
            CustomLog();
        }

        public BuffAssetData Clone()
        {
            BuffAssetData assetData = new()
            {
                Name = Name,
                Target = Target,
                Application = Application,
                Type = Type,
                StateEffect = StateEffect,
                Incompatible = Incompatible,
                IncompatibleStateEffect = IncompatibleStateEffect,

                UseSpawnCustomPrefab = UseSpawnCustomPrefab,
                InitBuffEntityPositionZero = InitBuffEntityPositionZero,
                InitBuffEntityPositionCaster = InitBuffEntityPositionCaster,
                IgnoreCheckOwnerAlive = IgnoreCheckOwnerAlive,
                IgnoreResetLevel = IgnoreResetLevel,

                MaxLevel = MaxLevel,

                MaxStack = MaxStack,
                MaxStackByLevel = MaxStackByLevel,

                MaxStackOptionMinRange = MaxStackOptionMinRange,
                MaxStackOptionMaxRange = MaxStackOptionMaxRange,
                MaxStackOptionStep = MaxStackOptionStep,

                MaxStackByStat = MaxStackByStat,
                IsAddMaxStackByStat = IsAddMaxStackByStat,
                ReleaseTimeByStack = ReleaseTimeByStack,
                StatApplicationByStack = StatApplicationByStack,
                SetStackByElapsedTimeOnApply = SetStackByElapsedTimeOnApply,
                RemoveBuffOnMaxStack = RemoveBuffOnMaxStack,
                BlockSpawnSoliloquyOnStackChanged = BlockSpawnSoliloquyOnStackChanged,

                ConditionalDuration = ConditionalDuration,
                IgnoreElapsedTimeResetOnOverlap = IgnoreElapsedTimeResetOnOverlap,
                DelayTime = DelayTime,
                Duration = Duration,
                DurationByLevel = DurationByLevel,
                DurationByStack = DurationByStack,
                DurationByStat = DurationByStat,

                MinDuration = MinDuration,
                MaxDuration = MaxDuration,
                GrowDuration = GrowDuration,

                Interval = Interval,
                RestTime = RestTime,
                RestTimeByLevel = RestTimeByLevel,                

                Hitmark = Hitmark,

                UseStatRedColor = UseStatRedColor,
                Stats = Stats,
                StatValues = StatValues,
                StatValuesByLevel = StatValuesByLevel,

                LinkedBuffStatTypes = LinkedBuffStatTypes,
                LinkedBuffStatDivisors = LinkedBuffStatDivisors,

                MinStatValues = MinStatValues,
                MaxStatValues = MaxStatValues,
                StatValuesIncreaseInRange = StatValuesIncreaseInRange,

                TriggerType = TriggerType,
                TriggerValue = TriggerValue,
                TriggerValueRatio = TriggerValueRatio,
                SaveTriggerValue = SaveTriggerValue,

                BuffOnRelease = BuffOnRelease,
                DeactiveBuffs = DeactiveBuffs,
                DeactiveStateEffects = DeactiveStateEffects,

                SFXName = SFXName,
                IsLoopSFX = IsLoopSFX,

                AnimationBool = AnimationBool,
                RemoveOnStopAnimation = RemoveOnStopAnimation,
                ShowBuffIcon = ShowBuffIcon,
                HideBuffIcon = HideBuffIcon,
            };

            if (Hitmark != HitmarkNames.None)
            {
                assetData.HitmarkAssetData = ScriptableDataManager.Instance.FindHitmarkClone(assetData.Hitmark);
            }

            return assetData;
        }

        /// <summary> 버프가 능력치를 가지고, 범위 값이 설정되었다면 전설 장비의 능력치로 간주합니다. </summary>
        public bool CheckIfLegendaryStat(StatNames statName)
        {
            if (!MinStatValues.IsValidArray())
            {
                return false;
            }

            // 버프의 능력치 적용에 해당 능력치를 가지고 있고, 범위 값이 설정되어있다면 전설 장비의 능력치로 간주합니다.
            for (int i = 0; i < Stats.Length; i++)
            {
                if (Stats[i] != statName) { continue; }
                if (MinStatValues.Length <= i) { continue; }
                if (MinStatValues[i].IsZero()) { continue; }

                return true;
            }
            return false;
        }

        //

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref TargetAsString, Target);
            UpdateIfChanged(ref ApplicationAsString, Application);
            UpdateIfChanged(ref TypeAsString, Type);
            UpdateIfChanged(ref StateEffectAsString, StateEffect);
            UpdateIfChanged(ref IncompatibleAsString, Incompatible);
            UpdateIfChanged(ref IncompatibleStateEffectAsString, IncompatibleStateEffect);
            UpdateIfChanged(ref MaxStackByStatAsString, MaxStackByStat);
            UpdateIfChanged(ref DurationByStatAsString, DurationByStat);
            UpdateIfChanged(ref HitmarkAsString, Hitmark);
            UpdateIfChangedArray(ref StatsAsString, Stats.ToStringArray());
            UpdateIfChangedArray(ref LinkedBuffStatTypesAsString, LinkedBuffStatTypes.ToStringArray());
            UpdateIfChanged(ref BuffOnReleaseAsString, BuffOnRelease);
            UpdateIfChangedArray(ref DeactiveBuffsAsString, DeactiveBuffs.ToStringArray());
            UpdateIfChangedArray(ref DeactiveStateEffectAsString, DeactiveStateEffects.ToStringArray());
            UpdateIfChanged(ref SFXNameAsString, SFXName);
            UpdateIfChanged(ref TriggerAsString, TriggerType);

            IsChangingAsset = false;

            return _hasChangedWhiteRefreshAll;
        }

        private bool _hasChangedWhiteRefreshAll = false;

        private void UpdateIfChanged<TEnum>(ref string target, TEnum newValue) where TEnum : Enum
        {
            string newString = newValue?.ToString();
            if (target != newString)
            {
                target = newString;
                _hasChangedWhiteRefreshAll = true;
            }
        }

        private void UpdateIfChangedArray(ref string[] target, string[] newArray)
        {
            if (!target.SequenceEqual(newArray))
            {
                target = newArray;
                _hasChangedWhiteRefreshAll = true;
            }
        }

#endif
    }
}