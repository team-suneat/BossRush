using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterAnimator : XBehaviour, IAnimatorStateMachine
    {
        //-------------------------------------------------------------------------------------------

        [Title("#Character Animator")]
        public bool IgnoreFlipOnAttacking;

        /// <summary> 공격 중 피격 애니메이션을 재생할 수 없도록 금지합니다. </summary>
        protected bool IsBlockingDamageAnimationWhileAttack;

        /// <summary> 시전 중 피격 애니메이션을 재생할 수 없도록 금지합니다. </summary>
        protected List<SkillNames> BlockingDamageAnimationWhileCast = new();

        /// <summary> 잡기 중 피격 애니메이션을 재생할 수 없도록 금지합니다. </summary>
        protected bool IsBlockingDamageAnimationWhileGrab;

        protected CharacterAnimatorLog AnimatorLog;

        public bool IsAttacking { get; set; }

        public bool IsDashing { get; set; }

        public bool IsGrabbing { get; set; }

        public bool IsDamaging { get; set; }

        public bool IsConsumingPotion { get; set; }

        public bool IsBlinking { get; set; }

        public bool IsTurning { get; set; }

        protected bool IsBlockingDamageAnimationWhileCast => BlockingDamageAnimationWhileCast.Count > 0;

        public bool IsBlockDeathAnimation { get; set; }

        // Component

        [SerializeField] protected Character _owner;
        [SerializeField] protected CharacterHorizontalMovement _abilityMovement;
        [SerializeField] protected CharacterFly _abilityFly;
        [SerializeField] protected Animator _animator;

        public delegate void OnStateEnterDelegate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);
        public delegate void OnStateExitDelegate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);

        private OnStateEnterDelegate OnStateEnter;
        private OnStateExitDelegate OnStateExit;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _animator = GetComponent<Animator>();
            _owner = this.FindFirstParentComponent<Character>();
            _abilityMovement = this.FindFirstParentComponent<CharacterHorizontalMovement>();
            _abilityFly = this.FindFirstParentComponent<CharacterFly>();
        }

        private void Awake()
        {
            _animator ??= GetComponent<Animator>();
            _owner ??= this.FindFirstParentComponent<Character>();
            _abilityMovement ??= this.FindFirstParentComponent<CharacterHorizontalMovement>();
            _abilityFly ??= this.FindFirstParentComponent<CharacterFly>();

            AnimatorLog = new CharacterAnimatorLog(_owner);
        }

        public virtual void Initialize()
        {
            IsAttacking = false;
            IsDashing = false;
            IsGrabbing = false;
            IsDamaging = false;
            IsConsumingPotion = false;
            IsBlinking = false;
            IsTurning = false;
            IsBlockDeathAnimation = false;
            _animator.UpdateAnimatorBoolIfExists("IsSuicide", false);

            InitializeAnimatorParameters();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNED_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_ATTACKING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_CASTING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DASING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_GRABBING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_IS_CONSUMING_POTION_PARAMETER_ID, false, AnimatorParameters);
            _animator.UpdateAnimatorBool(ANIMATOR_BLINK_PARAMETER_ID, false, AnimatorParameters);

            ResetPlayingSkillAnimationName();
            ClearBlockDamageAnimationWhileCast();
        }

        public void SetAttackSpeed(float attackSpeed)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_ATTACK_SPEED_PARAMETER_ID, attackSpeed, AnimatorParameters);
        }

        public void SetMoveSpeed(float moveSpeed)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_MOVE_SPEED_PARAMETER_ID, moveSpeed, AnimatorParameters);
        }

        public virtual void SetDamageTriggerIndex(int index)
        {
            _damageTriggerIndex = index;
        }

        public virtual void OnEquipItem(ItemNames itemName)
        {
        }

        #region Parameter

        public void SetPhaseParameter(float parameter)
        {
            _animator.UpdateAnimatorFloat(ANIMATOR_BOSS_PHASE_PARAMETER_ID, parameter, AnimatorParameters);
        }

        public void SetDamageTypeParameter(bool isPowerfulAttack)
        {
            if (_owner.AssetData.SuperArmor)
            {
                return;
            }

            if (isPowerfulAttack)
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 1, AnimatorParameters);
                _damageTypeIndex = 1;
            }
            else if (!_owner.PhysicsController.Controller.IsGrounded)
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 1, AnimatorParameters);
                _damageTypeIndex = 1;
            }
            else
            {
                _animator.UpdateAnimatorFloat(ANIMATOR_DAMAGE_TYPE_PARAMETER_ID, 0, AnimatorParameters);
                _damageTypeIndex = 0;
            }
        }

        #endregion Parameter

        #region Play

        public void PlaySpawnAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_SPAWN_PARAMETER_ID, AnimatorParameters);
        }

        public void PlaySpecialSpawnAnimation(string parameterName)
        {
            _animator.UpdateAnimatorTriggerIfExists(parameterName);
        }

        public virtual void PlaySpecialIdleAnimation()
        {
            AnimatorLog.LogInfo("{0} 애니메이션 파마메터를 설정합니다. {1}", ANIMATOR_SPECIAL_IDLE_PARAMETER_NAME, true.ToBoolString());

            _animator.UpdateAnimatorBool(ANIMATOR_SPECIAL_IDLE_PARAMETER_ID, true, AnimatorParameters);
        }

        public virtual bool PlayDamageAnimation(HitmarkAssetData damageAssetData)
        {
            return false;
        }

        public virtual void PlayDeathAnimation()
        {
            if (!IsBlockDeathAnimation)
            {
                _animator.UpdateAnimatorTrigger(ANIMATOR_DEATH_PARAMETER_ID, AnimatorParameters);
            }
        }

        public void PlayInteractAnimation()
        {
            AnimatorLog.LogInfo("{0} 애니메이션 파마메터를 설정합니다. {1}", ANIMATOR_INTERACT_PARAMETER_NAME, true.ToBoolString());

            _animator.UpdateAnimatorBool(ANIMATOR_INTERACT_PARAMETER_ID, true, AnimatorParameters);
        }

        public void PlayBlinkAnimation()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_BLINK_PARAMETER_ID, true, AnimatorParameters);
        }

        public void PlayTeleportAnimation()
        {
            _animator.UpdateAnimatorTrigger(ANIMATOR_TELEPORT_PARAMETER_ID, AnimatorParameters);
        }

        public void PlayTurnAnimation()
        {
            if (IsTurning)
            {
                AnimatorLog.LogInfo("이미 돌아서고 있는 중에는 돌아설 수 없습니다.");
                return;
            }

            if (IsAttacking)
            {
                AnimatorLog.LogInfo($"공격 중에는 돌아설 수 없습니다.");
                return;
            }

            if (CheckPlayingSkillAnimation())
            {
                AnimatorLog.LogInfo("기술({0}) 시전 중에는 돌아설 수 없습니다.", PlayingSkillAnimationName);
                return;
            }

            if (IsDashing || IsGrabbing || IsDamaging || IsConsumingPotion || IsBlinking)
            {
                AnimatorLog.LogInfo($"특정 애니메이션 중에는 돌아설 수 없습니다. 돌진 중:{IsDashing}, 잡힘 중:{IsGrabbing}, 피격 중:{IsDamaging}, 물약 사용 중:{IsConsumingPotion}, 순간이동 중:{IsBlinking}");
                return;
            }

            _animator.UpdateAnimatorTrigger(ANIMATOR_TURN_PARAMETER_ID, AnimatorParameters);
        }

        #endregion Play

        #region Stop

        public void StopInteractAnimation()
        {
            AnimatorLog.LogInfo("{0} 애니메이션 파마메터를 초기화합니다. {1}", ANIMATOR_INTERACT_PARAMETER_ID, false.ToBoolString());

            _animator.UpdateAnimatorBool(ANIMATOR_INTERACT_PARAMETER_ID, false, AnimatorParameters);
        }

        public void StopBlinkAnimation()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_BLINK_PARAMETER_ID, false, AnimatorParameters);
        }

        #endregion Stop

        public virtual void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (CheckStateName(stateInfo, "Spawn"))
            {
                OnAnimatorSpawnStateEnter();
            }
            else if (CheckStateNames(stateInfo, "Damage", "DamageGround"))
            {
                OnAnimatorDamageStateEnter();
            }
            else if (CheckStateName(stateInfo, "Stun"))
            {
                AnimatorLog.LogInfo("기절 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            }
            else if (CheckStateName(stateInfo, "Freeze"))
            {
                AnimatorLog.LogInfo("빙결 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            }
            else if (CheckStateName(stateInfo, "Grabbed"))
            {
                AnimatorLog.LogInfo("잡아당겨짐 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            }
            else if (CheckStateNames(stateInfo, "Grab", "GrabProgress", "GrabCompelete"))
            {
                OnAnimatorGrabStateEnter();
            }
            else if (CheckStateName(stateInfo, "Potion"))
            {
                OnAnimatorConsumePotionStateEnter();
            }
            else if (CheckStateName(stateInfo, "DashStart"))
            {
                OnAnimatorDashStateEnter();
            }
            else if (CheckStateName(stateInfo, "Blink"))
            {
                OnAnimatorBlinkStateEnter();
            }
            else if (CheckStateName(stateInfo, "Turn"))
            {
                OnAnimatorTurnStateEnter();
            }
            else if (CheckStateName(stateInfo, "Teleport"))
            {
                OnAnimatorTeleportStateEnter();
            }
            else if (CheckStateNames(stateInfo, "Phase2", "Phase3", "Phase4"))
            {
                LockMovement();
            }
            else if (IsAttackState(stateInfo, true))
            {
                OnAnimatorAttackStateEnter();
            }
            else if (IsSkillState(stateInfo, true))
            {
                OnAnimatorSkillStateEnter(stateInfo);
            }

            OnStateEnter?.Invoke(animator, stateInfo, layerIndex);
        }

        public virtual void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (CheckStateName(stateInfo, "Spawn"))
            {
                OnAnimatorSpawnStateExit();
            }
            else if (CheckStateName(stateInfo, "SpecialIdle"))
            {
                OnAnimatorSpecialIdleStateExit();
            }
            else if (CheckStateNames(stateInfo, "Damage", "DamageGround"))
            {
                OnAnimatorDamageStateExit();
            }
            else if (CheckStateNames(stateInfo, "Grab", "GrabProgress", "GrabCompelete"))
            {
                OnAnimatorGrabStateExit();
            }
            else if (CheckStateName(stateInfo, "Potion"))
            {
                OnAnimatorConsumePotionStateExit();
            }
            else if (CheckStateName(stateInfo, "DashEnd"))
            {
                OnAnimatorDashStateExit();
            }
            else if (CheckStateName(stateInfo, "Blink"))
            {
                OnAnimatorBlinkStateExit();
            }
            else if (CheckStateName(stateInfo, "Turn"))
            {
                OnAnimatorTurnStateExit();
            }
            else if (CheckStateName(stateInfo, "Teleport"))
            {
                OnAnimatorTeleportStateExit();
            }
            else if (CheckStateNames(stateInfo, "Phase2", "Phase3", "Phase4"))
            {
                UnlockMovement();
            }
            else if (IsAttackState(stateInfo, false))
            {
                OnAnimatorAttackStateExit();
            }
            else if (IsSkillState(stateInfo, false))
            {
                OnAnimatorSkillStateExit();
            }

            OnStateExit?.Invoke(animator, stateInfo, layerIndex);
        }

        protected bool CheckStateName(AnimatorStateInfo stateInfo, string animationName)
        {
            if (stateInfo.IsName(animationName))
            {
                return true;
            }

            return false;
        }

        protected bool CheckStateNames(AnimatorStateInfo stateInfo, params string[] animationNames)
        {
            if (animationNames != null)
            {
                for (int i = 0; i < animationNames.Length; i++)
                {
                    string animationName = animationNames[i];
                    if (stateInfo.IsName(animationName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region On Animator State Delegate

        public void RegisterOnStateEnterDelegate(OnStateEnterDelegate action)
        {
            if (OnStateEnter != null)
            {
                System.Delegate[] delegateArray = OnStateEnter.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    Log.Warning(LogTags.Animation, "중복된 애니메이션 상태 입장 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnStateEnter += action;
        }

        public void RegisterOnStateExitDelegate(OnStateExitDelegate action)
        {
            if (OnStateExit != null)
            {
                System.Delegate[] delegateArray = OnStateExit.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    Log.Warning(LogTags.Animation, "중복된 애니메이션 상태 퇴장 이벤트를 등록할 수 없습니다. {0}", action.Method);
                    return;
                }
            }

            OnStateExit += action;
        }

        public void UnregisterOnStateEnterDelegate(OnStateEnterDelegate action)
        {
            if (OnStateEnter != null)
            {
                System.Delegate[] delegateArray = OnStateEnter.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnStateEnter -= action;
                    return;
                }
            }

            Log.Warning(LogTags.Animation, "등록되지않은 애니메이션 상태 입장 이벤트를 등록 해제할 수 없습니다. {0}", action.Method);
        }

        public void UnregisterOnStateExitDelegate(OnStateExitDelegate action)
        {
            if (OnStateExit != null)
            {
                System.Delegate[] delegateArray = OnStateExit.GetInvocationList();
                if (delegateArray.Contains(action))
                {
                    OnStateExit -= action;
                    return;
                }
            }

            Log.Warning(LogTags.Animation, "등록되지않은 애니메이션 상태 퇴장 이벤트를 등록 해제할 수 없습니다. {0}", action.Method);
        }

        #endregion On Animator State Delegate

        #region On Animator State Changed

        protected virtual void OnAnimatorSpawnStateEnter()
        {
            if (_animator != null)
            {
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, true, AnimatorParameters);
            }
        }

        protected virtual void OnAnimatorSpawnStateExit()
        {
            if (_animator != null)
            {
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNING_PARAMETER_ID, false, AnimatorParameters);
                _animator.UpdateAnimatorBool(ANIMATOR_IS_SPAWNED_PARAMETER_ID, true, AnimatorParameters);
            }
        }

        private void OnAnimatorSpecialIdleStateExit()
        {
            if (_animator != null)
            {
                _animator.UpdateAnimatorBool(ANIMATOR_SPECIAL_IDLE_PARAMETER_ID, false, AnimatorParameters);
            }
        }

        private void OnAnimatorGrabStateEnter()
        {
            AnimatorLog.LogInfo("잡기 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);

            StartGrabbing();
        }

        private void OnAnimatorGrabStateExit()
        {
            StopGrabbing();

            ResetBlockDamageAnimationWhileGrab();
        }

        private void OnAnimatorConsumePotionStateEnter()
        {
            AnimatorLog.LogInfo("물약 사용 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);

            StartConsumingPotion();
        }

        private void OnAnimatorConsumePotionStateExit()
        {
            StopConsumingPotion();
        }

        protected virtual void OnAnimatorDamageStateEnter()
        {
            AnimatorLog.LogInfo("피격 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            LockMovement();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, true, AnimatorParameters);
            IsDamaging = true;
        }

        protected virtual void OnAnimatorDamageStateExit()
        {
            UnlockMovement();

            _animator.UpdateAnimatorBool(ANIMATOR_IS_DAMAGING_PARAMETER_ID, false, AnimatorParameters);
            IsDamaging = false;
        }

        protected void OnAnimatorDashStateEnter()
        {
            AnimatorLog.LogInfo("대시 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);

            LockMovement();
            StartDashing();
        }

        protected void OnAnimatorDashStateExit()
        {
            UnlockMovement();
            StopDashing();
        }

        protected void OnAnimatorBlinkStateEnter()
        {
            AnimatorLog.LogInfo("순간이동 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            LockMovement();
            StartBlink();
        }

        protected void OnAnimatorBlinkStateExit()
        {
            UnlockMovement();
            StopBlink();
        }

        protected void OnAnimatorTurnStateEnter()
        {
            AnimatorLog.LogInfo("뒤돌기 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
            LockMovement();
            StartTurn();
        }

        protected void OnAnimatorTurnStateExit()
        {
            UnlockMovement();
            StopTurn();

            _owner.TryFlip();
        }

        protected virtual void OnAnimatorTeleportStateEnter()
        {
            AnimatorLog.LogInfo("순간이동 상태의 애니메이션에 진입했습니다. PlayingSkillAnimationName: {0}", PlayingSkillAnimationName);
        }

        protected virtual void OnAnimatorTeleportStateExit()
        {
        }

        #endregion On Animator State Changed

        #region Start & Stop

        private void StartDashing()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DASING_PARAMETER_ID, true, AnimatorParameters);
            IsDashing = true;
        }

        public void StopDashing()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_DASING_PARAMETER_ID, false, AnimatorParameters);
            IsDashing = false;
        }

        private void StartGrabbing()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_GRABBING_PARAMETER_ID, true, AnimatorParameters);

            IsGrabbing = true;

            TSCharacterGrab grabAbility = _owner.FindAbility<TSCharacterGrab>();
            if (grabAbility != null)
            {
                grabAbility.ResizeGrabbingCollider();
            }
        }

        private void StopGrabbing()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_GRABBING_PARAMETER_ID, false, AnimatorParameters);

            IsGrabbing = false;

            TSCharacterGrab grabAbility = _owner.FindAbility<TSCharacterGrab>();
            if (grabAbility != null)
            {
                grabAbility.ResizeDefaultCollider();
            }
        }

        private void StartConsumingPotion()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_CONSUMING_POTION_PARAMETER_ID, true, AnimatorParameters);
            IsConsumingPotion = true;
        }

        private void StopConsumingPotion()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_CONSUMING_POTION_PARAMETER_ID, false, AnimatorParameters);
            IsConsumingPotion = false;
        }

        private void StartBlink()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_BLINK_PARAMETER_ID, true, AnimatorParameters);
            IsBlinking = true;
        }

        private void StopBlink()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_BLINK_PARAMETER_ID, false, AnimatorParameters);
            IsBlinking = false;
        }

        private void StartTurn()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_TURNING_PARAMETER_ID, true, AnimatorParameters);
            IsTurning = true;
        }

        private void StopTurn()
        {
            _animator.UpdateAnimatorBool(ANIMATOR_IS_TURNING_PARAMETER_ID, false, AnimatorParameters);
            IsTurning = false;
        }

        #endregion Start & Stop

        private void OnDrawGizmos()
        {
            if (IsDamaging)
            {
                TSGizmoEx.DrawText("Damaging", position);
            }
        }
    }
}