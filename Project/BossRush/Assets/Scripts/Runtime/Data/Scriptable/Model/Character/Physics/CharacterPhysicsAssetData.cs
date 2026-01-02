using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat.Data
{
    [System.Serializable]
    public class CharacterPhysicsAssetData
    {
        #region Horizontal Movement

        [Title("Horizontal Movement")]
        [Tooltip("캐릭터가 걸을 때의 속도")]
        public float WalkSpeed = 6f;

        [Tooltip("이것이 사실이면 입력 소스에서 입력을 받고, 그렇지 않으면 SetHorizontalMove()를 통해 설정해야 합니다.")]
        public bool ReadInput = true;

        [Tooltip("이것이 사실이라면 가속이 움직임에 적용되지 않고 즉시 최고 속도가 됩니다(Megaman 움직임을 생각하십시오). " +
            "주의: 즉각적인 가속이 있는 캐릭터는 일반 캐릭터처럼 x축에서 넉백될 수 없으며, 이는 절충안입니다.")]
        public bool InstantAcceleration = false;

        [Tooltip("입력이 고려되는 임계값(작은 조이스틱 노이즈를 제거하기 위해 일반적으로 0.1f)")]
        public float InputThreshold = 0.1f;

        [Range(0f, 1f)]
        [Tooltip("플레이어가 얼마나 많은 공중 제어를 가지고 있는지")]
        public float AirControl = 1f;

        [Tooltip("플레이어가 공중에서 뒤집을 수 있는지 여부")]
        public bool AllowFlipInTheAir = true;

        [Tooltip("이 능력이 사망 후에도 수평 이동을 계속 처리해야 하는지 여부")]
        public bool ActiveAfterDeath = false;

        [Tooltip("지면에 닿을 때 피드백이 재생되기 전에 캐릭터가 공중에 떠 있어야 하는 시간(초)")]
        public float MinimumAirTimeBeforeFeedback = 0.2f;

        [Tooltip("벽과 측면 충돌 시 상태를 Idle로 재설정해야 하는지 여부")]
        public bool StopWalkingWhenCollidingWithAWall = false;

        #endregion Horizontal Movement

        #region Jump

        [Title("Horizontal Movement")]
        public List<CharacterJumpData> JumpDatas = new();

        #endregion Jump

        public CharacterJumpData CharacterJumpData
        {
            get
            {
                if (JumpDatas.Count > 0)
                {
                    return JumpDatas[0];
                }

                return null;
            }
        }

        public CharacterJumpData GetFirstJumpdata()
        {
            if (JumpDatas.Count > 0)
            {
                return JumpDatas[0];
            }

            return null;
        }

        public CharacterJumpData GetJumpData(int index)
        {
            if (JumpDatas.Count > index)
            {
                return JumpDatas[index];
            }

            return null;
        }

        public CharacterJumpData GetJumpData(JumpTypes jumpType)
        {
            if (JumpDatas.Count > 0)
            {
                for (int i = 0; i < JumpDatas.Count; i++)
                {
                    if (JumpDatas[i].Type == jumpType)
                    {
                        return JumpDatas[i];
                    }
                }
            }

            Log.Warning(LogTags.Character, "점프 데이터가 존재하지 않습니다. 점프타입: {0}", jumpType);

            return null;
        }

        public CharacterPhysicsAssetData Clone()
        {
            for (int i = 0; i < JumpDatas.Count; i++)
            {
                JumpDatas[i].Clone();
            }

            return new CharacterPhysicsAssetData()
            {
                WalkSpeed = WalkSpeed,
                ReadInput = ReadInput,
                InstantAcceleration = InstantAcceleration,
                InputThreshold = InputThreshold,
                AirControl = AirControl,
                AllowFlipInTheAir = AllowFlipInTheAir,
                ActiveAfterDeath = ActiveAfterDeath,
                MinimumAirTimeBeforeFeedback = MinimumAirTimeBeforeFeedback,
                StopWalkingWhenCollidingWithAWall = StopWalkingWhenCollidingWithAWall,
            };
        }

        public void Paste(CharacterPhysicsAssetData physicsAsset)
        {
            physicsAsset.WalkSpeed = WalkSpeed;
            physicsAsset.ReadInput = ReadInput;
            physicsAsset.InstantAcceleration = InstantAcceleration;
            physicsAsset.InputThreshold = InputThreshold;
            physicsAsset.AirControl = AirControl;
            physicsAsset.AllowFlipInTheAir = AllowFlipInTheAir;
            physicsAsset.ActiveAfterDeath = ActiveAfterDeath;
            physicsAsset.MinimumAirTimeBeforeFeedback = MinimumAirTimeBeforeFeedback;
            physicsAsset.StopWalkingWhenCollidingWithAWall = StopWalkingWhenCollidingWithAWall;

            for (int i = 0; i < physicsAsset.JumpDatas.Count; i++)
            {
                JumpDatas[i].Paste(physicsAsset.JumpDatas[i]);
            }
        }
    }
}