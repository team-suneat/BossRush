using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VSlot
    {
        public int SlotID;
        public bool IsUnlocked;
    }

    [System.Serializable]
    public class VCharacterSlot
    {
        public List<VSlot> Slots = new();

        /// <summary>
        /// 게임 데이터 로드 시 호출됩니다. 현재는 초기화할 데이터가 없습니다.
        /// </summary>
        public void OnLoadGameData()
        {
            // VCharacterSlot은 세이브 데이터에서 직접 로드되므로 추가 초기화가 필요 없습니다.
        }

        public void Lock(int index)
        {
            if (Slots.IsValid(index))
            {
                Slots[index].IsUnlocked = false;
            }
        }

        public void Unlock(int index)
        {
            if (Slots.IsValid(index))
            {
                Slots[index].IsUnlocked = true;
            }
        }

        public static VCharacterSlot CreateDefault()
        {
            return new VCharacterSlot()
            {
                Slots = new List<VSlot>()
                {
                    new VSlot(){ SlotID = 0, IsUnlocked = true},
                    new VSlot(){ SlotID = 1, IsUnlocked = true},
                    new VSlot(){ SlotID = 2, IsUnlocked = true},
                    new VSlot(){ SlotID = 3, IsUnlocked = true},
                    new VSlot(){ SlotID = 4, IsUnlocked = false},
                    new VSlot(){ SlotID = 5, IsUnlocked = false},
                }
            };
        }
    }
}