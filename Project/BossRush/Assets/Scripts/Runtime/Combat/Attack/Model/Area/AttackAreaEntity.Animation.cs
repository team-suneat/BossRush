

namespace TeamSuneat
{
    public partial class AttackAreaEntity : AttackEntity
    {
        private void SetActiveAnimatorParameter(bool isHit)
        {
            if (!AssetData.IsValid())
            {
                return;
            }

            if (!AssetData.UseHitAnimatorParamter)
            {
                return;
            }

            if (Owner == null)
            {
                return;
            }

            if (Owner.Animator == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(NameString))
            {
                return;
            }

            
            _ = Owner.Animator.UpdateAnimatorBoolIfExists(NameString + "Active", isHit);
            
        }

        private void SetHitAnimatorParameter(bool isHit)
        {
            if (!AssetData.IsValid())
            {
                return;
            }

            if (Owner == null)
            {
                return;
            }

            if (Owner.Animator == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(NameString))
            {
                return;
            }

            if (AssetData.UseHitAnimatorParamter)
            {
                _ = Owner.Animator.UpdateAnimatorFloatIfExists(NameString + "Hit", isHit ? 1f : 0f);
            }
        }
    }
}