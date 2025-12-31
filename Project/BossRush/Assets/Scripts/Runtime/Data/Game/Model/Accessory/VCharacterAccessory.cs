using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterAccessory
    {
        public Dictionary<string, VAccessory> Accessories = new();
        public List<string> UnlockedAccessories = new();
        public string EquippedAccessoryNameString;

        [NonSerialized]
        private readonly Dictionary<ItemNames, VAccessory> _accessoryMap = new();

        [NonSerialized]
        private ItemNames _equippedAccessoryName = ItemNames.None;

        public ItemNames EquippedAccessoryName => _equippedAccessoryName;

        public void OnLoadGameData()
        {
            _accessoryMap.Clear();

            ItemNames itemName = ItemNames.None;
            foreach (KeyValuePair<string, VAccessory> kvp in Accessories)
            {
                VAccessory accessory = kvp.Value;
                accessory.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref itemName, kvp.Key))
                {
                    Log.Error(LogTags.GameData_Accessory, "악세사리 키를 ItemNames로 변환하지 못했습니다: {0}", kvp.Key);
                    continue;
                }

                accessory.Name = itemName;
                _accessoryMap[itemName] = accessory;
            }

            if (!string.IsNullOrEmpty(EquippedAccessoryNameString))
            {
                EnumEx.ConvertTo(ref _equippedAccessoryName, EquippedAccessoryNameString);
            }

            Log.Info(LogTags.GameData_Accessory, "[Character] 악세사리 데이터를 불러옵니다. 총 {0}개, 장착: {1}",
                Accessories.Count, _equippedAccessoryName.ToLogString());
        }
        
        public bool CheckUnlocked(ItemNames accessoryName)
        {
            return UnlockedAccessories.Contains(accessoryName.ToString());
        }

        public void Unlock(ItemNames accessoryName)
        {
            string key = accessoryName.ToString();
            if (!UnlockedAccessories.Contains(key))
            {
                UnlockedAccessories.Add(key);
                Log.Info(LogTags.GameData_Accessory, "악세사리를 해금합니다: {0}", accessoryName);
            }
        }

        public bool HasAccessory(ItemNames accessoryName)
        {
            return Accessories.ContainsKey(accessoryName.ToString());
        }

        public VAccessory FindAccessory(ItemNames accessoryName)
        {
            if (_accessoryMap.TryGetValue(accessoryName, out VAccessory accessory))
            {
                return accessory;
            }

            Log.Warning(LogTags.GameData_Accessory, "악세사리를 찾을 수 없습니다: {0}", accessoryName.ToLogString());
            return null;
        }

        public VAccessory FindEquippedAccessory()
        {
            if (_equippedAccessoryName == ItemNames.None)
            {
                return null;
            }

            return FindAccessory(_equippedAccessoryName);
        }

        public void AddAccessory(ItemNames accessoryName)
        {
            string key = accessoryName.ToString();
            if (!_accessoryMap.ContainsKey(accessoryName))
            {
                VAccessory newAccessory = new(accessoryName);
                Accessories[key] = newAccessory;
                _accessoryMap[accessoryName] = newAccessory;

                Log.Info(LogTags.GameData_Accessory, "인게임 악세사리를 등록합니다: {0}", accessoryName.ToLogString());
            }
        }

        public void AddAccessory(ItemNames accessoryName, GradeNames gradeName, StatNames statName)
        {
            AddAccessory(accessoryName);

            if (gradeName != GradeNames.None && statName != StatNames.None && _accessoryMap.TryGetValue(accessoryName, out VAccessory accessory))
            {
                accessory.AddGrade(gradeName, statName);
            }
            else
            {
                Log.Error("인게임 악세사리 추가에 필요한 올바른 등급 또는 능력치 이름이 아닙니다: {0}, {1}", gradeName.ToLogString(), statName.ToLogString());
            }
        }

        public void RemoveAccessory(ItemNames accessoryName)
        {
            string key = accessoryName.ToString();
            if (_accessoryMap.ContainsKey(accessoryName))
            {
                _ = Accessories.Remove(key);
                _ = _accessoryMap.Remove(accessoryName);

                if (_equippedAccessoryName == accessoryName)
                {
                    _equippedAccessoryName = ItemNames.None;
                    EquippedAccessoryNameString = string.Empty;
                }

                Log.Info(LogTags.GameData_Accessory, "인게임 악세사리를 등록해제합니다: {0}", accessoryName.ToLogString());
            }
        }

        public void EquipAccessory(ItemNames accessoryName)
        {
            if (accessoryName == ItemNames.None)
            {
                _equippedAccessoryName = ItemNames.None;
                EquippedAccessoryNameString = string.Empty;
                Log.Info(LogTags.GameData_Accessory, "악세사리 장착을 해제합니다.");
                return;
            }

            if (!_accessoryMap.ContainsKey(accessoryName))
            {
                Log.Warning(LogTags.GameData_Accessory, "장착할 악세사리가 없습니다: {0}", accessoryName.ToLogString());
                return;
            }

            _equippedAccessoryName = accessoryName;
            EquippedAccessoryNameString = accessoryName.ToString();
            Log.Info(LogTags.GameData_Accessory, "악세사리를 장착합니다: {0}", accessoryName.ToLogString());
        }

        public static VCharacterAccessory CreateDefault()
        {
            return new VCharacterAccessory();
        }
    }
}

