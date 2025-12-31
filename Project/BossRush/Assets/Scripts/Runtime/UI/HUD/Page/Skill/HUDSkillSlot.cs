using Sirenix.OdinInspector;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    // 인게임 스킬 슬롯 UI - 스킬 아이콘, 쿨타임, 사용 가능 여부 표시
    public class HUDSkillSlot : XBehaviour
    {
        public enum SkillSlotState
        {
            None,           // 슬롯이 비어있거나 유효하지 않음
            Locked,         // 슬롯이 잠금 상태
            Unlockable,     // 재화 지불로 강제 해금 가능한 상태
            Available,      // 스킬이 있고 사용 가능함
            Cooldown        // 쿨타임 중
        }
        [Title("#HUDSkillSlot")]
        [SerializeField] private int _slotIndex;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private UIGauge _skillGauge;
        [SerializeField] private Button _skillButton;
        [SerializeField] private GameObject _passiveIndicator;
        [SerializeField] private GameObject _unlockableIndicator;

        [Title("#Gauge Colors")]
        [SerializeField] private Color _timeBasedColor;
        [SerializeField] private Color _attackCountBasedColor;

        private VSkill _currentSkill;
        private SkillNames _currentSkillName = SkillNames.None;
        private SkillAsset _skillAsset;
        private SkillSlotState _currentState = SkillSlotState.None;

        public int SlotIndex => _slotIndex;
        public SkillNames CurrentSkillName => _currentSkillName;
        public SkillSlotState CurrentState => _currentState;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _skillIconImage ??= this.FindComponent<Image>("Skill Icon Image");
            _skillGauge ??= this.FindComponent<UIGauge>("Skill Gauge");
            _skillButton ??= this.FindComponent<Button>("Skill Button");
            _passiveIndicator ??= this.FindGameObject("Passive Indicator");
            _unlockableIndicator ??= this.FindGameObject("Unlockable");
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"HUDSkillSlot({_slotIndex})");
        }

        private void Awake()
        {
            if (_skillButton != null)
            {
                _skillButton.onClick.AddListener(OnSkillButtonClicked);
            }
        }

        public void Setup(int slotIndex)
        {
            _slotIndex = slotIndex;
            Refresh();
        }

        public void Refresh()
        {
            if (!TryValidateRefresh(out VCharacterSkill characterSkill, out VSkillSlot slot, out SkillNames skillName))
            {
                return;
            }

            LoadSkillData(characterSkill, skillName);
            RefreshSkillIcon();
            RefreshPassiveIndicator();
            _currentState = SkillSlotState.Available;
        }

        private bool TryValidateRefresh(out VCharacterSkill characterSkill, out VSkillSlot slot, out SkillNames skillName)
        {
            characterSkill = null;
            slot = null;
            skillName = SkillNames.None;

            VProfile profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                ClearSlot();
                return false;
            }

            characterSkill = profile.Skill;
            if (characterSkill == null || !characterSkill.Slots.IsValid(_slotIndex))
            {
                ClearSlot();
                return false;
            }

            slot = characterSkill.Slots[_slotIndex];
            if (!slot.IsUnlocked)
            {
                int unlockedSlotCount = GetUnlockedSlotCount(characterSkill);
                if (unlockedSlotCount == _slotIndex)
                {
                    SetUnlockableState();
                }
                else
                {
                    SetLockedState();
                }
                return false;
            }

            if (string.IsNullOrEmpty(slot.SkillNameString))
            {
                ClearSlot();
                return false;
            }

            if (!TryParseSkillName(slot.SkillNameString, out skillName))
            {
                ClearSlot();
                return false;
            }

            return true;
        }

        private bool TryParseSkillName(string skillNameString, out SkillNames skillName)
        {
            skillName = SkillNames.None;

            if (!EnumEx.ConvertTo(ref skillName, skillNameString))
            {
                Log.Warning(LogTags.UI_Page, "스킬 슬롯 이름을 SkillNames로 변환하지 못했습니다: {0}", skillNameString);
                return false;
            }

            return true;
        }

        private void LoadSkillData(VCharacterSkill characterSkill, SkillNames skillName)
        {
            _currentSkillName = skillName;
            _currentSkill = characterSkill.FindSkill(skillName);
            _skillAsset = ScriptableDataManager.Instance?.FindSkill(skillName);
        }

        private void RefreshSkillIcon()
        {
            if (_skillIconImage == null)
            {
                return;
            }

            Sprite icon = _skillAsset?.Data?.Icon;
            if (icon != null)
            {
                _skillIconImage.sprite = icon;
                _skillIconImage.enabled = true;
                _skillIconImage.SetGreyScale(false);
            }
            else
            {
                _skillIconImage.enabled = false;
            }
        }

        private void RefreshPassiveIndicator()
        {
            if (_passiveIndicator == null)
            {
                return;
            }

            bool isPassive = _skillAsset?.Data?.Type == SkillTypes.Passive;
            _passiveIndicator.SetActive(isPassive);
        }

        public void UpdateCooldown(float currentCooldown, float maxCooldown, bool isAvailable)
        {
            if (!IsSkillValid())
            {
                _currentState = SkillSlotState.None;
                HideCooldown();
                return;
            }

            bool isPassive = IsPassiveSkill();
            if (isPassive)
            {
                _currentState = SkillSlotState.Available;
                HideCooldown();
                UpdateAvailability(isAvailable);
                return;
            }

            if (currentCooldown > 0f && maxCooldown > 0f)
            {
                _currentState = SkillSlotState.Cooldown;
            }
            else
            {
                _currentState = SkillSlotState.Available;
            }

            UpdateCooldownGauge(currentCooldown, maxCooldown);
            UpdateCooldownText(currentCooldown);
            UpdateAvailability(isAvailable);
        }

        public void UpdateDuration(float currentDuration, float maxDuration)
        {
            if (_skillGauge == null || !IsSkillValid())
            {
                _skillGauge?.ResetFrontValue();
                return;
            }

            if (maxDuration > 0f && currentDuration > 0f)
            {
                float fillAmount = currentDuration / maxDuration;
                _skillGauge.SetFrontValue(fillAmount);
            }
            else
            {
                _skillGauge.ResetFrontValue();
            }
        }

        private bool IsSkillValid()
        {
            return _currentSkillName != SkillNames.None && _skillAsset != null;
        }

        private bool IsPassiveSkill()
        {
            return _skillAsset?.Data?.Type == SkillTypes.Passive;
        }

        private void UpdateCooldownGauge(float currentCooldown, float maxCooldown)
        {
            if (_skillGauge == null || _skillAsset?.Data == null)
            {
                return;
            }

            if (maxCooldown <= 0f || currentCooldown <= 0f)
            {
                _skillGauge.ResetFrontValue();
                return;
            }

            CooldownTypes cooldownType = _skillAsset.Data.CooldownType;
            Color gaugeColor = cooldownType == CooldownTypes.TimeBased ? _timeBasedColor : _attackCountBasedColor;

            _skillGauge.SetFrontColor(gaugeColor);
            float fillAmount = 1f - (currentCooldown / maxCooldown);
            _skillGauge.SetFrontValue(fillAmount);
        }

        private void UpdateCooldownText(float currentCooldown)
        {
            if (_skillGauge == null || _skillAsset?.Data == null || currentCooldown <= 0f)
            {
                _skillGauge?.ResetValueText();
                return;
            }

            CooldownTypes cooldownType = _skillAsset.Data.CooldownType;
            string text = string.Empty;

            if (cooldownType == CooldownTypes.TimeBased)
            {
                text = "0.0";
            }
            else if (cooldownType == CooldownTypes.AttackCountBased)
            {
                text = Mathf.CeilToInt(currentCooldown).ToString();
            }

            if (string.IsNullOrEmpty(text))
            {
                _skillGauge.ResetValueText();
            }
            else
            {
                _skillGauge.SetValueText(text);
            }
        }

        private void UpdateAvailability(bool isAvailable)
        {
            if (_skillIconImage != null)
            {
                _skillIconImage.SetGreyScale(!isAvailable);
            }

            if (_skillButton != null)
            {
                _skillButton.interactable = isAvailable;
            }
        }

        private void HideCooldown()
        {
            if (_skillGauge != null)
            {
                _skillGauge.ResetFrontValue();
                _skillGauge.ResetValueText();
            }

            if (_skillIconImage != null)
            {
                _skillIconImage.SetGreyScale(false);
            }
        }

        private int GetUnlockedSlotCount(VCharacterSkill characterSkill)
        {
            if (characterSkill == null || !characterSkill.Slots.IsValid())
            {
                return 0;
            }

            int count = 0;
            for (int i = 0; i < characterSkill.Slots.Count; i++)
            {
                if (characterSkill.Slots[i].IsUnlocked)
                {
                    count++;
                }
            }

            return count;
        }

        private void SetLockedState()
        {
            _currentState = SkillSlotState.Locked;
            ResetSkillData();
            ResetUIElements();
            if (_skillButton != null)
            {
                _skillButton.interactable = false;
            }
            if (_unlockableIndicator != null)
            {
                _unlockableIndicator.SetActive(false);
            }
            HideCooldown();
        }

        private void SetUnlockableState()
        {
            _currentState = SkillSlotState.Unlockable;
            ResetSkillData();
            ResetUIElements();
            if (_skillButton != null)
            {
                _skillButton.interactable = true;
            }
            if (_unlockableIndicator != null)
            {
                _unlockableIndicator.SetActive(true);
            }
            HideCooldown();
        }

        private void ClearSlot()
        {
            _currentState = SkillSlotState.None;
            ResetSkillData();
            ResetUIElements();
            _unlockableIndicator?.SetActive(false);
            HideCooldown();
        }

        private void ResetSkillData()
        {
            _currentSkillName = SkillNames.None;
            _currentSkill = null;
            _skillAsset = null;
        }

        private void ResetUIElements()
        {
            if (_skillIconImage != null)
            {
                _skillIconImage.enabled = false;
            }

            if (_passiveIndicator != null)
            {
                _passiveIndicator.SetActive(false);
            }

            if (_skillGauge != null)
            {
                _skillGauge.ResetFrontValue();
            }
        }

        private void OnSkillButtonClicked()
        {
            if (_currentSkillName == SkillNames.None)
            {
                return;
            }

            // TODO: 스킬 사용 로직 구현
            Log.Info(LogTags.UI_Page, "스킬 사용 버튼 클릭: {0}", _currentSkillName);
        }
    }
}