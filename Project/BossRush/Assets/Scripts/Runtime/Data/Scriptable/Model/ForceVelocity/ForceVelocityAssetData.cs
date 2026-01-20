using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class ForceVelocityAssetData : ScriptableData<int>
    {
        [SuffixLabel("개별 에셋 변경 모드")]
        public bool IsChangingAsset;

        [GUIColor("GetForceVelocityNameColor")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("ForceVelocity 이름")]
        public FVNames Name;

        [FoldoutGroup("#기본 설정")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("대상")]
        public FVSubjects Subject;

        [FoldoutGroup("#기본 설정")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("피해 여부")]
        public bool IsDamage;

        [FoldoutGroup("#기본 설정")]
        [GUIColor("GetBoolColor")]
        [SuffixLabel("관통 여부")]
        public bool IsPiercing;

        [FoldoutGroup("#기본 설정")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("적용 방식")]
        public ApplicationTypes Application;

        [FoldoutGroup("#기본 설정")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("스탯")]
        public StatNames[] Stats;

        [FoldoutGroup("#기본 설정")]
        [GUIColor("GetIntColor")]
        [SuffixLabel("우선순위")]
        public int Priority;

        [FoldoutGroup("#기본 설정")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("방향")]
        public FVDirections Direction;

        [FoldoutGroup("#속도 및 힘")]
        [SuffixLabel("속도 벡터")]
        public Vector2 ForceVelocity;

        [FoldoutGroup("#속도 및 힘")]
        [SuffixLabel("가속도 벡터")]
        public Vector2 Acceleration;

        [FoldoutGroup("#속도 및 힘")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("중력")]
        public float Gravity;

        [FoldoutGroup("#속도 및 힘")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("마찰")]
        public float Friction;

        [FoldoutGroup("#속도 및 힘")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("공기 저항")]
        public float AirResist;

        [FoldoutGroup("#속도 및 힘")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("비행 몬스터 공기 저항")]
        public float AirResistFlyingMonster;

        [FoldoutGroup("#시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("지연 시간(초)")]
        public float Delay;

        [FoldoutGroup("#시간")]
        [GUIColor("GetFloatColor")]
        [SuffixLabel("지속 시간(초)")]
        public float Duration;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("방향성 타입")]
        public FVDirectionalType DirectionalType = FVDirectionalType.None;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("중력 타입")]
        public FVGravityType GravityType = FVGravityType.None;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("가속도 타입")]
        public FVAccelerationType AccelerationType = FVAccelerationType.None;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("마찰 및 저항 타입")]
        public FVFrictionType FrictionType = FVFrictionType.None;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("충돌 시 정지 타입")]
        public FVStopOnCollisionType StopOnCollisionType = FVStopOnCollisionType.None;

        [FoldoutGroup("#옵션")]
        [EnableIf("IsChangingAsset")]
        [SuffixLabel("무시 타입")]
        public FVIgnoreType IgnoreType = FVIgnoreType.None;

        [FoldoutGroup("#String")] public string SubjectString;
        [FoldoutGroup("#String")] public string ApplicationString;
        [FoldoutGroup("#String")] public string[] StatString;
        [FoldoutGroup("#String")] public string DirectionString;
        [FoldoutGroup("#String")] public string DirectionalTypeString;
        [FoldoutGroup("#String")] public string GravityTypeString;
        [FoldoutGroup("#String")] public string AccelerationTypeString;
        [FoldoutGroup("#String")] public string FrictionTypeString;
        [FoldoutGroup("#String")] public string StopOnCollisionTypeString;
        [FoldoutGroup("#String")] public string IgnoreTypeString;

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

            if (!EnumEx.ConvertTo(ref Subject, SubjectString)) { Log.Error("Asset 내 Subject 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref Application, ApplicationString)) { Log.Error("Asset 내 Application 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref Stats, StatString)) { Log.Error("Asset 내 Stats 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref Direction, DirectionString)) { Log.Error("Asset 내 Direction 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref DirectionalType, DirectionalTypeString)) { Log.Error("Asset 내 DirectionalType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref GravityType, GravityTypeString)) { Log.Error("Asset 내 GravityType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref AccelerationType, AccelerationTypeString)) { Log.Error("Asset 내 AccelerationType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref FrictionType, FrictionTypeString)) { Log.Error("Asset 내 FrictionType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref StopOnCollisionType, StopOnCollisionTypeString)) { Log.Error("Asset 내 StopOnCollisionType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
            if (!EnumEx.ConvertTo(ref IgnoreType, IgnoreTypeString)) { Log.Error("Asset 내 IgnoreType 변수 변환에 실패했습니다. {0}", Name.ToLogString()); }
        }

        public override void Refresh()
        {
            base.Refresh();

            SubjectString = Subject.ToString();
            ApplicationString = Application.ToString();
            StatString = Stats.ToStringArray();
            DirectionString = Direction.ToString();
            DirectionalTypeString = DirectionalType.ToString();
            GravityTypeString = GravityType.ToString();
            AccelerationTypeString = AccelerationType.ToString();
            FrictionTypeString = FrictionType.ToString();
            StopOnCollisionTypeString = StopOnCollisionType.ToString();
            IgnoreTypeString = IgnoreType.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
        }

#if UNITY_EDITOR

        public bool RefreshWithoutSave()
        {
            _hasChangedWhiteRefreshAll = false;

            UpdateIfChanged(ref SubjectString, Subject);
            UpdateIfChanged(ref ApplicationString, Application);
            UpdateIfChangedArray(ref StatString, Stats.ToStringArray());
            UpdateIfChanged(ref DirectionString, Direction);
            UpdateIfChanged(ref DirectionalTypeString, DirectionalType);
            UpdateIfChanged(ref GravityTypeString, GravityType);
            UpdateIfChanged(ref AccelerationTypeString, AccelerationType);
            UpdateIfChanged(ref FrictionTypeString, FrictionType);
            UpdateIfChanged(ref StopOnCollisionTypeString, StopOnCollisionType);
            UpdateIfChanged(ref IgnoreTypeString, IgnoreType);

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
            if (target == null || newArray == null)
            {
                if (target != newArray)
                {
                    target = newArray;
                    _hasChangedWhiteRefreshAll = true;
                }
                return;
            }

            if (target.Length != newArray.Length)
            {
                target = newArray;
                _hasChangedWhiteRefreshAll = true;
                return;
            }

            for (int i = 0; i < target.Length; i++)
            {
                if (target[i] != newArray[i])
                {
                    target = newArray;
                    _hasChangedWhiteRefreshAll = true;
                    return;
                }
            }
        }

#endif

        public ForceVelocityAssetData Clone()
        {
            ForceVelocityAssetData assetData = new()
            {
                Name = Name,
                Subject = Subject,
                IsDamage = IsDamage,
                IsPiercing = IsPiercing,
                Application = Application,
                Stats = Stats,
                Priority = Priority,
                Direction = Direction,
                ForceVelocity = ForceVelocity,
                Acceleration = Acceleration,
                Gravity = Gravity,
                Friction = Friction,
                AirResist = AirResist,
                AirResistFlyingMonster = AirResistFlyingMonster,
                Delay = Delay,
                Duration = Duration,
                DirectionalType = DirectionalType,
                GravityType = GravityType,
                AccelerationType = AccelerationType,
                FrictionType = FrictionType,
                StopOnCollisionType = StopOnCollisionType,
                IgnoreType = IgnoreType,
            };

            return assetData;
        }
    }
}