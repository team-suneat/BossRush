namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VSkillSlot
    {
        public int SlotID; // 스킬 슬롯 번호
        public bool IsUnlocked; // 스킬 슬롯 해금 여부
        public string SkillNameString; // 스킬 이름
    }
}