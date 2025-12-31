using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CollisionController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class PhysicsController : XBehaviour
    {
        public void Move()
        {
            if (FVMover != null)
            {
                if (CheckAppliedAnyForceVelocity())
                {
                    MoveForceVelocity();

                    return;
                }
                else
                {
                    ResetForceVelocity();

                    CallForceVelocityCompltedEvent();

                    RemoveForceVelocityBuff();
                }
            }

            if (FlyingOwner)
            {
                if (_isFalling)
                {
                    CalculateNotFlyingOwnerVelocity();
                }
                else
                {
                    CalculateFlyingOwnerVelocity();
                }
            }
            else
            {
                CalculateNotFlyingOwnerVelocity();
            }

            MoveVelocity();

            CollisionOnMoveVelocity();

            if (_prevIsGrounded != Controller.IsGrounded)
            {
                _prevIsGrounded = Controller.IsGrounded;

                RefreshAbilityOnGrounded();
            }

            RefreshAbilityOnMoveVelocity();

            RefreshHigherHeight();
        }

        public void OnDisable()
        {
            ResetForceVelocity();

            CallForceVelocityCompltedEvent();

            RemoveForceVelocityBuff();
        }

        public void OnSpawn()
        {
            FVMover = new ForceVelocityMover();

            SetDirectionalInput(0, 0);

            ResetVelocity();

            ResetForceVelocityExtern();

            ResetTargetToFollow();
        }

        public void OnDie()
        {
            if (Owner.DataCharacter.IsFallingOnDie)
            {
                if (false == Owner.DataCharacter.FallingDelayTime.IsZero())
                {
                    CoroutineNextTimer(Owner.DataCharacter.FallingDelayTime, () => { _isFalling = true; });
                }
                else
                {
                    _isFalling = true;
                }
            }

            SetDirectionalInput(0, 0);

            ResetVelocity();

            ResetForceVelocityExtern();

            ResetTargetToFollow();
        }

        public void OnDespawn()
        {
            ResetVelocity();

            ResetCustomJumpVelocity();

            ResetAdditionalVelocity();

            ResetForceVelocity();
        }

        public void ApplyMoveDataToJumpInfo()
        {
            JumpMinHeight = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_CHARACTER_JUMP_MIN_HEIGHT);
            JumpMaxHeight = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_CHARACTER_JUMP_MAX_HEIGHT);
            TimeToJumpApex = JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_TIME_TO_JUMP_APEX);

            gravity = -(2 * JumpMaxHeight) / Mathf.Pow(TimeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity) * TimeToJumpApex;
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * JumpMinHeight);
        }

        public void ApplyCharacterMoveData(float newMoveSpeed)
        {
            DefaultMoveSpeed = newMoveSpeed;
        }

        public void ResetVelocity()
        {
            Velocity = Vector3.zero;

            if (Owner != null)
            {
                Log.Info(LogTags.Physics, "{0}, 이동값을 초기화합니다.", Owner.Name.ToLogString());
            }
        }

        private void MoveVelocity()
        {
            if (IsLockVelcotiyX)
            {
                Velocity.x = 0;
            }

            if (IsLockVelcotiyY)
            {
                Velocity.y = 0;
            }

            Vector2 moveAmount = (Vector2)Velocity * Time.fixedDeltaTime;

            Controller.Move(moveAmount);
        }

        private void CalculateFlyingOwnerVelocity()
        {
            if (Following != null)
            {
                CalculateFollowVelocityInAir(Following.position);
            }
            else if (false == FollowPosition.IsZero())
            {
                CalculateFollowVelocityInAir(FollowPosition);
            }
            else
            {
                int directionalXToInt = System.Math.Sign(m_directionalInput.x);
                int directionalYToInt = System.Math.Sign(m_directionalInput.y);

                CalculateVelocityInAir(directionalXToInt, directionalYToInt);
            }
        }

        private void CalculateNotFlyingOwnerVelocity()
        {
            if (Following != null)
            {
                CalculateFollowVelocityInGround(Following.position);

                HandleWallSliding();
            }
            else if (false == FollowPosition.IsZero())
            {
                CalculateFollowVelocityInGround(FollowPosition);

                HandleWallSliding();
            }
            else
            {
                int directionalXToInt = System.Math.Sign(m_directionalInput.x);

                CalculateVelocityInGround(directionalXToInt);

                HandleWallSliding();

                CalculateAdditionalVelocity();
            }
        }

        private void CollisionOnMoveVelocity()
        {
            if (Controller.IsCollideY)
            {
                if (Controller.collisions.slidingDownMaxSlope)
                {
                    Velocity.y += Controller.collisions.slopeNormal.y * -gravity * Time.fixedDeltaTime;
                }
                else
                {
                    Velocity.y = 0;
                }
            }
        }

        private void RefreshAbilityOnGrounded()
        {
            if (Owner != null && Owner.abilitySystem != null)
            {
                if (Controller.IsGrounded)
                {
                    Owner.abilitySystem.ResetJumpCount();

                    Owner.abilitySystem.ResetAirAttackCount();
                }
            }
        }

        private void RefreshAbilityOnMoveVelocity()
        {
            if (Owner != null)
            {
                if (Owner.abilitySystem != null)
                {
                    if (Owner.abilitySystem.IsSliding)
                    {
                        Owner.abilitySystem.AddJumpCount();

                        Owner.abilitySystem.AddAirAttackCount();
                    }
                }
            }
        }

        protected virtual void HandleWallSliding()
        {
            if (Owner.abilitySystem == null)
            {
                return;
            }

            if (false == Owner.abilitySystem.UseWallSliding)
            {
                return;
            }

            Owner.abilitySystem.ResetWallSlide();

            bool canWallSlide = false;

            if (false == Controller.IsGrounded && Velocity.y < 0)
            {
                if (Controller.IsLeftCollision && Velocity.x < 0)
                {
                    canWallSlide = Owner.abilitySystem.TryWallSlide();
                }
                if (Controller.IsRightCollision && Velocity.x > 0)
                {
                    canWallSlide = Owner.abilitySystem.TryWallSlide();
                }
            }

            if (canWallSlide)
            {
                int directionalXToInt = System.Math.Sign(m_directionalInput.x);
                wallDirX = (Controller.collisions.left) ? -1 : 1;

                if (Velocity.y < -WallSlideSpeedMax)
                {
                    Velocity.y = -WallSlideSpeedMax;
                }

                if (timeToWallUnstick > 0)
                {
                    m_velocityXSmoothing = 0;
                    Velocity.x = 0;

                    if (directionalXToInt != wallDirX && directionalXToInt != 0)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        timeToWallUnstick = WallStickTime;
                    }
                }
                else
                {
                    timeToWallUnstick = WallStickTime;
                }
            }
        }

        private void CalculateVelocityInGround(int directionalX)
        {
            if (directionalX != 0)
            {
                float targetVelocityX = directionalX * MoveSpeed;

                m_smoothTime = (Controller.IsGrounded) ? AccelerationTimeGrounded : AccelerationTimeAirborne;

                Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref m_velocityXSmoothing, m_smoothTime);
            }
            else
            {
                Velocity.x = 0f;
            }

            Velocity.y += gravity * Time.fixedDeltaTime;
            Velocity.y = Mathf.Clamp(Velocity.y, -MaxVelocityY, MaxVelocityY);
        }

        public void SetAdditionalVelocity(Vector3 additionalVelocity)
        {
            AdditionalVelocity = additionalVelocity;
        }

        public void ResetAdditionalVelocity()
        {
            AdditionalVelocity = Vector3.zero;
        }

        private void CalculateAdditionalVelocity()
        {
            if (AdditionalVelocity != Vector3.zero)
            {
                Velocity += AdditionalVelocity;
            }
        }

        private void RefreshHigherHeight()
        {
            if (Controller.IsGrounded)
            {
                if (_higherHeight != float.MinValue)
                {
                    float distance = ValueTypeEx.GetDifference(_higherHeight, transform.position.y);

                    if (distance > JsonDataManager.FindGameConstantValue(GameConstantNames.DEFAULT_CONDITION_DISTANCE_LANDING_FX))
                    {
                        SpawnLandingEffect();
                    }

                    _higherHeight = float.MinValue;
                }
            }
            else if (_higherHeight < transform.position.y)
            {
                _higherHeight = transform.position.y;
            }
        }

        private void SpawnLandingEffect()
        {
            if (false == string.IsNullOrEmpty(LandingVFX))
            {
                if (LandingPoint != null)
                {
                    ResourcesManager.SpawnPrefab(LandingVFX, LandingPoint.position);
                }
                else
                {
                    ResourcesManager.SpawnPrefab(LandingVFX, position);
                }
            }
        }

        private void CalculateVelocityInAir(int directionalX, int directionalY)
        {
            if (directionalX != 0)
            {
                float targetVelocityX = directionalX * MoveSpeed;
                m_smoothTime = (Controller.IsGrounded) ? AccelerationTimeGrounded : AccelerationTimeAirborne;
                Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref m_velocityXSmoothing, m_smoothTime);
            }
            else
            {
                Velocity.x = 0f;
            }

            if (directionalY != 0)
            {
                float targetVelocityY = directionalY * MoveSpeed;
                Velocity.y = Mathf.Clamp(targetVelocityY, -MaxVelocityY, MaxVelocityY);
            }
            else
            {
                Velocity.y = 0f;
            }
        }
    }
}