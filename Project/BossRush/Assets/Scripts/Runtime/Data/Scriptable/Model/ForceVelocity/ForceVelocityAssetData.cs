using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace TeamSuneat.Data
{
    [Serializable]
    public partial class ForceVelocityAssetData : ScriptableData<int>
    {
        public bool IsChangingAsset;

        [GUIColor("GetForceVelocityNameColor")]
        public FVNames Name;

        public int TID;
        public FVSubjects Subject;
        public bool IsDamage;
        public bool IsPiercing;
        public ApplicationTypes Application;
        public StatNames[] Stats;
        public int Priority;
        public FVDirections Direction;
        public Vector2 ForceVelocity;
        public Vector2 Acceleration;
        public float Gravity;
        public float Friction;
        public float AirResist;
        public float AirResistFlyingMonster;
        public float Delay;
        public float Duration;
        public bool DirectionalFacing;
        public bool ReverseDirectionalFacing;
        public bool UseGravity;
        public bool UseCustomGravity;
        public bool UseAccelerationX;
        public bool UseAccelerationY;
        public bool UseForceFriction;
        public bool UseAirResist;
        public bool StopXOnHitWall;
        public bool StopXOnHitGround;
        public bool StopYOnHitGround;
        public bool IgnorePlatform;
        public bool IgnoreCharacterInAir;

        public string SubjectString;
        public string ApplicationString;
        public string[] StatString;
        public string DirectionString;

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
        }

        public override void Refresh()
        {
            base.Refresh();

            SubjectString = Subject.ToString();
            ApplicationString = Application.ToString();
            StatString = Stats.ToStringArray();
            DirectionString = Direction.ToString();

            IsChangingAsset = false;
        }

        public override void OnLoadData()
        {
        }

        public ForceVelocityAssetData Clone()
        {
            ForceVelocityAssetData assetData = new()
            {
                Name = Name,
                TID = TID,
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
                DirectionalFacing = DirectionalFacing,
                ReverseDirectionalFacing = ReverseDirectionalFacing,
                UseGravity = UseGravity,
                UseCustomGravity = UseCustomGravity,
                UseAccelerationX = UseAccelerationX,
                UseAccelerationY = UseAccelerationY,
                UseForceFriction = UseForceFriction,
                UseAirResist = UseAirResist,
                StopXOnHitWall = StopXOnHitWall,
                StopXOnHitGround = StopXOnHitGround,
                StopYOnHitGround = StopYOnHitGround,
                IgnorePlatform = IgnorePlatform,
                IgnoreCharacterInAir = IgnoreCharacterInAir,
            };

            return assetData;
        }
    }
}