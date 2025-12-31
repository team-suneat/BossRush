using System;
using System.Collections.Generic;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterWeapon
    {
        public Dictionary<string, VWeapon> Weapons = new();
        public List<string> UnlockedWeapons = new();
        public List<string> SlotWeaponNameStrings = new();
        public int UnlockedSlotCount;

        [NonSerialized]
        private readonly Dictionary<ItemNames, VWeapon> _weaponMap = new();

        [NonSerialized]
        private readonly List<ItemNames> _slotWeaponNames = new();

        public IReadOnlyList<ItemNames> SlotWeaponNames => _slotWeaponNames;

        public List<ItemNames> GetWeaponNames()
        {
            return new List<ItemNames>(_slotWeaponNames);
        }

        public List<VWeapon> GetWeapons()
        {
            List<VWeapon> weapons = new();
            foreach (ItemNames weaponName in _slotWeaponNames)
            {
                if (_weaponMap.TryGetValue(weaponName, out VWeapon weapon))
                {
                    weapons.Add(weapon);
                }
            }

            return weapons;
        }

        //

        public void OnLoadGameData()
        {
            _weaponMap.Clear();

            ItemNames itemName = ItemNames.None;
            foreach (KeyValuePair<string, VWeapon> kvp in Weapons)
            {
                VWeapon weapon = kvp.Value;
                weapon.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref itemName, kvp.Key))
                {
                    Log.Error(LogTags.GameData_Weapon, "무기 키를 ItemNames로 변환하지 못했습니다: {0}", kvp.Key);
                    continue;
                }

                weapon.Name = itemName;
                _weaponMap[itemName] = weapon;
            }

            _slotWeaponNames.Clear();
            foreach (string slotName in SlotWeaponNameStrings)
            {
                if (!EnumEx.ConvertTo(ref itemName, slotName))
                {
                    Log.Error(LogTags.GameData_Weapon, "무기 슬롯 이름을 ItemNames로 변환하지 못했습니다: {0}", slotName);
                    continue;
                }

                if (_slotWeaponNames.Count >= UnlockedSlotCount)
                {
                    break;
                }

                if (_weaponMap.ContainsKey(itemName))
                {
                    _slotWeaponNames.Add(itemName);
                }
            }

            SyncSlotWeaponNameStrings();
        }

        //

        public bool CheckUnlocked(ItemNames weaponName)
        {
            return UnlockedWeapons.Contains(weaponName.ToString());
        }

        public void Unlock(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (!UnlockedWeapons.Contains(key))
            {
                UnlockedWeapons.Add(key);
                Log.Info(LogTags.GameData_Weapon, "무기를 해금합니다: {0}", weaponName);
            }
        }

        //

        public bool HasWeapon(ItemNames weaponName)
        {
            return Weapons.ContainsKey(weaponName.ToString());
        }

        public VWeapon FindWeapon(ItemNames weaponName)
        {
            if (_weaponMap.TryGetValue(weaponName, out VWeapon weapon))
            {
                return weapon;
            }

            Log.Warning(LogTags.GameData_Weapon, "무기를 찾을 수 없습니다: {0}", weaponName.ToLogString());
            return null;
        }

        public void AddWeapon(ItemNames weaponName)
        {
            if (_slotWeaponNames.Count >= UnlockedSlotCount)
            {
                Log.Warning(LogTags.GameData_Weapon, "무기 슬롯이 가득 찼습니다. 현재/최대: {0}/{1}", _slotWeaponNames.Count, UnlockedSlotCount);
                return;
            }

            string key = weaponName.ToString();
            if (!_weaponMap.ContainsKey(weaponName))
            {
                VWeapon newWeapon = new(weaponName);
                Weapons[key] = newWeapon;
                _weaponMap[weaponName] = newWeapon;

                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록합니다: {0}", weaponName.ToLogString());
            }

            if (!_slotWeaponNames.Contains(weaponName))
            {
                _slotWeaponNames.Add(weaponName);
                SlotWeaponNameStrings.Add(key);
            }
        }

        public void AddWeapon(ItemNames weaponName, GradeNames gradeName, StatNames statName)
        {
            AddWeapon(weaponName);

            if (gradeName != GradeNames.None && statName != StatNames.None && _weaponMap.TryGetValue(weaponName, out VWeapon weapon))
            {
                weapon.AddGrade(gradeName, statName);
            }
            else
            {
                Log.Error("인게임 무기 추가에 필요한 올바른 등급 또는 능력치 이름이 아닙니다: {0}, {1}", gradeName.ToLogString(), statName.ToLogString());
            }
        }

        public void RemoveWeapon(ItemNames weaponName)
        {
            string key = weaponName.ToString();
            if (_weaponMap.ContainsKey(weaponName))
            {
                _ = Weapons.Remove(key);
                _ = _weaponMap.Remove(weaponName);
                _ = _slotWeaponNames.Remove(weaponName);
                _ = SlotWeaponNameStrings.Remove(key);

                Log.Info(LogTags.GameData_Weapon, "인게임 무기를 등록해제합니다: {0}", weaponName.ToLogString());
            }
        }

        //

        public static VCharacterWeapon CreateDefault()
        {
            VCharacterWeapon defaultWeapons = new();

            return defaultWeapons;
        }

        private void SyncSlotWeaponNameStrings()
        {
            SlotWeaponNameStrings.Clear();
            for (int i = 0; i < _slotWeaponNames.Count; i++)
            {
                SlotWeaponNameStrings.Add(_slotWeaponNames[i].ToString());
            }
        }
    }
}