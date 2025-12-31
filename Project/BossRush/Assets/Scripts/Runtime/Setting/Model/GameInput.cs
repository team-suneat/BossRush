using TeamSuneat;

namespace TeamSuneat.Setting
{
    public class GameInput
    {
        public int UIInput;

        public bool IsBlockUIInput => UIInput > 0;

        public void BlockInput()
        {
            BlockUIInput();
        }

        public void UnblockInput()
        {
            UnblockUIInput();
        }

        public void ResetInput()
        {
            UIInput = 0;
            Log.Info(LogTags.Input, "[Game] UI 입력의 잠금을 모두 해제합니다.");
        }

        #region UI Input

        public void BlockUIInput()
        {
            UIInput += 1;
            Log.Info(LogTags.Input, "[Game] UI 입력을 잠금합니다. 잠금 횟수: {0}", UIInput.ToString());
        }

        public void UnblockUIInput()
        {
            UIInput--;
            if (UIInput < 0)
            {
                UIInput = 0;
            }
                        Log.Info(LogTags.Input, "[Game] UI 입력을 잠금해제합니다. 남은 잠금 횟수: {0}", UIInput.ToString());
        }

        #endregion UI Input
    }
}