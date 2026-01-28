using System.Collections.Generic;
using TeamSuneat.Data;

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

            // 액티브+인풋 캐스트 트리거 스킬을 가진 부적은 동시에 하나만 유지합니다.
            if (IsActiveInputCastSkillCharm(charmData))
            {
                RemoveCharmsWithActiveInputCastSkill();
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

            // 스킬 적용
            if ((charmData.ApplicationType & CharmApplicationType.Skill) != 0)
            {
                ApplyCharmSkill(charmData.SkillName);
            }
        }

        private void RemoveCharmEffect(CharmAssetData charmData)
        {
            if (charmData == null || !charmData.IsValid())
            {
                return;
            }

            Log.Info(LogTags.Charm, "{0}에서 부적 효과를 제거합니다: {1} ({2})", Owner.Name.ToLogString(), charmData.Name.ToLogString(), charmData.ApplicationType);

            // 스킬 해제
            if ((charmData.ApplicationType & CharmApplicationType.Skill) != 0)
            {
                RemoveCharmSkill(charmData.SkillName);
            }
        }

        private void ApplyCharmSkill(SkillName skillName)
        {
            if (skillName == SkillName.None)
            {
                Log.Warning(LogTags.Charm, "적용할 스킬 이름이 없습니다.");
                return;
            }

            if (Owner.Skill == null)
            {
                Log.Warning(LogTags.Charm, "스킬 시스템이 존재하지 않습니다.");
                return;
            }

            Owner.Skill.AddSkill(skillName, level: 1);
            Log.Info(LogTags.Charm, "{0}에게 부적 스킬을 적용했습니다: {1}", Owner.Name.ToLogString(), skillName.ToLogString());
        }

        private void RemoveCharmSkill(SkillName skillName)
        {
            if (skillName == SkillName.None)
            {
                return;
            }

            if (Owner.Skill == null)
            {
                return;
            }

            Owner.Skill.RemoveSkill(skillName);
            Log.Info(LogTags.Charm, "{0}에서 부적 스킬을 제거했습니다: {1}", Owner.Name.ToLogString(), skillName.ToLogString());
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

        private List<CharmName> FindCharmsWithActiveInputCastSkill()
        {
            List<CharmName> result = new();

            foreach (KeyValuePair<CharmName, CharmAssetData> kvp in _activeCharms)
            {
                CharmAssetData charmData = kvp.Value;
                if (charmData == null ||
                    (charmData.ApplicationType & CharmApplicationType.Skill) == 0 ||
                    charmData.SkillName == SkillName.None)
                {
                    continue;
                }

                SkillAssetData skillData = ScriptableDataManager.Instance?.FindSkillClone(charmData.SkillName);
                if (skillData != null &&
                    skillData.Type == SkillType.Active &&
                    skillData.TriggerType == SkillTriggerType.InputCast)
                {
                    result.Add(kvp.Key);
                }
            }

            return result;
        }

        private bool IsActiveInputCastSkillCharm(CharmAssetData charmData)
        {
            if (charmData == null ||
                (charmData.ApplicationType & CharmApplicationType.Skill) == 0 ||
                charmData.SkillName == SkillName.None)
            {
                return false;
            }

            SkillAssetData skillData = ScriptableDataManager.Instance?.FindSkillClone(charmData.SkillName);
            return skillData != null &&
                   skillData.Type == SkillType.Active &&
                   skillData.TriggerType == SkillTriggerType.InputCast;
        }

        private void RemoveCharmsWithActiveInputCastSkill()
        {
            // 기존 부적 중 액티브+인풋 캐스트 트리거 스킬을 가진 부적 모두 제거
            List<CharmName> charmsToRemove = FindCharmsWithActiveInputCastSkill();
            for (int i = 0; i < charmsToRemove.Count; i++)
            {
                RemoveCharm(charmsToRemove[i]);
            }
        }
    }
}
