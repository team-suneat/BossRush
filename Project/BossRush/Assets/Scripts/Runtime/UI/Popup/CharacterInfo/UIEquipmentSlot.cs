using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 장비 슬롯 (무기/악세사리 공용) 컴포넌트
    public class UIEquipmentSlot : XBehaviour
    {
        [Title("#UIEquipmentSlot")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private UILocalizedText _nameText;
        [SerializeField] private UILocalizedText _levelText;
        [SerializeField] private GameObject _emptyStateObject;

        private ItemNames _currentItemName;
        private bool _isEmpty = true;

        public ItemNames CurrentItemName => _currentItemName;
        public bool IsEmpty => _isEmpty;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _iconImage ??= this.FindComponent<Image>("Icon Image");
            _nameText ??= this.FindComponent<UILocalizedText>("Name Text");
            _levelText ??= this.FindComponent<UILocalizedText>("Level Text");
            _emptyStateObject ??= this.FindGameObject("Empty State");
        }

        public void SetWeaponData(VWeapon weapon)
        {
            if (weapon == null)
            {
                SetEmpty();
                return;
            }

            _currentItemName = weapon.Name;
            _isEmpty = false;

            SetName(weapon.Name);
            SetLevel(weapon.Level);
            UpdateEmptyState(false);
            UpdateIconVisibility(true);
        }

        public void SetAccessoryData(VAccessory accessory)
        {
            if (accessory == null)
            {
                SetEmpty();
                return;
            }

            _currentItemName = accessory.Name;
            _isEmpty = false;

            SetName(accessory.Name);
            SetLevel(accessory.Level);
            UpdateEmptyState(false);
            UpdateIconVisibility(true);
        }

        public void SetEmpty()
        {
            _currentItemName = ItemNames.None;
            _isEmpty = true;

            UpdateIconVisibility(false);

            if (_nameText != null)
            {
                _nameText.SetText(string.Empty);
            }

            if (_levelText != null)
            {
                _levelText.SetText(string.Empty);
            }

            UpdateEmptyState(true);
        }

        public void SetIcon(Sprite iconSprite)
        {
            if (_iconImage == null)
            {
                return;
            }

            if (iconSprite != null)
            {
                _iconImage.sprite = iconSprite;
                _iconImage.enabled = true;
            }
            else
            {
                _iconImage.enabled = false;
            }
        }

        private void SetName(ItemNames itemName)
        {
            if (_nameText == null)
            {
                return;
            }

            string localizedName = itemName.GetLocalizedString();
            _nameText.SetText(localizedName);
        }

        private void SetLevel(int level)
        {
            if (_levelText == null)
            {
                return;
            }

            _levelText.SetText(level.GetLevelString());
        }

        private void UpdateEmptyState(bool isEmpty)
        {
            if (_emptyStateObject != null)
            {
                _emptyStateObject.SetActive(isEmpty);
            }
        }

        private void UpdateIconVisibility(bool isVisible)
        {
            if (_iconImage != null)
            {
                _iconImage.enabled = isVisible;
            }
        }
    }
}

