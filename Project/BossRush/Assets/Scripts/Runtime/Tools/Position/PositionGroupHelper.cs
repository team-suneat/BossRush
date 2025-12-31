using Sirenix.OdinInspector;

namespace TeamSuneat
{
    public class PositionGroupHelper : XBehaviour
    {
        [Button]
        private void LogTagetTypePositionGroup()
        {
            var groups = GetComponentsInChildren<PositionGroup>(true);

            foreach (var item in groups)
            {
                Log.Progress("PositionGroup.Types:{1} 포지션 그룹: {0}", item.GetHierarchyPath(), item.Type.ToSelectString(PositionGroup.Types.None));
            }

            Log.Info("포지션 그룹 로그 순환 완료: {0}", groups.Length);
        }
    }
}