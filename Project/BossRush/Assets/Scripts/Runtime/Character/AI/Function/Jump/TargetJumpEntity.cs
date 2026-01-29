using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public enum TargetJumpName
    {
        None,

        Short,
        Medium,
        Long,
    }

    public class TargetJumpEntity : XBehaviour
    {
        [FoldoutGroup("#TargetJumpEntity")]
        [SuffixLabel("점프 타입")]
        [SerializeField]
        private TargetJumpName _name = TargetJumpName.None;

        [FoldoutGroup("#String")]
        public string NameString;

        [FoldoutGroup("#TargetJumpEntity")]
        [SuffixLabel("이 거리까지 담당")]
        [SerializeField]
        private float _maxDistance = 3f;

        [FoldoutGroup("#TargetJumpEntity")]
        [SuffixLabel("최대 높이 (시작/목표 중 높은 점 기준)")]
        [SerializeField]
        private float _maxHeight = 1.5f;

        [FoldoutGroup("#TargetJumpEntity")]
        [SuffixLabel("전체 체공 시간")]
        [SerializeField]
        private float _totalTime = 0.6f;

        public TargetJumpName Name => _name;
        public float MaxDistance => _maxDistance;
        public float MaxHeight => _maxHeight;
        public float TotalTime => _totalTime;

        public override void AutoSetting()
        {
            base.AutoSetting();

            NameString = _name.ToString();
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref _name, NameString);
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"TargetJumpEntity ({NameString})");
        }

        public Vector2 CalculateParabolicVelocity(Vector2 startPos, Vector2 targetPos, float gravity)
        {
            float dx = targetPos.x - startPos.x;
            float dy = targetPos.y - startPos.y;

            float baseY = Mathf.Max(startPos.y, targetPos.y);
            float peakY = baseY + Mathf.Max(0f, _maxHeight);
            float h0 = peakY - startPos.y;
            float h1 = peakY - targetPos.y;

            const float MinHeight = 0.5f;
            if (h0 < MinHeight)
            {
                float add = MinHeight - h0;
                peakY += add;
                h0 += add;
                h1 = peakY - targetPos.y;
            }

            float totalTime = Mathf.Max(0.1f, _totalTime);
            float vx = dx / totalTime;
            float vy = (dy + 0.5f * gravity * totalTime * totalTime) / totalTime;

            return new Vector2(vx, vy);
        }
    }
}