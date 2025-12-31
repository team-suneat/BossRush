using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffSystem : XBehaviour
    {
        private BuffEntity SpawnBuffEntity(BuffAssetData assetData, int level, Character caster)
        {
            BuffEntity buffEntity = null;
            if (assetData.UseSpawnCustomPrefab)
            {
                buffEntity = ResourcesManager.SpawnBuffEntity(assetData.Name, transform);
            }
            else
            {
                buffEntity = ResourcesManager.SpawnBuffEntity(transform);
            }

            if (buffEntity != null)
            {
                buffEntity.Setup(assetData, Owner, caster, level);
            }
            else
            {
                Log.Error("버프를 생성할 수 없습니다. Buff: {0}, BuffLevel: {1}, Caster: {2}",
                    assetData.Name.ToLogString(), level.ToSelectString(1), caster.GetHierarchyName());
            }

            return buffEntity;
        }

        /// <summary>
        /// 추가한 또는 덮어씌운 버프 독립체를 활성화합니다.
        /// </summary>
        private void ActivateEntity(BuffEntity buffEntity, Vector3 entityPosition)
        {
            if (buffEntity != null)
            {
                if (_stacks.ContainsKey(buffEntity.Name))
                {
                    BuffStack buffStack = _stacks[buffEntity.Name];

                    buffEntity.ActivateStackRenderers(buffStack);
                }

                if (!entityPosition.IsZero())
                {
                    buffEntity.position = entityPosition;
                }

                buffEntity.Activate();
            }
        }

        /// <summary>
        /// 버프 독립체의 레벨 또는 스택을 올립니다.
        /// </summary>
        private void OverlapEntity(BuffEntity buffEntity, int buffLevel)
        {
            if (buffEntity.CheckRemovingStackSequentially())
            {
                LogWarning("{0}, 버프의 스택을 순차적으로 제거중입니다. 버프를 추가할 수 없습니다.", buffEntity.Name.ToLogString());

                return;
            }

            LogInfo("{0}, 버프를 덮어씌웁니다. Level:{1} ▶ {2}", buffEntity.Name.ToLogString(), buffEntity.Level, buffLevel);

            if (!buffEntity.AssetData.IgnoreElapsedTimeResetOnOverlap)
            {
                buffEntity.ResetElapsedTime();
            }

            if (buffEntity.AddStackCount())
            {
                buffEntity.RefreshStats();
            }
            else if (buffEntity.Level != buffLevel)
            {
                if (buffEntity.AssetData != null)
                {
                    if (!buffEntity.AssetData.IgnoreResetLevel)
                    {
                        buffEntity.SetLevel(buffLevel);
                        buffEntity.RefreshStats();
                    }
                }
            }

            buffEntity.PlayOverlapFeedbacks();

            StartXCoroutine(SendAddedBuffGlobalEvent(buffEntity.Name, buffEntity.Level));
        }
    }
}