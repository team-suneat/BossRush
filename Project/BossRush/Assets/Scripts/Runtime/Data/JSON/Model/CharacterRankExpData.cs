namespace TeamSuneat.Data
{
    /// <summary>
    /// 캐릭터의 랭크별 경험치 데이터 (해당 랭크에서 다음 랭크까지 필요한 경험치)
    /// </summary>
    [System.Serializable]
    public class CharacterRankExpData : IData<int>
    {
        public int Rank;
        public int RequiredExperience;

        public int GetKey()
        {
            return Rank;
        }

        public void Refresh()
        {
        }

        public void OnLoadData()
        {
        }
    }
}

