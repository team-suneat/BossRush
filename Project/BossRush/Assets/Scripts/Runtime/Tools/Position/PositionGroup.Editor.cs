using Sirenix.OdinInspector;

namespace TeamSuneat
{
    public partial class PositionGroup : XBehaviour
    {
#if UNITY_EDITOR

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            SetupChildren();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (PositionGroupName != PositionGroupNames.None)
            {
                PositionGroupNameString = PositionGroupName.ToString();
            }
        }

        private void OnValidate()
        {
            if (!EnumEx.ConvertTo(ref PositionGroupName, PositionGroupNameString))
            {
                if (!string.IsNullOrEmpty(PositionGroupNameString))
                {
                    Log.Error($"포지션 그룹의 키({PositionGroupNameString})를 변환할 수 없습니다: {this.GetHierarchyPath()}");
                }
            }
        }

        public override void AutoNaming()
        {
            if (PositionGroupName != PositionGroupNames.None)
            {
                SetGameObjectName($"Position Group({PositionGroupName})");
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (ShowChildrenPositionInGizmo)
            {
                for (int i = 0; i < Children.Count; i++)
                {
                    if (Children[i] == null)
                    {
                        Children.RemoveNull();
                        break;
                    }

                    GizmoEx.DrawGizmoCross(Children[i].position, 0.2f, GameColors.Dev);
                    GizmoEx.DrawText((i + 1).ToString(), Children[i].position, GameColors.Dev);
                }
            }
        }

#endif
    }
}