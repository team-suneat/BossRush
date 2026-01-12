using Sirenix.OdinInspector;

namespace TeamSuneat.UserInterface
{
    [System.Serializable]
    public class FixedIndexOverrides
    {
        [LabelText("위쪽")]
        public int UpIndex = -1;

        [LabelText("아래쪽")]
        public int DownIndex = -1;

        [LabelText("왼쪽")]
        public int LeftIndex = -1;

        [LabelText("오른쪽")]
        public int RightIndex = -1;
    }
}
