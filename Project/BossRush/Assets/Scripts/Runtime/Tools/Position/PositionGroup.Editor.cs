using Sirenix.OdinInspector;
using System.Diagnostics;

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

            if (HitmarkName != HitmarkNames.None)
            {
                HitmarkNameString = HitmarkName.ToString();
            }
        }

        private void OnValidate()
        {
            if (!EnumEx.ConvertTo(ref HitmarkName, HitmarkNameString))
            {
                if (!string.IsNullOrEmpty(HitmarkNameString))
                {
                    Log.Error("포지션 그룹의 히트마크({0})를 변환할 수 없습니다: {1}", HitmarkNameString, this.GetHierarchyPath());
                }
            }
        }

        public override void AutoNaming()
        {
            if (HitmarkName != HitmarkNames.None)
            {
                SetGameObjectName(string.Format("Attack Position Group({0})", HitmarkName));
            }
        }

        [FoldoutGroup("#Custom Buttons", 1000)]
        [Button("Paste Children Positions", ButtonSizes.Medium)]
        [Conditional("UNITY_EDITOR")]
        public void PasteChildrenPositions()
        {
            if (LinkedPositionGroup != null)
            {
                for (int i = 0; i < LinkedPositionGroup.Children.Count; i++)
                {
                    if (Children.Count > i)
                    {
                        Children[i].position = LinkedPositionGroup.Children[i].position;
                    }
                }
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