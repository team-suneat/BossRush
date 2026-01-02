namespace TeamSuneat.Setting
{
    public class GameInput
    {
        public int CharacterInput;
        public int UIInput;

        public bool IsBlockCharacterInput => CharacterInput > 0 || _isBlockInputFromPlayerAnimation;

        public bool IsBlockUIInput => UIInput > 0;

        // 캐릭터 생성 중 캐릭터 입력 차단
        private bool _isBlockInputFromPlayerAnimation;

        public void BlockInput()
        {
            BlockCharacterInput();
            BlockUIInput();
        }

        public void UnblockInput()
        {
            UnblockCharacterInput();
            UnblockUIInput();
        }

        public void ResetInput()
        {
            CharacterInput = 0;
            UIInput = 0;
            Log.Info(LogTags.Input, "[Game] 캐릭터와 UI 입력의 잠금을 모두 해제합니다.");
        }

        #region Character Input

        public void BlockCharacterInput()
        {
            CharacterInput += 1;
            Log.Info(LogTags.Input, "[Game] 캐릭터 입력을 잠금합니다. 잠금 횟수: {0}", CharacterInput.ToString());
        }

        public void UnblockCharacterInput()
        {
            CharacterInput -= 1;
            if (CharacterInput < 0)
            {
                CharacterInput = 0;
            }
            Log.Info(LogTags.Input, "[Game] 캐릭터 입력을 잠금해제합니다. 남은 잠금 횟수: {0}", CharacterInput.ToString());
        }

        #endregion Character Input

        #region Character Input Of Player Spawn

        public void BlockCharacterInputFromPlayerAnimation()
        {
            _isBlockInputFromPlayerAnimation = true;
            Log.Info(LogTags.Input, "[Game] 플레이어 캐릭터 애니메이션 중 캐릭터 입력을 잠금합니다. 잠금 횟수: {0}", _isBlockInputFromPlayerAnimation.ToBoolString());
        }

        public void UnblockCharacterInputFromPlayerAnimation()
        {
            _isBlockInputFromPlayerAnimation = false;
            Log.Info(LogTags.Input, "[Game] 플레이어 캐릭터 애니메이션 중 캐릭터 입력을 잠금해제합니다. 남은 잠금 횟수: {0}", _isBlockInputFromPlayerAnimation.ToBoolString());
        }

        #endregion Character Input Of Player Spawn

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