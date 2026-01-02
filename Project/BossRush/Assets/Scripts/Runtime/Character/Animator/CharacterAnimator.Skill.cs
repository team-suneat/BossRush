using System.Collections.Generic;
using TeamSuneat.Data;

using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        public int ActiveLayerCount = 1;

        // key : Animation Name
        private readonly Dictionary<string, SkillAnimationAsset> _skillAnimationAssets = new();
        private readonly List<string> _skillAnimationNames = new();
        private int _skillProgress = 0;

        public string _playingSkillAnimationName;

        /// <summary> 재생하는 기술 애니메이션의 이름 </summary>
        public string PlayingSkillAnimationName => _playingSkillAnimationName;

        private SkillEntity PlayingSkillEntity { get; set; }

        // 기술 애니메이션 등록/등록해제 (Register & Unregister)

        public void RegisterSkillAnimations(List<SkillAnimationAsset> skillAnimations)
        {
            if (skillAnimations != null)
            {
                for (int i = 0; i < skillAnimations.Count; i++)
                {
                    RegisterSkillAnimation(skillAnimations[i]);
                }
            }
        }

        public void UnregisterSkillAnimations(List<SkillAnimationAsset> skillAnimations)
        {
            if (skillAnimations != null)
            {
                for (int i = 0; i < skillAnimations.Count; i++)
                {
                    UnregisterSkillAnimation(skillAnimations[i]);
                }
            }
        }

        private void RegisterSkillAnimation(SkillAnimationAsset skillAnimation)
        {
            if (string.IsNullOrEmpty(skillAnimation.AnimationName))
            {
                AnimatorLog.LogSkillAnimationNameNotSet(skillAnimation);
            }
            else
            {
                if (!_skillAnimationNames.Contains(skillAnimation.AnimationName))
                {
                    _skillAnimationNames.Add(skillAnimation.AnimationName);
                }

                if (!_skillAnimationAssets.ContainsKey(skillAnimation.AnimationName))
                {
                    _skillAnimationAssets.Add(skillAnimation.AnimationName, skillAnimation);

                    AnimatorLog.LogSkillAnimationRegistered(skillAnimation);
                }
                else
                {
                    AnimatorLog.LogSkillAnimationAlreadyRegistered(skillAnimation);
                }
            }
        }

        private void UnregisterSkillAnimation(SkillAnimationAsset skillAnimation)
        {
            if (string.IsNullOrEmpty(skillAnimation.AnimationName))
            {
                AnimatorLog.LogSkillAnimationNameNotSet(skillAnimation);
            }
            else
            {
                if (_skillAnimationNames.Contains(skillAnimation.AnimationName))
                {
                    _skillAnimationNames.Remove(skillAnimation.AnimationName);
                }

                if (_skillAnimationAssets.ContainsKey(skillAnimation.AnimationName))
                {
                    _skillAnimationAssets.Remove(skillAnimation.AnimationName);

                    AnimatorLog.LogSkillAnimationUnregistered(skillAnimation);
                }
            }
        }

        // 재생 중인 기술 애니메이션

        public bool CheckPlayingSkillAnimation() => CheckPlayingSkillAnimation(SkillCategories.None);

        public bool CheckPlayingSkillAnimation(SkillCategories ignoreCategory)
        {
            if (!string.IsNullOrEmpty(PlayingSkillAnimationName))
            {
                if (_skillAnimationAssets.ContainsKey(PlayingSkillAnimationName))
                {
                    if (ignoreCategory != SkillCategories.None && PlayingSkillEntity != null)
                    {
                        return PlayingSkillEntity.Category != ignoreCategory;
                    }

                    return true;
                }
            }

            return false;
        }

        public SkillAnimationAsset GetPlayingAnimation()
        {
            if (!string.IsNullOrEmpty(PlayingSkillAnimationName))
            {
                if (_skillAnimationAssets.ContainsKey(PlayingSkillAnimationName))
                {
                    return _skillAnimationAssets[PlayingSkillAnimationName];
                }
            }

            return null;
        }

        private SkillAnimationAsset FindPlayingAnimation(SkillNames skillName)
        {
            foreach (KeyValuePair<string, SkillAnimationAsset> item in _skillAnimationAssets)
            {
                if (item.Value.SkillName == skillName)
                {
                    return item.Value;
                }
            }

            return null;
        }

        // 기술 애니메이션 재생

        public void PlaySkillAnimation(SkillAnimationAsset skillAnimation, SkillEntity skillEntity)
        {
            PlayingSkillEntity = skillEntity;

            // 애니메이션 재생 명령 시점에 이름 설정 (상태 전환 충돌 방지)
            SetPlayingSkillAnimationName(skillAnimation.AnimationName);

            AnimatorLog.LogPlaySkillAnimation(skillAnimation);
            for (int i = 0; i < ActiveLayerCount; i++)
            {
                _animator.Play(skillAnimation.AnimationName, i);
            }

            if (skillAnimation.IsBlockDamageAnimation)
            {
                SetBlockDamageAnimationWhileCast(skillAnimation.SkillName);
            }

            StartForceVelocity(skillAnimation.ForceVelocityName);
        }

        public void PlaySequenceSkillAnimation(SkillAnimationAsset skillAnimation, SkillEntity skillEntity)
        {
            PlayingSkillEntity = skillEntity;

            // 애니메이션 재생 명령 시점에 이름 설정 (상태 전환 충돌 방지)
            SetPlayingSkillAnimationName(skillAnimation.AnimationName);

            // Play Skill Animation

            AnimatorLog.LogPlaySkillAnimation(skillAnimation);
            for (int i = 0; i < ActiveLayerCount; i++)
            {
                _animator.Play(skillAnimation.AnimationName, i);
            }

            if (skillAnimation.IsBlockDamageAnimation)
            {
                SetBlockDamageAnimationWhileCast(skillAnimation.SkillName);
            }

            //

            string parameterName = skillAnimation.SkillName.ToString() + "Progress";
            if (_animator.UpdateAnimatorBoolIfExists(parameterName, true))
            {
                StartForceVelocity(skillAnimation.ForceVelocityName);
            }
            else
            {
                AnimatorLog.LogCannotPlaySequenceSkillAnimation(parameterName);
            }
        }

        // 기술 애니메이션 정지

        public void StopSequenceSkill(SkillNames skillName)
        {
            if (skillName != SkillNames.None)
            {
                SkillAnimationAsset skillAnimation = FindPlayingAnimation(skillName);
                StopSequenceSkillAnimation(skillAnimation);
            }
        }

        public void StopSequenceSkillAnimation(SkillAnimationAsset skillAnimation)
        {
            if (skillAnimation != null)
            {
                string parameterName = skillAnimation.SkillName.ToString() + "Progress";

                if (_animator.UpdateAnimatorBoolIfExists(parameterName, false))
                {
                    AnimatorLog.LogStopSequenceSkillAnimation(parameterName);
                    StopForceVelocity(skillAnimation.ForceVelocityName);
                }
                else
                {
                    AnimatorLog.LogCannotStopSequenceSkillAnimation(parameterName);
                }
            }
        }

        public void StopPlayingSkillAnimation(bool useForceOnExit)
        {
            if (string.IsNullOrEmpty(PlayingSkillAnimationName))
            {
                return;
            }
            if (!_skillAnimationAssets.ContainsKey(PlayingSkillAnimationName))
            {
                return;
            }

            SkillAnimationAsset skillAnimation = _skillAnimationAssets[PlayingSkillAnimationName];
            StopSequenceSkillAnimation(skillAnimation);

            if (useForceOnExit)
            {
                OnAnimatorSkillStateExit(true);
            }
        }

        //

        private void SetPlayingSkillAnimationName(string animationName)
        {
            AnimatorLog.LogSetPlayingSkillAnimationName(animationName);
            _playingSkillAnimationName = animationName;
        }

        private void ResetPlayingSkillAnimationName()
        {
            AnimatorLog.LogResetPlayingSkillAnimationName(PlayingSkillAnimationName);
            _playingSkillAnimationName = string.Empty;
        }

        private void ResetPlayingSkillEntity(SkillNames skillName)
        {
            if (PlayingSkillEntity != null)
            {
                if (PlayingSkillEntity.Name == skillName)
                {
                    PlayingSkillEntity = null;
                }
            }
        }

        // Animator State

        private bool IsSkillState(AnimatorStateInfo stateInfo, bool isEnter)
        {
            for (int i = 0; i < _skillAnimationNames.Count; i++)
            {
                if (string.IsNullOrEmpty(_skillAnimationNames[i]))
                {
                    continue;
                }

                string currentAnimationName = _skillAnimationNames[i];
                if (CheckStateName(stateInfo, currentAnimationName))
                {
                    _skillProgress = 0;
                    if (isEnter)
                    {
                        // 이미 설정된 애니메이션 이름이면 중복 설정하지 않음 (상태 전환 충돌 방지)
                        if (string.IsNullOrEmpty(PlayingSkillAnimationName) || PlayingSkillAnimationName != currentAnimationName)
                        {
                            Log.Progress(LogTags.Skill_Buffer, "재생하려 했던 애니메이션의 이름과 다른 이름의 애니메이션이 재생됩니다. 의도:{0}, 현재:{1}", PlayingSkillAnimationName, currentAnimationName.ToSelectString());
                            SetPlayingSkillAnimationName(currentAnimationName);
                        }
                    }
                    return true;
                }

                if (_skillAnimationAssets.ContainsKey(currentAnimationName))
                {
                    SkillAnimationAsset currentAnimation = _skillAnimationAssets[currentAnimationName];
                    if (currentAnimation.IsSequence)
                    {
                        if (CheckStateName(stateInfo, currentAnimationName + "Progress"))
                        {
                            _skillProgress = 1;
                            return true;
                        }
                    }
                }
                else
                {
                    AnimatorLog.LogFailedToFindCurrentAnimationName(currentAnimationName);
                }

                if (CheckStateName(stateInfo, currentAnimationName + "Complete"))
                {
                    _skillProgress = 2;
                    return true;
                }
            }

            AnimatorLog.LogNotASkillAnimation(PlayingSkillAnimationName, PlayingSkillAnimationName);
            return false;
        }

        private void OnAnimatorSkillStateEnter(AnimatorStateInfo stateInfo)
        {
            if (_skillAnimationAssets.ContainsKey(PlayingSkillAnimationName))
            {
                StartMovementLock(stateInfo.length);
                StartFlipLock(stateInfo.length);
                AnimatorLog.LogSkillAnimationStart(PlayingSkillAnimationName, stateInfo.length);
            }
        }

        protected void OnAnimatorSkillStateExit(bool useForceOnExit = false)
        {
            if (_skillAnimationAssets.ContainsKey(PlayingSkillAnimationName))
            {
                SkillAnimationAsset stoppedSkillAnimationAsset = _skillAnimationAssets[PlayingSkillAnimationName];
                if (stoppedSkillAnimationAsset.IsSequence)
                {
                    if (_skillProgress == 1 || useForceOnExit)
                    {
                        OnStopSkillAnimation(stoppedSkillAnimationAsset);
                    }
                    else
                    {
                        AnimatorLog.LogCannotForceStopSequenceSkillAnimation(PlayingSkillAnimationName, _skillProgress);
                    }
                }
                else
                {
                    OnStopSkillAnimation(stoppedSkillAnimationAsset);
                }
            }
        }

        protected virtual void OnStopSkillAnimation(SkillAnimationAsset skillAnimation)
        {
            UnlockMovement();
            UnlockFlip();

            if (PlayingSkillAnimationName == skillAnimation.AnimationName)
            {
                ResetPlayingSkillAnimationName();
                ResetPlayingSkillEntity(skillAnimation.SkillName);

                if (skillAnimation.IsBlockDamageAnimation)
                {
                    ResetBlockDamageAnimationWhileCast(skillAnimation.SkillName);
                }
            }
            else
            {
                ResetBlockDamageAnimationWhileCast(skillAnimation.SkillName);
            }

            SkillEntity[] entities = _owner.Skill.FindAll(skillAnimation);
            if (entities.IsValidArray())
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    SkillEntity entity = entities[i];
                    AnimatorLog.LogSkillAnimationExit(skillAnimation.AnimationName, entity.Name.ToString());
                    entity.OnExitSkillAnimationState(skillAnimation.AnimationName);
                }
            }

            // 애니메이션 종료 시 버퍼 처리 트리거
            if (_owner.Skill.IsBufferSystemEnabled())
            {
                AnimatorLog.LogProgress("애니메이션 종료 - 버퍼 처리 트리거: {0}", skillAnimation.AnimationName);
                _owner.Skill.TriggerBufferedInputProcessing();
            }
            else
            {
                AnimatorLog.LogProgress("애니메이션 종료 - 버퍼 시스템 비활성화: {0}", skillAnimation.AnimationName);
            }
        }

        public void OnStopSkillAnimationExtern()
        {
            SkillAnimationAsset playingAnimation = GetPlayingAnimation();
            if (playingAnimation == null) { return; }

            AnimatorLog.LogForceStopSkillAnimation(PlayingSkillAnimationName);

            UnlockMovement();
            UnlockFlip();

            ResetPlayingSkillAnimationName();
            if (playingAnimation.IsBlockDamageAnimation)
            {
                ResetBlockDamageAnimationWhileCast(playingAnimation.SkillName);
            }
            if (PlayingSkillEntity != null)
            {
                if (PlayingSkillEntity.Name == playingAnimation.SkillName)
                {
                    PlayingSkillEntity.OnExitSkillAnimationState(PlayingSkillAnimationName);
                    PlayingSkillEntity = null;
                    return;
                }
            }

            SkillEntity entity = _owner.Skill.Find(playingAnimation.SkillName);
            if (entity != null)
            {
                entity.OnExitSkillAnimationState(PlayingSkillAnimationName);
            }
        }

        // Force Velocity

        private void StartForceVelocity(FVNames forceVelocityName)
        {
            if (forceVelocityName == FVNames.None)
            {
                return;
            }

            CharacterForceVelocity attackerAbilityFV = _owner.FindAbility<CharacterForceVelocity>();
            if (attackerAbilityFV == null)
            {
                return;
            }

            ForceVelocityAsset forceVelocityAsset = ScriptableDataManager.Instance.FindForceVelocity(forceVelocityName);
            if (!forceVelocityAsset.IsValid())
            {
                return;
            }

            attackerAbilityFV.StartForceVelocity(forceVelocityAsset, _owner.IsFacingRight, this);
        }

        private void StopForceVelocity(FVNames forceVelocityName)
        {
            if (forceVelocityName == FVNames.None)
            {
                return;
            }

            CharacterForceVelocity attackerAbilityFV = _owner.FindAbility<CharacterForceVelocity>();
            if (attackerAbilityFV == null)
            {
                return;
            }

            attackerAbilityFV.StopForceVelocity(forceVelocityName);
        }
    }
}