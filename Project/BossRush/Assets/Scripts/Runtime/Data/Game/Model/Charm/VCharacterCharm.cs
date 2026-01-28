using System;
using System.Collections.Generic;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    [System.Serializable]
    public class VCharacterCharm
    {
        public Dictionary<string, VCharm> Charms = new();
        public List<string> UnlockedCharms = new();
        public List<string> SlotCharmNameStrings = new();
        public int UnlockedSlotCount;

        [NonSerialized]
        private readonly Dictionary<CharmName, VCharm> _charmMap = new();

        [NonSerialized]
        private readonly List<CharmName> _slotCharmNames = new();

        [NonSerialized]
        private CharmName _activeInputCastCharmName = CharmName.None;

        public IReadOnlyList<CharmName> SlotCharmNames => _slotCharmNames;

        public CharmName ActiveInputCastCharmName => _activeInputCastCharmName;

        public List<CharmName> GetCharmNames()
        {
            return new List<CharmName>(_slotCharmNames);
        }

        public List<VCharm> GetCharms()
        {
            List<VCharm> charms = new();
            foreach (CharmName charmName in _slotCharmNames)
            {
                if (_charmMap.TryGetValue(charmName, out VCharm charm))
                {
                    charms.Add(charm);
                }
            }

            return charms;
        }

        //

        public void OnLoadGameData()
        {
            BuildCharmMap();
            BuildSlotCharmNames();
            SyncSlotCharmNameStrings();
            UpdateActiveInputCastCharmCache();
        }

        //

        private void BuildCharmMap()
        {
            _charmMap.Clear();
            CharmName charmName = CharmName.None;
            foreach (KeyValuePair<string, VCharm> kvp in Charms)
            {
                VCharm charm = kvp.Value;
                charm.OnLoadGameData();

                if (!EnumEx.ConvertTo(ref charmName, kvp.Key))
                {
                    Log.Error(LogTags.Charm, "부적 키를 CharmName으로 변환하지 못했습니다: {0}", kvp.Key);
                    continue;
                }

                charm.Name = charmName;
                _charmMap[charmName] = charm;
                Log.Info(LogTags.Charm, "인게임 부적을 등록합니다: {0}", charmName.ToLogString());
            }
        }

        private void BuildSlotCharmNames()
        {
            _slotCharmNames.Clear();

            CharmName charmName = CharmName.None;
            for (int i = 0; i < SlotCharmNameStrings.Count; i++)
            {
                // 슬롯 문자열을 enum으로 복원합니다.
                string slotName = SlotCharmNameStrings[i];
                if (!EnumEx.ConvertTo(ref charmName, slotName))
                {
                    Log.Error(LogTags.Charm, "부적 슬롯 이름을 CharmName으로 변환하지 못했습니다: {0}", slotName);
                    continue;
                }

                // 세이브 데이터가 더 길어도, 현재 해금된 슬롯 수만큼만 구성합니다.
                if (_slotCharmNames.Count >= UnlockedSlotCount)
                {
                    break;
                }

                // 유효한 부적만 슬롯에 올립니다.
                if (_charmMap.ContainsKey(charmName))
                {
                    _slotCharmNames.Add(charmName);
                    Log.Info(LogTags.Charm, "인게임 부적을 슬롯에 추가합니다: {0}", charmName.ToLogString());
                }
            }
        }

        public bool CheckUnlocked(CharmName charmName)
        {
            return UnlockedCharms.Contains(charmName.ToString());
        }

        public void Unlock(CharmName charmName)
        {
            string key = charmName.ToString();
            if (!UnlockedCharms.Contains(key))
            {
                UnlockedCharms.Add(key);
                Log.Info(LogTags.Charm, "부적을 해금합니다: {0}", charmName);

                // 실제로 해금된 경우에만 이벤트 전송
                GlobalEvent<CharmName>.Send(GlobalEventType.CHARM_UNLOCKED, charmName);
            }
        }

        //

        public bool HasCharm(CharmName charmName)
        {
            return Charms.ContainsKey(charmName.ToString());
        }

        public VCharm FindCharm(CharmName charmName)
        {
            if (_charmMap.TryGetValue(charmName, out VCharm charm))
            {
                return charm;
            }

            Log.Warning(LogTags.Charm, "부적을 찾을 수 없습니다: {0}", charmName.ToLogString());
            return null;
        }

        public void AddCharm(CharmName charmName)
        {
            if (_slotCharmNames.Count >= UnlockedSlotCount)
            {
                Log.Warning(LogTags.Charm, "부적 슬롯이 가득 찼습니다. 현재/최대: {0}/{1}", _slotCharmNames.Count, UnlockedSlotCount);
                return;
            }

            string key = charmName.ToString();
            if (!_charmMap.ContainsKey(charmName))
            {
                VCharm newCharm = new(charmName);
                Charms[key] = newCharm;
                _charmMap[charmName] = newCharm;

                Log.Info(LogTags.Charm, "인게임 부적을 등록합니다: {0}", charmName.ToLogString());
            }

            bool wasSlotAdded = false;
            if (!_slotCharmNames.Contains(charmName))
            {
                _slotCharmNames.Add(charmName);
                SlotCharmNameStrings.Add(key);
                wasSlotAdded = true;
            }

            // 실제로 슬롯에 추가된 경우에만 이벤트 전송
            if (wasSlotAdded)
            {
                UpdateActiveInputCastCharmCache();
                GlobalEvent<CharmName>.Send(GlobalEventType.CHARM_ADDED, charmName);
            }
        }

        public void RemoveCharm(CharmName charmName)
        {
            string key = charmName.ToString();
            bool wasRemoved = false;
            if (_charmMap.ContainsKey(charmName))
            {
                _ = Charms.Remove(key);
                _ = _charmMap.Remove(charmName);
                bool wasSlotRemoved = _slotCharmNames.Remove(charmName);
                _ = SlotCharmNameStrings.Remove(key);

                Log.Info(LogTags.Charm, "인게임 부적을 등록해제합니다: {0}", charmName.ToLogString());
                wasRemoved = wasSlotRemoved;
            }

            // 실제로 슬롯에서 제거된 경우에만 이벤트 전송
            if (wasRemoved)
            {
                UpdateActiveInputCastCharmCache();
                GlobalEvent<CharmName>.Send(GlobalEventType.CHARM_REMOVED, charmName);
            }
        }

        //

        public static VCharacterCharm CreateDefault()
        {
            VCharacterCharm defaultCharms = new();
            defaultCharms.InitializeDefaultSaveData();

            return defaultCharms;
        }

        private void InitializeDefaultSaveData()
        {
            // 주의: 세이브 기본값 초기화 전용입니다.
            // - 이벤트/로그를 발생시키지 않습니다.
            // - 런타임 캐시(_charmMap, _slotCharmNames)를 직접 만지지 않습니다.
            // - 저장될 필드(Charms/UnlockedCharms/SlotCharmNameStrings/UnlockedSlotCount)만 세팅합니다.

            Charms.Clear();
            UnlockedCharms.Clear();
            SlotCharmNameStrings.Clear();

            CharmName defaultCharmName = CharmName.DefaultCharm;
            string defaultCharmKey = defaultCharmName.ToString();

            UnlockedSlotCount = 1;
            Charms[defaultCharmKey] = new VCharm(defaultCharmName);
            UnlockedCharms.Add(defaultCharmKey);
            SlotCharmNameStrings.Add(defaultCharmKey);

            UpdateActiveInputCastCharmCache();

            Log.Info(LogTags.Charm, "기본 세이브 데이터를 초기화합니다. 해금된 슬롯 수:{0}, 소지한 부적 수:{1}, 액티브+인풋 캐스트 트리거 스킬을 가진 부적: {2}",
                UnlockedSlotCount,
                UnlockedCharms.Count,
                ActiveInputCastCharmName.ToLogString());
        }

        public void UnlockSlot(int count)
        {
            int oldCount = UnlockedSlotCount;
            UnlockedSlotCount = Mathf.Max(0, UnlockedSlotCount + count);

            if (UnlockedSlotCount != oldCount)
            {
                Log.Info(LogTags.Charm, "부적 슬롯을 해금합니다. 현재 슬롯 수: {0}", UnlockedSlotCount);
                GlobalEvent<int>.Send(GlobalEventType.CHARM_SLOT_UNLOCKED, UnlockedSlotCount);
            }
        }

        public void LockSlot(int count)
        {
            int oldCount = UnlockedSlotCount;
            UnlockedSlotCount = Mathf.Max(0, UnlockedSlotCount - count);

            if (UnlockedSlotCount != oldCount)
            {
                Log.Info(LogTags.Charm, "부적 슬롯을 잠금합니다. 현재 슬롯 수: {0}", UnlockedSlotCount);
                GlobalEvent<int>.Send(GlobalEventType.CHARM_SLOT_LOCKED, UnlockedSlotCount);
            }
        }

        private void SyncSlotCharmNameStrings()
        {
            SlotCharmNameStrings.Clear();
            for (int i = 0; i < _slotCharmNames.Count; i++)
            {
                SlotCharmNameStrings.Add(_slotCharmNames[i].ToString());
            }
        }

        private void UpdateActiveInputCastCharmCache()
        {
            _activeInputCastCharmName = CharmName.None;

            if (ScriptableDataManager.Instance == null)
            {
                return;
            }

            for (int i = 0; i < _slotCharmNames.Count; i++)
            {
                CharmName charmName = _slotCharmNames[i];
                CharmAssetData charmData = ScriptableDataManager.Instance.FindCharmClone(charmName);
                if (!charmData.IsValid() ||
                    (charmData.ApplicationType & CharmApplicationType.Skill) == 0)
                {
                    continue;
                }

                SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(charmData.SkillName);
                if (skillData.IsValid() &&
                    skillData.Type == SkillType.Active &&
                    skillData.TriggerType == SkillTriggerType.InputCast)
                {
                    _activeInputCastCharmName = charmName;
                    return;
                }
            }
        }
    }
}
