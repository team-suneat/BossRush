using Sirenix.OdinInspector;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class AttackSystem : XBehaviour
    {
        [FoldoutGroup("#Attack Settings")]
        [SuffixLabel("기본 공격 히트마크")]
        [SerializeField] private HitmarkNames _basicAttackHitmark;

        [FoldoutGroup("#Attack Settings")]
        [SerializeField] private string _basicAttackHitmarkString;

        [FoldoutGroup("#Attack Settings")]
        [SuffixLabel("캐릭터")]
        [SerializeField] private Character _ownerCharacter;

        private readonly AttackEntityRegistry _registry = new();

        public HitmarkNames BasicAttackHitmark => _basicAttackHitmark;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _ownerCharacter = this.FindFirstParentComponent<Character>();
        }

        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref _basicAttackHitmark, _basicAttackHitmarkString);
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            _basicAttackHitmarkString = _basicAttackHitmark.ToString();
        }

        public override void AutoNaming()
        {
            SetGameObjectName($"#Attack({_ownerCharacter.NameString})");
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            _registry.Clear();
            RegisterAll();
        }

        private void RegisterAll()
        {
            AttackEntity[] entities = GetComponentsInChildren<AttackEntity>();
            if (entities.IsValid())
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    RegisterSingleEntity(entities[i]);
                }
            }
        }

        private void RegisterSingleEntity(AttackEntity attackEntity)
        {
            attackEntity.Initialize();
            _registry.Register(attackEntity);
        }

        public void Initialize()
        {
        }

        public void OnBattleReady()
        {
            if (_registry.IsValid())
            {
                foreach (KeyValuePair<HitmarkNames, AttackEntity> item in _registry.GetAllEntities())
                {
                    item.Value.OnBattleReady();
                }
            }
        }

        //───────────────────────────────────────────────────────────────────────────────────────────────────────────────

        public void ActivateBasic()
        {
            if (_basicAttackHitmark != HitmarkNames.None)
            {
                Activate(_basicAttackHitmark);
            }
            else
            {
                Log.Warning("기본 공격 히트마크가 설정되지 않았습니다.");
            }
        }

        public void Activate(string hitmarkNameString, float? weaponDamageOverride = null)
        {
            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName != HitmarkNames.None)
            {
                Activate(hitmarkName, weaponDamageOverride);
            }
            else
            {
                LogFailedToFindEntity(hitmarkName, hitmarkNameString);
            }
        }

        public void Activate(HitmarkNames hitmarkName, float? weaponDamageOverride = null)
        {
            if (!ValidateHitmarkName(hitmarkName))
            {
                return;
            }

            AttackEntity entity = _registry.Find(hitmarkName);
            if (entity != null)
            {
                entity.SetWeaponDamageOverride(weaponDamageOverride);
                entity.Activate();
            }
            else
            {
                LogFailedToFindEntity(hitmarkName);
            }
        }

        public void Deactivate(string hitmarkNameString)
        {
            HitmarkNames hitmarkName = DataConverter.ToEnum<HitmarkNames>(hitmarkNameString);
            if (hitmarkName != HitmarkNames.None)
            {
                Deactivate(hitmarkName);
            }
            else
            {
                LogFailedToFindEntity(hitmarkName, hitmarkNameString);
            }
        }

        public void Deactivate(HitmarkNames hitmarkName)
        {
            if (!ValidateHitmarkName(hitmarkName) || !_registry.IsValid() || !_registry.Contains(hitmarkName))
            {
                return;
            }

            AttackEntity attackEntity = _registry.Find(hitmarkName);
            attackEntity?.Deactivate();
        }

        public void DeactivateAll()
        {
            if (!_registry.IsValid())
            {
                return;
            }

            var hitmarks = _registry.GetAllHitmarks();
            for (int i = 0; i < hitmarks.Count; i++)
            {
                Deactivate(hitmarks[i]);
            }
        }

        //

        public void OnDeath()
        {
            if (!_registry.IsValid())
            {
                return;
            }

            var hitmarks = _registry.GetAllHitmarks();
            for (int i = 0; i < hitmarks.Count; i++)
            {
                NotifyEntityOwnerDeath(hitmarks[i]);
            }
        }

        private void NotifyEntityOwnerDeath(HitmarkNames hitmarkName)
        {
            AttackEntity entity = _registry.Find(hitmarkName);
            if (entity != null)
            {
                entity.OnOwnerDeath();
            }
        }

        public bool ContainEntity(HitmarkNames hitmarkName)
        {
            return _registry.Contains(hitmarkName);
        }

        public AttackEntity FindEntity(HitmarkNames hitmarkName)
        {
            return _registry.Find(hitmarkName);
        }

        public AttackEntity CreateAndRegisterEntity(HitmarkAssetData assetData)
        {
            if (_registry.Contains(assetData.Name))
            {
                return _registry.Find(assetData.Name);
            }

            AttackEntity attackEntity = ResourcesManager.SpawnAttackEntity(assetData.Name, transform);
            if (attackEntity != null)
            {
                SetupEntity(attackEntity, assetData.Name);
            }
            else
            {
                Log.Error($"공격 독립체({assetData.Name.ToLogString()})를 찾을 수 없습니다. 새로운 공격 독립체를 생성할 수 없습니다.");
            }

            return attackEntity;
        }

        private void SetupEntity(AttackEntity attackEntity, HitmarkNames hitmarkName)
        {
            attackEntity.Name = hitmarkName;
            attackEntity.Initialize();
            _registry.Register(attackEntity);
        }

        public AttackEntity SpawnAndRegisterEntity(HitmarkNames hitmarkName)
        {
            if (_registry.Contains(hitmarkName))
            {
                return null;
            }

            AttackEntity attackEntity = ResourcesManager.SpawnAttackEntity(hitmarkName, transform);
            if (attackEntity != null)
            {
                attackEntity.SetOwner(_ownerCharacter);
                SetupEntity(attackEntity, hitmarkName);
            }

            return attackEntity;
        }

        // Validation Methods

        private bool ValidateHitmarkName(HitmarkNames hitmarkName)
        {
            if (hitmarkName == HitmarkNames.None)
            {
                Log.Warning(LogTags.Attack, "유효하지 않은 히트마크 이름입니다: {0}", hitmarkName.ToLogString());
                return false;
            }
            return true;
        }

        // Log Methods

        private void LogFailedToFindEntity(HitmarkNames hitmarkName)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Attack, "{0}, 설정된 히트마크 이름을 가진 Attack Entity로 찾을 수 없습니다.", hitmarkName.ToLogString());
            }
        }

        private void LogFailedToFindEntity(HitmarkNames hitmarkName, string hitmarkNameString)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Attack, "{0} ({1}), 설정된 히트마크 이름을 가진 Attack Entity로 찾을 수 없습니다.", hitmarkName.ToLogString(), hitmarkNameString);
            }
        }
    }
}