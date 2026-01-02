using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Sirenix.OdinInspector;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        [FoldoutGroup("#SkillEntity-Animation", 2)]
        [LabelText("애니메이션 사용")]
        public bool UseAnimation;

        [FoldoutGroup("#SkillEntity-Animation", 2)]
        [LabelText("지상에서 애니메이션 사용 여부")]
        public bool UseAnimationOnGround;

        [FoldoutGroup("#SkillEntity-Animation", 2)]
        [LabelText("공중에서 애니메이션 사용 여부")]
        public bool UseAnimationInAir;

        [FoldoutGroup("#SkillEntity-Animation", 2)]
        [LabelText("애니메이션 재생 완료시 공격 정지")]
        public bool StopAttackOnPlayAnimation;

        private List<SkillAnimationAsset> _animationAssets = new();
        private readonly List<SkillAnimationAsset> _replacedAnimationAssets = new();
        private readonly List<SkillAnimationAsset> _usableAnimationAssets = new();
        private bool _useReplaceAnimation;

        private void LoadAnimationAssets()
        {
            if (UseAnimationOnGround || UseAnimationInAir)
            {
                _animationAssets = ScriptableDataManager.Instance.FindSkillAnimations(Name);
                if (_animationAssets.IsValid())
                {
                    foreach (SkillAnimationAsset asset in _animationAssets)
                    {
                        asset.InitializeCastVFX(Owner);
                        if (!string.IsNullOrEmpty(asset.ReplacedAnimationName) || asset.ReplacedSkillName != SkillNames.None)
                        {
                            _useReplaceAnimation = true;
                        }

                        LogInfoSkillAnimationLoaded(asset.AnimationName);
                    }
                }
                else
                {
                    LogWarningSkillAnimationNotFound();
                }
            }
        }

        private void SetupUsableAnimationAssets()
        {
            if (UseAnimationOnGround || UseAnimationInAir)
            {
                _replacedAnimationAssets.Clear();
                _usableAnimationAssets.Clear();

                for (int i = 0; i < _animationAssets.Count; i++)
                {
                    SkillAnimationAsset animationAsset = _animationAssets[i];
                  
                    if (animationAsset.ReplacedSkillName == SkillNames.None)
                    {
                        LogInfoUsableAnimationSet(animationAsset.AnimationName);
                        _usableAnimationAssets.Add(animationAsset);
                    }
                    else if (animationAsset.CheckReplacedConditions(Owner))
                    {
                        LogInfoReplacedAnimationSet(animationAsset.AnimationName);
                        _replacedAnimationAssets.Add(animationAsset);

                        SetOrderRange();
                        SetSkillOrderToFirst(_skillOrderInAir);
                        SetSkillOrderToFirst(_skillOrderOnGround);
                    }
                }

                if (_replacedAnimationAssets.Count == 0 && _usableAnimationAssets.Count == 0)
                {
                    LogWarningNoUsableAnimationAssets();
                }
            }
        }

        //

        private void RegisterSkillAnimation()
        {
            if (Owner.CharacterAnimator != null)
            {
                Owner.CharacterAnimator.RegisterSkillAnimations(_animationAssets);
            }
        }

        private void UnregisterSkillAnimation()
        {
            if (Owner.CharacterAnimator != null)
            {
                Owner.CharacterAnimator.UnregisterSkillAnimations(_animationAssets);
            }
        }

        //

        private SkillAnimationAsset GetSkillAnimation()
        {
            Order order = GetCurrentOrder();
            List<SkillAnimationAsset> assetsToCheck;

            if (_replacedAnimationAssets.Count > 0)
            {
                assetsToCheck = _replacedAnimationAssets;
            }
            else
            {
                assetsToCheck = _usableAnimationAssets;
            }

            if (assetsToCheck.IsValid())
            {
                for (int i = 0; i < assetsToCheck.Count; i++)
                {
                    SkillAnimationAsset asset = assetsToCheck[i];
                    if (!asset.CheckPassGrounded(Owner.Controller.State.IsGrounded))
                    {
                        continue;
                    }

                    if (asset.Order != order.Current)
                    {
                        continue;
                    }

                    return asset;
                }

                LogWarningSkillAnimationNotFound(order.Current, !Owner.Controller.State.IsGrounded);
            }

            return null;
        }

        private int GetSkillMaxOrder(bool isGrounded)
        {
            int maxOrderIndex = 0;

            if (_replacedAnimationAssets.IsValid(1))
            {
                for (int i = 0; i < _replacedAnimationAssets.Count; i++)
                {
                    if (_replacedAnimationAssets[i].CheckPassGrounded(isGrounded))
                    {
                        maxOrderIndex++;
                    }
                }
            }
            else if (_usableAnimationAssets.IsValid(1))
            {
                for (int i = 0; i < _usableAnimationAssets.Count; i++)
                {
                    if (_usableAnimationAssets[i].CheckPassGrounded(isGrounded))
                    {
                        maxOrderIndex++;
                    }
                }
            }

            return maxOrderIndex;
        }

        //

        private bool TryActivateWithAnimation()
        {
            if ((UseAnimationOnGround && Owner.Controller.State.IsGrounded) || (UseAnimationInAir && !Owner.Controller.State.IsGrounded))
            {
                _castFailType = CastFailTypes.None;
                SkillAnimationAsset skillAnimation = GetSkillAnimation();
                if (skillAnimation == null || string.IsNullOrEmpty(skillAnimation.AnimationName))
                {
                    return false;
                }

                SkillAnimationAsset playingAnimation = Owner.CharacterAnimator.GetPlayingAnimation();
                if (playingAnimation != null)
                {
                    if (playingAnimation.Priority >= skillAnimation.Priority)
                    {
                        LogProgress("현재 기술 애니메이션의 우선순위가 높거나 같아 새로운 기술 애니메이션을 재생할 수 없습니다. 현재: {0}({1}), 새로운: {2}({3})",
                            playingAnimation.AnimationName, playingAnimation.Priority, skillAnimation.AnimationName, skillAnimation.Priority);
                        return false;
                    }
                    else
                    {
                        LogProgress("재생 중인 기술 애니메이션의 우선순위가 재생하려는 기술 애니메이션 우선순위보다 낮습니다. 기술을 활성화할 수 있습니다. 현재: {0}({1}), 새로운: {2}({3})",
                            playingAnimation.AnimationName, playingAnimation.Priority, skillAnimation.AnimationName, skillAnimation.Priority);
                    }
                }
                else
                {
                    LogProgress("재생 중인 기술 애니메이션 정보가 없습니다. 기술을 활성화할 수 있습니다.");
                }
            }

            return true;
        }

        //

        private bool TryPlayAnimation()
        {
            SkillAnimationAsset skillAnimation = GetSkillAnimation();
            if (skillAnimation != null)
            {
                PlaySkillAnimation(skillAnimation);
                return true;
            }
            return false;
        }

        private void PlaySkillAnimation(SkillAnimationAsset skillAnimation)
        {
            if (skillAnimation.IsSequence)
            {
                Owner.CharacterAnimator.PlaySequenceSkillAnimation(skillAnimation, this);
            }
            else
            {
                Owner.CharacterAnimator.PlaySkillAnimation(skillAnimation, this);
            }

            string parameterName = skillAnimation.AnimationName + "Index";
            Owner.Animator.UpdateAnimatorFloatIfExists(parameterName, _skillOrderOnGround.Current);
        }

        //

        public void OnExitSkillAnimationState(string animationName)
        {
            LogInfoSkillAnimationExited(animationName);

            if (_skillOrderOnGround.CheckMax())
            {
                Owner.Skill.ResetLastSkill();

                SetSkillOrderToFirst(_skillOrderOnGround);
                StopDropOrder(_skillOrderOnGround);
            }

            RemoveAnimationBuffs();

            if (StopAttackOnPlayAnimation)
            {
                DeactivateAttackEntities();
            }

            DespawnCastVFX(animationName);
            StartRestSkillAnimation(animationName);
        }

        private void DespawnCastVFX(string animationName)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                LogAnimationNameNotSet();
                return;
            }

            foreach (SkillAnimationAsset asset in _animationAssets)
            {
                if (asset.AnimationName == animationName)
                {
                    asset.DespawnCastVFX();
                    break;
                }
            }
        }

        //

        private bool _isRestSkillAnimation;

        private bool CheckRestSkillAnimation()
        {
            return _isRestSkillAnimation;
        }

        private void StartRestSkillAnimation(string animationName)
        {
            if (!_isRestSkillAnimation && !string.IsNullOrEmpty(animationName))
            {
                foreach (SkillAnimationAsset asset in _animationAssets)
                {
                    if (asset.AnimationName != animationName)
                    {
                        continue;
                    }

                    if (asset.AnimationCooldownOnStop <= 0)
                    {
                        continue;
                    }

                    _ = StartXCoroutine(ProcessRestSkillAnimation(asset.AnimationCooldownOnStop));
                    break;
                }
            }
        }

        private IEnumerator ProcessRestSkillAnimation(float restTime)
        {
            _isRestSkillAnimation = true;

            yield return new WaitForSeconds(restTime);

            _isRestSkillAnimation = false;
        }

        #region Log

        private void LogInfoSkillAnimationLoaded(string animationName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Animation, "[Entity] 기술({0})의 애니메이션 에셋을 불러옵니다. Animation: {1}", Name.ToLogString(), animationName.ToSelectString());
            }
        }

        private void LogWarningSkillAnimationNotFound()
        {
            if (Log.LevelInfo)
            {
                Log.Warning(LogTags.Skill_Animation, "[Entity] 기술({0})의 애니메이션 에셋을 찾을 수 없습니다.", Name.ToLogString());
            }
        }

        private void LogInfoUsableAnimationSet(string animationName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Animation, "[Entity] 기술({0})의 사용 가능한 애니메이션 에셋을 설정합니다. Animation: {1}", Name.ToLogString(), animationName.ToSelectString());
            }
        }

        private void LogInfoReplacedAnimationSet(string animationName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Animation, "[Entity] 기술({0})의 사용 가능한 대체 애니메이션 에셋을 설정합니다. Animation: {1}", Name.ToLogString(), animationName.ToSelectString());
            }
        }

        private void LogWarningNoUsableAnimationAssets()
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Animation, "[Entity] 기술({0})의 사용 가능한 애니메이션 에셋을 설정할 수 없습니다. 기술의 애니메이션 에셋 수: {1}", Name.ToLogString(), _animationAssets.Count);
            }
        }

        private void LogWarningSkillAnimationNotFound(int order, bool isAirborne)
        {
            if (Log.LevelWarning)
            {
                Log.Warning(LogTags.Skill_Animation, "[Entity] 기술({0})의 애니메이션을 찾을 수 없습니다. 순서: {1}, 공중 여부: {2}", Name.ToLogString(), order, isAirborne.ToBoolString());
            }
        }

        private void LogInfoSkillAnimationExited(string animationName)
        {
            if (Log.LevelInfo)
            {
                Log.Info(LogTags.Skill_Animation, "[Entity] 기술({0})의 애니메이션({1})이 종료되었습니다.", Name.ToLogString(), animationName);
            }
        }

        private void LogAnimationNameNotSet()
        {
            if (Log.LevelProgress)
            {
                Log.Progress(LogTags.Skill_Animation, "애니메이션 이름이 설정되지 않아, 기술 애니메이션 시전 VFX를 삭제할 수 없습니다. {0}", Name.ToLogString());
            }
        }

        #endregion Log
    }
}