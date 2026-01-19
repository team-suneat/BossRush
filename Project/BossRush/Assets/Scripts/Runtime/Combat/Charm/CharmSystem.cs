using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class CharmSystem : XBehaviour
    {
        private readonly Dictionary<CharmName, CharmAssetData> _activeCharms = new();

        public PlayerCharacter Owner { get; private set; }

        private void Awake()
        {
            Owner = this.FindFirstParentComponent<PlayerCharacter>();
        }

        public bool HasCharm(CharmName charmName)
        {
            return _activeCharms.ContainsKey(charmName);
        }

        public void AddCharm(CharmName charmName)
        {
            if (charmName == CharmName.None || Owner == null)
            {
                return;
            }

            if (_activeCharms.ContainsKey(charmName))
            {
                Log.Warning(LogTags.Charm, "부적이 이미 적용되어 있습니다: {0}", charmName.ToLogString());
                return;
            }

            CharmAssetData charmData = ScriptableDataManager.Instance.FindCharmClone(charmName);
            if (!charmData.IsValid())
            {
                Log.Warning(LogTags.Charm, "부적 데이터를 찾을 수 없습니다: {0}", charmName.ToLogString());
                return;
            }

            _activeCharms[charmName] = charmData;
            ApplyCharmEffect(charmData);
        }

        public void RemoveCharm(CharmName charmName)
        {
            if (!_activeCharms.TryGetValue(charmName, out CharmAssetData charmData))
            {
                return;
            }

            RemoveCharmEffect(charmData);
            _activeCharms.Remove(charmName);
        }

        public void ClearAll()
        {
            int count = _activeCharms.Count;
            foreach (var kvp in _activeCharms)
            {
                RemoveCharmEffect(kvp.Value);
            }
            _activeCharms.Clear();

            if (Owner != null && count > 0)
            {
                Log.Info(LogTags.Charm, "{0}의 모든 부적 효과 제거: {1}개", Owner.Name.ToLogString(), count);
            }
        }

        private void ApplyCharmEffect(CharmAssetData charmData)
        {
            if (charmData == null || !charmData.IsValid())
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}에게 부적 효과를 적용합니다: {1} ({2})", Owner.Name.ToLogString(), charmData.Name.ToLogString(), charmData.ApplicationType);

            // 버프 적용
            if ((charmData.ApplicationType & CharmApplicationType.Buff) != 0)
            {
                ApplyCharmBuff(charmData.BuffName);
            }

            // 스킬 적용
            if ((charmData.ApplicationType & CharmApplicationType.Skill) != 0)
            {
                ApplyCharmSkill(charmData.SkillName);
            }

            // 패시브 적용
            if ((charmData.ApplicationType & CharmApplicationType.Passive) != 0)
            {
                ApplyCharmPassive(charmData.PassiveName);
            }
        }

        private void RemoveCharmEffect(CharmAssetData charmData)
        {
            if (charmData == null || !charmData.IsValid())
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}에서 부적 효과를 제거합니다: {1} ({2})", Owner.Name.ToLogString(), charmData.Name.ToLogString(), charmData.ApplicationType);

            // 버프 해제
            if ((charmData.ApplicationType & CharmApplicationType.Buff) != 0)
            {
                RemoveCharmBuff(charmData.BuffName);
            }

            // 스킬 해제
            if ((charmData.ApplicationType & CharmApplicationType.Skill) != 0)
            {
                RemoveCharmSkill(charmData.SkillName);
            }

            // 패시브 해제
            if ((charmData.ApplicationType & CharmApplicationType.Passive) != 0)
            {
                RemoveCharmPassive(charmData.PassiveName);
            }
        }

        private void ApplyCharmBuff(BuffName buffName)
        {
            if (buffName == BuffName.None)
            {
                Log.Warning(LogTags.Charm, "적용할 버프 이름이 없습니다.");
                return;
            }

            if (Owner.Buff == null)
            {
                Log.Warning(LogTags.Charm, "버프 시스템이 존재하지 않습니다.");
                return;
            }

            Owner.Buff.Add(buffName, 1, Owner);
            Log.Info(LogTags.Charm, "{0}에게 부적 버프를 적용했습니다: {1}", Owner.Name.ToLogString(), buffName.ToLogString());
        }

        private void RemoveCharmBuff(BuffName buffName)
        {
            if (buffName == BuffName.None)
            {
                return;
            }

            if (Owner.Buff == null)
            {
                return;
            }

            Owner.Buff.Remove(buffName);
            Log.Info(LogTags.Charm, "{0}에서 부적 버프를 제거했습니다: {1}", Owner.Name.ToLogString(), buffName.ToLogString());
        }

        private void ApplyCharmSkill(SkillName skillName)
        {
            if (skillName == SkillName.None)
            {
                Log.Warning(LogTags.Charm, "적용할 스킬 이름이 없습니다.");
                return;
            }

            // TODO: 스킬 적용 로직 구현
            Log.Info(LogTags.Charm, "{0}에게 부적 스킬 적용 준비됨 (구현 예정): {1}", Owner.Name.ToLogString(), skillName.ToLogString());
        }

        private void RemoveCharmSkill(SkillName skillName)
        {
            if (skillName == SkillName.None)
            {
                return;
            }

            // TODO: 스킬 해제 로직 구현
            Log.Info(LogTags.Charm, "{0}에서 부적 스킬 해제 준비됨 (구현 예정): {1}", Owner.Name.ToLogString(), skillName.ToLogString());
        }

        private void ApplyCharmPassive(PassiveName passiveName)
        {
            if (passiveName == PassiveName.None)
            {
                Log.Warning(LogTags.Charm, "적용할 패시브 이름이 없습니다.");
                return;
            }

            // TODO: 패시브 적용 로직 구현
            Log.Info(LogTags.Charm, "{0}에게 부적 패시브 적용 준비됨 (구현 예정): {1}", Owner.Name.ToLogString(), passiveName.ToLogString());
        }

        private void RemoveCharmPassive(PassiveName passiveName)
        {
            if (passiveName == PassiveName.None)
            {
                return;
            }

            // TODO: 패시브 해제 로직 구현
            Log.Info(LogTags.Charm, "{0}에서 부적 패시브 해제 준비됨 (구현 예정): {1}", Owner.Name.ToLogString(), passiveName.ToLogString());
        }

        protected override void RegisterGlobalEvent()
        {
            GlobalEvent<CharmName>.Register(GlobalEventType.CHARM_UNLOCKED, OnCharmUnlocked);
            GlobalEvent<CharmName>.Register(GlobalEventType.CHARM_ADDED, OnCharmAdded);
            GlobalEvent<CharmName>.Register(GlobalEventType.CHARM_REMOVED, OnCharmRemoved);
            GlobalEvent<int>.Register(GlobalEventType.CHARM_SLOT_UNLOCKED, OnCharmSlotUnlocked);
            GlobalEvent<int>.Register(GlobalEventType.CHARM_SLOT_LOCKED, OnCharmSlotLocked);
        }

        protected override void UnregisterGlobalEvent()
        {
            GlobalEvent<CharmName>.Unregister(GlobalEventType.CHARM_UNLOCKED, OnCharmUnlocked);
            GlobalEvent<CharmName>.Unregister(GlobalEventType.CHARM_ADDED, OnCharmAdded);
            GlobalEvent<CharmName>.Unregister(GlobalEventType.CHARM_REMOVED, OnCharmRemoved);
            GlobalEvent<int>.Unregister(GlobalEventType.CHARM_SLOT_UNLOCKED, OnCharmSlotUnlocked);
            GlobalEvent<int>.Unregister(GlobalEventType.CHARM_SLOT_LOCKED, OnCharmSlotLocked);
        }

        private void OnCharmUnlocked(CharmName charmName)
        {
            if (Owner == null)
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}: 부적 해금 이벤트 수신: {1}", Owner.Name.ToLogString(), charmName.ToLogString());
        }

        private void OnCharmAdded(CharmName charmName)
        {
            if (Owner == null)
            {
                return;
            }

            AddCharm(charmName);
        }

        private void OnCharmRemoved(CharmName charmName)
        {
            if (Owner == null)
            {
                return;
            }

            RemoveCharm(charmName);
        }

        private void OnCharmSlotUnlocked(int slotCount)
        {
            if (Owner == null)
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}: 부적 슬롯 해금 이벤트 수신: {1}개", Owner.Name.ToLogString(), slotCount);
        }

        private void OnCharmSlotLocked(int slotCount)
        {
            if (Owner == null)
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}: 부적 슬롯 잠금 이벤트 수신: {1}개", Owner.Name.ToLogString(), slotCount);
        }
    }
}
