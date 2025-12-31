using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Feedbacks
{
    /// the possible modes for the timescale
    public enum TimescaleModes
    { Scaled, Unscaled }

    /// <suTSary>
    /// 각 GameFeedback의 동작을 정의하는 데 사용되는 지연, 재사용 대기시간 및 반복 값을 수집하는 클래스
    /// </suTSary>
    [System.Serializable]
    public class GameFeedbackTiming
    {
        /// <suTSary>
        /// 호스트 GameFeedbacks의 지시에 따라 이 피드백이 재생될 수 있는 가능한 방법
        /// </suTSary>
        public enum GameFeedbacksDirectionConditions
        { Always, OnlyWhenForwards, OnlyWhenBackwards };

        /// <suTSary>
        /// 이 피드백을 재생할 수 있는 가능한 방법
        /// </suTSary>
        public enum PlayDirections
        { FollowGameFeedbacksDirection, OppositeGameFeedbacksDirection, AlwaysNormal, AlwaysRewind }

        [Title("Timescale")]
        [Tooltip("스케일된 시간 또는 스케일되지 않은 시간에 작업하는지 여부")]
        public TimescaleModes TimescaleMode = TimescaleModes.Scaled;

        [Title("Exceptions")]
        [Tooltip("이것이 사실이면 일시 중지를 유지하면 이 피드백이 완료될 때까지 기다리지 않습니다.")]
        public bool ExcludeFromHoldingPauses = false;

        [Tooltip("상위 GameFeedbacks(Player) 총 지속 시간에서 이 피드백을 계산할지 여부")]
        public bool ContributeToTotalDuration = true;

        [Title("Delays")]
        [Tooltip("지연을 재생하기 전에 적용할 초기 지연(초)")]
        public float InitialDelay = 0f;

        [Tooltip("두 번의 플레이 사이에 필수 재사용 대기 시간")]
        public float CooldownDuration = 0f;

        [Title("Stop")]
        [Tooltip("이것이 사실이면 이 피드백은 상위 GameFeedbacks에서 Stop이 호출될 때 자체적으로 중단되고, 그렇지 않으면 계속 실행됩니다.")]
        public bool InterruptsOnStop = true;

        [Title("Repeat")]
        [Tooltip("반복 모드, 피드백을 한 번, 여러 번 또는 영원히 재생해야 하는지 여부")]
        public int NumberOfRepeats = 0;

        [Tooltip("이것이 사실이라면 피드백은 영원히 반복될 것입니다")]
        public bool RepeatForever = false;

        [Tooltip("이 피드백의 두 실행 사이의 지연(초)입니다. 여기에는 피드백 기간이 포함되지 않습니다.")]
        public float DelayBetweenRepeats = 1f;

        [Title("Play Direction")]
        [Tooltip("이는 호스트 GameFeedbacks가 재생될 때 이 피드백이 재생되는 방식을 정의합니다." +
                  "- Always (기본값) : 이 피드백은 항상 재생됩니다." +
                  "- OnlyWhenForwards : 이 피드백은 호스트 GameFeedbacks가 위에서 아래 방향(앞으로)으로 재생되는 경우에만 재생됩니다." +
                  "- OnlyWhenBackwards : 이 피드백은 호스트 GameFeedbacks가 아래에서 위로(뒤로) 재생되는 경우에만 재생됩니다.")]
        public GameFeedbacksDirectionConditions GameFeedbacksDirectionCondition = GameFeedbacksDirectionConditions.Always;

        [Tooltip("이것은 이 피드백이 재생되는 방식을 정의합니다. 정상적인 방향으로 재생하거나 되감기(사운드가 뒤로 재생됨)로 재생할 수 있습니다." +
                  " 일반적으로 확대되는 개체는 축소되고 곡선은 오른쪽에서 왼쪽으로 평가되는 등)" +
                  "- BasedOnGameFeedbacksDirection : 호스트 GameFeedbacks가 앞으로 재생될 때 정상적으로 재생되고, 뒤로 재생될 때 되감기에서 재생됩니다." +
                  "- OppositeGameFeedbacksDirection : 호스트 GameFeedbacks가 앞으로 재생될 때 되감기에서 재생되고 일반적으로 뒤로 재생될 때 재생됩니다." +
                  "- Always Normal : 호스트 GameFeedbacks의 방향에 관계없이 항상 정상적으로 재생됩니다." +
                  "- Always Rewind : 호스트 GameFeedbacks의 방향에 관계없이 항상 되감기로 재생됩니다.")]
        public PlayDirections PlayDirection = PlayDirections.FollowGameFeedbacksDirection;

        [Title("Intensity")]
        [Tooltip("이것이 사실이라면 부모 GameFeedbacks가 더 낮은 강도로 재생되더라도 강도는 일정할 것입니다.")]
        public bool ConstantIntensity = false;

        [Tooltip("이것이 사실이면 이 피드백은 강도가 IntensityIntervalMin보다 높거나 같고 IntensityIntervalMax보다 낮은 경우에만 재생됩니다.")]
        public bool UseIntensityInterval = false;

        [Tooltip("이 피드백이 재생되는 데 필요한 최소 강도")]
        [EnableIf("UseIntensityInterval", true)]
        public float IntensityIntervalMin = 0f;

        [Tooltip("이 피드백이 재생되는 데 필요한 최대 강도")]
        [EnableIf("UseIntensityInterval", true)]
        public float IntensityIntervalMax = 0f;

        [Title("Sequence")]
        [Tooltip("이 피드백을 재생하는 데 사용할 TSSequence")]
        public GameFeedbackSequence Sequence;

        [Tooltip("고려할 TSSequence의 TrackID")]
        public int TrackID = 0;

        [Tooltip("대상 시퀀스의 양자화(Quantized)된 버전을 사용할지 여부")]
        public bool Quantized = false;

        [Tooltip("대상 시퀀스의 양자화된 버전을 사용하는 경우 재생 시 시퀀스에 적용할 BPM")]
        [EnableIf("Quantized", true)]
        public int TargetBPM = 120;
    }
}