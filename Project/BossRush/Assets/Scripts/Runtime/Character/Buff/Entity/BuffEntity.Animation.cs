using Lean.Pool;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        private void ApplyAnimation()
        {
            if (Owner != null && Owner.Animator != null)
            {
                if (!string.IsNullOrEmpty(AssetData.AnimationBool))
                {
                    if (Owner.Animator.UpdateAnimatorBoolIfExists(AssetData.AnimationBool, true))
                    {
                        LogInfo("버프에 등록된 캐릭터의 애니메이션의 Bool 파라메터를 적용합니다. {0}, {1}", AssetData.AnimationBool, true.ToBoolString());
                    }
                }
            }
        }

        private void ReleaseAnimation()
        {
            if (Owner != null && Owner.Animator != null)
            {
                if (!string.IsNullOrEmpty(AssetData.AnimationBool))
                {
                    if (Owner.Animator.UpdateAnimatorBoolIfExists(AssetData.AnimationBool, false))
                    {
                        LogInfo("버프에 등록된 캐릭터의 애니메이션의 Bool 파라메터를 적용 해제합니다. {0}, {1}", AssetData.AnimationBool, false.ToBoolString());
                    }
                }
            }
        }
    }
}