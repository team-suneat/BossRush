using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class XPatternStep
    {
        public PatternStepNames StepName;

        [ShowIf("StepName", PatternStepNames.FaceDirectional)]
        public FacingDirections FacingDirection;

        public float FaceAgainstWallDistance;

        public bool UseRandomOrder;

        public int OrderIndex;

        [ShowIf("UseRandomOrder")] public int OrderMaxIndex;

        public bool UseRepeat;

        public bool UseRandomRepeat;

        [ShowIf("UseRandomRepeat")] public int RepeatMinCount;

        [ShowIf("UseRandomRepeat")] public int RepeatMaxCount;

        [HideInInspector] public int CurrentRepeatCount;

        [HideInInspector] public int CurrentRepeatMaxCount;

        public bool IsCompleteStep => CurrentRepeatCount >= CurrentRepeatMaxCount;

        public void RefreshRepeatMaxCount()
        {
            if (UseRepeat && UseRandomRepeat)
            {
                CurrentRepeatMaxCount = RandomEx.Range(RepeatMinCount - 1, RepeatMaxCount);
            }
        }
    }
}