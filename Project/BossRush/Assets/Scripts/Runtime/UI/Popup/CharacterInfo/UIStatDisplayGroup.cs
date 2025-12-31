using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // 능력치 표시 그룹 컴포넌트
    public class UIStatDisplayGroup : XBehaviour
    {
        [Title("#UIStatDisplayGroup")]
        [SerializeField] private UIStatDisplayEntry[] _statEntries;

        // 캐릭터 정보에서 표시할 기본 능력치 목록
        private static readonly StatNames[] DISPLAY_STATS = new StatNames[]
        {
            StatNames.Attack,
            StatNames.Health,
            StatNames.HealthRegen,
            StatNames.CriticalChance,
            StatNames.CriticalDamage,
            StatNames.Mana,
            StatNames.ManaRegen,
            StatNames.Accuracy,
            StatNames.Dodge,
            StatNames.GoldGain,
            StatNames.XPGain,
        };

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            if (_statEntries == null || _statEntries.Length == 0)
            {
                _statEntries = this.GetComponentsInChildren<UIStatDisplayEntry>(true);
            }
        }

        public void RefreshStats(StatSystem statSystem)
        {
            if (statSystem == null)
            {
                ClearAll();
                return;
            }

            int entryCount = _statEntries.Length;
            int displayCount = DISPLAY_STATS.Length;

            for (int i = 0; i < entryCount; i++)
            {
                UIStatDisplayEntry entry = _statEntries[i];
                if (entry == null)
                {
                    continue;
                }

                if (i < displayCount)
                {
                    StatNames statName = DISPLAY_STATS[i];
                    float value = statSystem.FindValueOrDefault(statName);
                    entry.SetData(statName, value);
                    entry.SetActive(true);
                }
                else
                {
                    entry.Clear();
                    entry.SetActive(false);
                }
            }
        }

        public void RefreshStatsFromProfile()
        {
            PlayerCharacter player = CharacterManager.Instance?.Player;
            if (player != null && player.Stat != null)
            {
                RefreshStats(player.Stat);
            }
            else
            {
                ClearAll();
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < _statEntries.Length; i++)
            {
                _statEntries[i]?.Clear();
            }
        }

        public int GetEntryCount()
        {
            return _statEntries.Length;
        }

        public UIStatDisplayEntry GetEntry(int index)
        {
            if (!_statEntries.IsValid(index))
            {
                return null;
            }

            return _statEntries[index];
        }
    }
}