using Sirenix.OdinInspector;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 캐릭터 기본 정보 탭 페이지
    public class UICharacterBasicInfoPage : UIPage
    {
        [Title("#UICharacterBasicInfoPage")]
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private UIEquipmentSlot _weaponSlot;
        [SerializeField] private UIEquipmentSlot _accessorySlot;
        [SerializeField] private UIStatDisplayGroup _statDisplayGroup;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _levelText ??= this.FindComponent<UILocalizedText>("Level Text");
            _weaponSlot ??= this.FindComponent<UIEquipmentSlot>("Weapon Slot");
            _accessorySlot ??= this.FindComponent<UIEquipmentSlot>("Accessory Slot");
            _statDisplayGroup ??= GetComponentInChildren<UIStatDisplayGroup>(true);
        }

        protected override void OnShow()
        {
            base.OnShow();
            Refresh();
        }

        public void Refresh()
        {
            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                ClearAll();
                return;
            }

            RefreshLevel(profile);
            RefreshWeaponSlot(profile);
            RefreshAccessorySlot(profile);
            RefreshStats();
        }

        private void RefreshLevel(VProfile profile)
        {
            if (_levelText == null)
            {
                return;
            }

            int level = profile.Level?.Level ?? 1;
            _levelText.SetText(level.GetLevelString());
        }

        private void RefreshWeaponSlot(VProfile profile)
        {
            if (_weaponSlot == null)
            {
                return;
            }

            VCharacterWeapon weaponData = profile.Weapon;
            if (weaponData == null || weaponData.SlotWeaponNames.Count == 0)
            {
                _weaponSlot.SetEmpty();
                return;
            }

            // 첫 번째 장착 무기 표시
            ItemNames firstWeaponName = weaponData.SlotWeaponNames[0];
            VWeapon weapon = weaponData.FindWeapon(firstWeaponName);
            _weaponSlot.SetWeaponData(weapon);
        }

        private void RefreshAccessorySlot(VProfile profile)
        {
            if (_accessorySlot == null)
            {
                return;
            }

            VCharacterAccessory accessoryData = profile.Accessory;
            if (accessoryData == null)
            {
                _accessorySlot.SetEmpty();
                return;
            }

            VAccessory equippedAccessory = accessoryData.FindEquippedAccessory();
            _accessorySlot.SetAccessoryData(equippedAccessory);
        }

        private void RefreshStats()
        {
            if (_statDisplayGroup == null)
            {
                return;
            }

            _statDisplayGroup.RefreshStatsFromProfile();
        }

        private void ClearAll()
        {
            if (_levelText != null)
            {
                _levelText.SetText(string.Empty);
            }

            _weaponSlot?.SetEmpty();
            _accessorySlot?.SetEmpty();
            _statDisplayGroup?.ClearAll();
        }
    }
}

