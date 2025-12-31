using TeamSuneat.Stage;
using UnityEngine;

namespace TeamSuneat
{
    public partial class ResourcesManager
    {
        internal static StageSystem SpawnStage(StageNames stageName, Transform point)
        {
            StageSystem stageSystem = SpawnPrefab<StageSystem>(stageName.ToString(), point);
            if (stageSystem != null)
            {
                stageSystem.ResetLocalTransform();
            }

            return stageSystem;
        }
    }
}