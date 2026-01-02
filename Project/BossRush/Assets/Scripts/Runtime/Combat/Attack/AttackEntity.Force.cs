using System;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class AttackEntity : XBehaviour
    {
        protected void StartAttackForce()
        {
            if (!IsAttackForceValid())
            {
                return;
            }

            CharacterForceVelocity attackerAbilityFV = Owner.FindAbility<CharacterForceVelocity>();
            if (attackerAbilityFV == null)
            {
                return;
            }

            ForceVelocityAsset attackForceAsset = ScriptableDataManager.Instance.FindForceVelocity(AssetData.AttackForce);
            if (!IsAttackForceAssetValid(attackForceAsset))
            {
                return;
            }

            if (attackForceAsset.Data.IsRequireHorizontalInput && !IsInputDirectionValid())
            {
                LogWarningForInvalidDirection();
                return;
            }

            attackerAbilityFV.StartForceVelocity(attackForceAsset, Owner.IsFacingRight, this);
        }

        private bool IsAttackForceValid()
        {
            if (!AssetData.IsValid())
            {
                return false;
            }

            if (AssetData.AttackForce == FVNames.None)
            {
                return false;
            }

            return true;
        }

        private bool IsAttackForceAssetValid(ForceVelocityAsset attackForceAsset)
        {
            if (!attackForceAsset.IsValid())
            {
                Log.Warning(LogTags.ForceVelocity, $"공격({Name.ToLogString()})의 공격 FV({AssetData.AttackForce.ToLogString()}) 에셋을 찾을 수 없습니다.");
                return false;
            }
            return true;
        }

        private bool IsInputDirectionValid()
        {
            float horizontalInput = TSInputManager.Instance.PrimaryMovement.x;
            return (Owner.IsFacingRight && horizontalInput > 0) || (!Owner.IsFacingRight && horizontalInput < 0);
        }

        protected void StartDamageForce()
        {
            if (!IsTargetCharacterValid() || !IsDamageForceValid())
            {
                return;
            }

            bool isFacingRight = DetermineFacingDirection();
            CharacterForceVelocity targetAbilityFV = _damageInfo.TargetCharacter.FindAbility<CharacterForceVelocity>();

            if (targetAbilityFV != null)
            {
                ApplyDamageForce(targetAbilityFV, isFacingRight);
            }
        }

        private bool IsTargetCharacterValid()
        {
            if (_damageInfo.TargetCharacter == null)
            {
                return false;
            }
            return true;
        }

        private bool IsDamageForceValid()
        {
            if (AssetData.DamageForce == FVNames.None)
            {
                return false;
            }
            return true;
        }

        private bool DetermineFacingDirection()
        {
            return position.x < _damageInfo.TargetCharacter.position.x;
        }

        private void ApplyDamageForce(CharacterForceVelocity targetAbilityFV, bool isFacingRight)
        {
            ForceVelocityAsset damageForceAsset = ScriptableDataManager.Instance.FindForceVelocity(AssetData.DamageForce);
            if (!damageForceAsset.IsValid())
            {
                LogWarningForMissingDamageForceAsset();
                return;
            }

            isFacingRight = GetFaceDirectionForDamageForce(damageForceAsset.Data.Direction, targetAbilityFV.position, isFacingRight);
            float forceDuration = DetermineForceDuration(damageForceAsset);
            targetAbilityFV.StartForceVelocity(damageForceAsset, isFacingRight, forceDuration, this);
        }

        private bool GetFaceDirectionForDamageForce(FVDirections direction, Vector3 targetPosition, bool defaultValue)
        {
            switch (direction)
            {
                case FVDirections.AttackerFace:
                    return Owner.IsFacingRight;

                case FVDirections.AttackerFaceReverse:
                    return !Owner.IsFacingRight;

                case FVDirections.ToAttacker:
                    return Owner.position.x > targetPosition.x;

                case FVDirections.FromAttacker:
                    return Owner.position.x < targetPosition.x;

                case FVDirections.ByScaleX:
                    return lossyScale.x > 0;

                default:
                    return defaultValue;
            }
        }

        private float DetermineForceDuration(ForceVelocityAsset damageForceAsset)
        {
            if (!AssetData.UseCustomDurationOfDamageForce)
            {
                return damageForceAsset.Data.ForceDuration;
            }

            CharacterForceVelocity attackerAbilityFV = _damageInfo.Attacker.FindAbility<CharacterForceVelocity>();
            if (attackerAbilityFV != null && attackerAbilityFV.CurrentForceVelocityName == AssetData.AttackForce)
            {
                float forceDuration = attackerAbilityFV.ForceVelocityLastTime;
                LogInfoForCustomForceDuration(forceDuration);
                return forceDuration;
            }

            LogWarningForCustomForceDuration();
            return damageForceAsset.Data.ForceDuration;
        }

        protected void StopForce(FVNames fVName)
        {
            try
            {
                if (fVName == FVNames.None)
                {
                    return;
                }

                CharacterForceVelocity forceVelocity = Owner.FindAbility<CharacterForceVelocity>();
                if (forceVelocity != null)
                {
                    ForceVelocityAsset asset = ScriptableDataManager.Instance.FindForceVelocity(AssetData.AttackForce);
                    if (asset.IsValid())
                    {
                        forceVelocity.StopForceVelocity(this);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("{0}, 공격 독립체에서 Attack Force를 적용 해제할 수 없습니다. {1}", this.GetHierarchyName(), e.Message);
            }
        }

        #region Not Use

        protected void StartAttackForce2()
        {
            if (AssetData.AttackForce == FVNames.None)
            {
                return;
            }

            CharacterForceVelocity attackerAbilityFV = Owner.FindAbility<CharacterForceVelocity>();
            if (attackerAbilityFV == null)
            {
                return;
            }

            ForceVelocityAsset attackForceAsset = ScriptableDataManager.Instance.FindForceVelocity(AssetData.AttackForce);
            if (!attackForceAsset.IsValid())
            {
                Log.Warning(LogTags.ForceVelocity, $"공격({Name.ToLogString()})의 공격 FV({AssetData.AttackForce.ToLogString()}) 에셋을 찾을 수 없습니다.");
                return;
            }

            if (attackForceAsset.Data.IsRequireHorizontalInput)
            {
                if (Owner.IsFacingRight && TSInputManager.Instance.PrimaryMovement.x <= 0)
                {
                    Log.Warning(LogTags.ForceVelocity, $"공격 캐릭터의 방향과 입력 방향이 달라 공격 FV를 적용할 수 없습니다.");
                    return;
                }
                else if (!Owner.IsFacingRight && TSInputManager.Instance.PrimaryMovement.x >= 0)
                {
                    Log.Warning(LogTags.ForceVelocity, $"공격 캐릭터의 방향과 입력 방향이 달라 공격 FV를 적용할 수 없습니다.");
                    return;
                }
            }

            attackerAbilityFV.StartForceVelocity(attackForceAsset, Owner.IsFacingRight, this);
        }

        protected void StartDamageForce2()
        {
            try
            {
                if (_damageInfo.TargetCharacter == null)
                {
                    return;
                }

                if (AssetData.DamageForce == FVNames.None)
                {
                    return;
                }

                bool isFacingRight = true;
                if (position.x > _damageInfo.TargetCharacter.position.x)
                {
                    isFacingRight = false;
                }
                else if (position.x < _damageInfo.TargetCharacter.position.x)
                {
                    isFacingRight = true;
                }

                CharacterForceVelocity targetAbilityFV = _damageInfo.TargetCharacter.FindAbility<CharacterForceVelocity>();
                if (targetAbilityFV != null)
                {
                    ForceVelocityAsset damageForceAsset = ScriptableDataManager.Instance.FindForceVelocity(AssetData.DamageForce);
                    if (!damageForceAsset.IsValid())
                    {
                        Log.Warning(LogTags.ForceVelocity, $"공격({Name.ToLogString()})의 피해 FV({AssetData.DamageForce.ToLogString()}) 에셋을 찾을 수 없습니다.");
                        return;
                    }

                    if (damageForceAsset.Data.Direction == FVDirections.AttackerFace)
                    {
                        isFacingRight = Owner.IsFacingRight;
                    }
                    else if (damageForceAsset.Data.Direction == FVDirections.AttackerFaceReverse)
                    {
                        isFacingRight = !Owner.IsFacingRight;
                    }

                    float forceDuration = damageForceAsset.Data.ForceDuration;
                    if (AssetData.UseCustomDurationOfDamageForce)
                    {
                        CharacterForceVelocity attackerAbilityFV = _damageInfo.Attacker.FindAbility<CharacterForceVelocity>();
                        if (attackerAbilityFV != null && attackerAbilityFV.CurrentForceVelocityName == AssetData.AttackForce)
                        {
                            forceDuration = attackerAbilityFV.ForceVelocityLastTime;

                            Log.Info(LogTags.ForceVelocity, $"공격 FV({AssetData.AttackForce.ToLogString()})의 남은 시간만큼 피해 FV({AssetData.DamageForce.ToLogString()})의 지속시간({forceDuration})을 설정합니다.");
                        }
                        else
                        {
                            Log.Warning(LogTags.ForceVelocity, $"공격 FV({AssetData.AttackForce.ToLogString()})의 남은 시간을 피해 FV({AssetData.DamageForce.ToLogString()})의 지속시간으로 적용할 수 없습니다. 공격자가 공격 FV를 사용하지 않습니다.");
                        }
                    }

                    targetAbilityFV.StartForceVelocity(damageForceAsset, isFacingRight, forceDuration, this);
                }
            }
            catch (Exception e)
            {
                Log.Error("{0}, 공격 독립체에서 Damage Force를 적용할 수 없습니다. {1}", this.GetHierarchyName(), e.Message);
            }
        }

        #endregion Not Use

        //

        #region Log

        private void LogWarningForInvalidDirection()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.ForceVelocity, $"공격 캐릭터의 방향과 입력 방향이 달라 공격 FV를 적용할 수 없습니다.");
            }
        }

        private void LogWarningForMissingDamageForceAsset()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.ForceVelocity, $"공격({Name.ToLogString()})의 피해 FV({AssetData.DamageForce.ToLogString()}) 에셋을 찾을 수 없습니다.");
            }
        }

        private void LogInfoForCustomForceDuration(float forceDuration)
        {
            if (Log.LevelWarning)
            {
                Log.Info(LogTags.ForceVelocity, $"공격 FV({AssetData.AttackForce.ToLogString()})의 남은 시간만큼 피해 FV({AssetData.DamageForce.ToLogString()})의 지속시간({forceDuration})을 설정합니다.");
            }
        }

        private void LogWarningForCustomForceDuration()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.ForceVelocity, $"공격 FV({AssetData.AttackForce.ToLogString()})의 남은 시간을 피해 FV({AssetData.DamageForce.ToLogString()})의 지속시간으로 적용할 수 없습니다. 공격자가 공격 FV를 사용하지 않습니다.");
            }
        }

        #endregion Log
    }
}