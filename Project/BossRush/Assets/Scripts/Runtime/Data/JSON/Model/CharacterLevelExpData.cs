namespace TeamSuneat.Data
{
    /// <summary>
    /// 플레이어의 레벨별 경험치 데이터 (해당 레벨에서 다음 레벨까지 필요한 경험치)
    /// </summary>
    [System.Serializable]
    public class CharacterLevelExpData : IData<int>
    {
        public int Level;
        public int RequiredExperience;

        public int GetKey()
        {
            return Level;
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }
    }
}

