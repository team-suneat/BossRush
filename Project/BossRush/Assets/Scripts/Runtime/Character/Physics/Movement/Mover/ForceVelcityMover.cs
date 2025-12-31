using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class ForceVelocityMover
    {
        public int TID;
        public FVNames Name;
        public ForceVelocityAssetData Data;

        protected Character caster;
        protected Character owner;
        protected Projectile ownerProjectile;
        protected int buffTID;
        protected float gravity;
        protected Vector3 direction;
        protected Vector3 velocity;
        protected Vector3 targetPosition;
        protected bool facingRight;
        protected float duration;
        protected float elapsedTime;

        protected const float kMaxVelocityY = 55;

        public ForceVelocityMover()
        {
            Reset();
        }

        public void Reset()
        {
            TID = 0;
            Name = FVNames.None;
            Data = new ForceVelocityAssetData();
            caster = null;
            owner = null;
            ownerProjectile = null;
            buffTID = 0;
            elapsedTime = 0;
            gravity = 0;
            direction = Vector3.zero;
            velocity = Vector3.zero;
            targetPosition = Vector3.zero;
            facingRight = false;
            duration = 0;
        }

        public void SetUp(FVNames name)
        {
            ForceVelocityAsset asset = ScriptableDataManager.Instance.FindForceVelocity(name);

            if (asset != null && asset.Data != null)
            {
                TID = asset.TID;
                Data = asset.Data;
                Name = asset.Data.Name;
                duration = asset.Data.Duration;
                gravity = asset.Data.Gravity;
                velocity = asset.Data.ForceVelocity;

                ApplyStat();

                InitializeDirection();
            }
        }

        public void SetDirection(Vector3 direction)
        {
            this.direction = VectorEx.Normalize(direction);
        }

        private void ResetVelocity()
        {
            velocity = Vector3.zero;
        }

        public void SetCaster(Character character)
        {
            caster = character;
        }

        public void SetOwner(Character character)
        {
            owner = character;
        }

        public void SetProjectile(Projectile projectile)
        {
            ownerProjectile = projectile;
        }

        public void SetFVBuff(BuffNames buff)
        {
            buffTID = buff.ToInt();
        }

        public void SetGravity(float gravity)
        {
            this.gravity = gravity;
        }

        public void SetVelocityX(float x)
        {
            velocity.x = x;
        }

        public void SetVelocityY(float y)
        {
            velocity.y = y;
        }

        public void ResetElapsedTime()
        {
            elapsedTime = 0;
        }

        public void AddElapsedTime()
        {
            elapsedTime += Time.fixedDeltaTime;
        }

        public bool CheckDelayTime()
        {
            if (false == Mathf.Approximately(0f, Data.Delay))
            {
                return elapsedTime < Data.Delay;
            }
            else
            {
                return false;
            }
        }

        public bool CheckTargetPosition()
        {
            if (Data.Direction == FVDirections.ToTargetPosition)
            {
                if (false == targetPosition.IsZero())
                {
                    float distance = Vector3.Distance(owner.position, targetPosition);

                    return distance < 0.5f;
                }
            }

            return false;
        }

        public bool TryMove()
        {
            return false == CheckDelayTime();
        }

        public void Move(Vector2 directionalInput)
        {
            if (CheckTargetPosition())
            {
                ResetVelocity();
            }
            else
            {
                SetDirection(directionalInput.x);

                ApplyAcceleration();

                if (owner.Controller.Controller.IsGrounded)
                {
                    ApplyFriction();
                }
                else if (owner.IsFlying)
                {
                    ApplyAirResistInAir();
                }
                else
                {
                    ApplyAirResistInGround();
                }

                ApplyGravity();

                SetVelocityY(Mathf.Clamp(Velocity.y, -kMaxVelocityY, kMaxVelocityY));

                owner.Controller.Controller.Move(Velocity * Time.fixedDeltaTime, false);
            }
        }

        private void ApplyStat()
        {
            if (owner == null)
            {
                return;
            }

            for (int i = 0; i < Data.Stats.Length; i++)
            {
                if (Data.Stats[i] == StatNames.None)
                {
                    continue;
                }

                if (false == owner.Stat.ContainsKey(Data.Stats[i]))
                {
                    continue;
                }

                CharacterStat stat = owner.Stat.Find(Data.Stats[i]);

                if (stat.IsDefaultValue)
                {
                    continue;
                }

                if (Data.Stats[i] == StatNames.SkillFVDuration)
                {
                    duration = Data.Duration + stat.Value;
                }
                else if (Data.Stats[i] == StatNames.SkillFVSpeed)
                {
                    velocity *= stat.Value;
                }
            }
        }

        private void InitializeDirection()
        {
            if (Data.Direction == FVDirections.Velocity)
            {
                SetDirectionVelocity();
            }
            else if (Data.Direction == FVDirections.Face)
            {
                SetDirectionFace();
            }
            else if (Data.Direction == FVDirections.ToAttacker)
            {
                SetDirectionToAttacker();
            }
            else if (Data.Direction == FVDirections.ToTarget)
            {
                SetDirectionToTarget();
            }
            else if (Data.Direction == FVDirections.ToTargetPosition)
            {
                SetDirectionToTargetPosition();
            }
            else if (Data.Direction == FVDirections.ToDirection)
            {
                SetDirectionToDirection();
            }
            else if (Data.Direction == FVDirections.DirectionalX || Data.Direction == FVDirections.DirectionalInput)
            {
                SetDirectionDirectional(0);
            }
        }

        public void SetDirection(float directionalX)
        {
            if (Data.Direction == FVDirections.Velocity)
            {
                SetDirectionVelocity();
            }
            else if (Data.Direction == FVDirections.Face)
            {
                SetDirectionFace();
            }
            else if (Data.Direction == FVDirections.ToAttacker)
            {
                SetDirectionToAttacker();
            }
            else if (Data.Direction == FVDirections.DirectionalX || Data.Direction == FVDirections.DirectionalInput)
            {
                SetDirectionDirectional(directionalX);
            }
        }

        private void SetDirectionVelocity()
        {
            facingRight = Data.ForceVelocity.x > 0;

            velocity = VectorEx.ApplyFacingRight(velocity, facingRight);
        }

        private void SetDirectionFace()
        {
            if (ownerProjectile != null)
            {
                facingRight = ownerProjectile.FacingRightAtLaunch;
            }
            else if (caster != null)
            {
                facingRight = caster.FacingRight;
            }
            else if (owner != null)
            {
                facingRight = owner.FacingRight;
            }

            if (Data.ForceVelocity.x < 0)
            {
                velocity = VectorEx.ApplyFacingRight(velocity, !facingRight);
            }
            else
            {
                velocity = VectorEx.ApplyFacingRight(velocity, facingRight);
            }
        }

        private void SetDirectionToAttacker()
        {
            if (caster != null && owner != null)
            {
                direction = caster.position - owner.position;
                direction.Normalize();

                float x = System.Math.Sign(direction.x);
                float y = System.Math.Sign(direction.y);

                facingRight = direction.x > 0;

                velocity = new Vector3(Mathf.Abs(velocity.x) * x, Mathf.Abs(velocity.y) * y);
            }
            else
            {
                Log.Error("캐스터와 오너가 설정되어있지 않다. {0}", Name.ToLogString());
            }
        }

        private void SetDirectionToTarget()
        {
            if (owner != null && owner.TargetVital != null)
            {
                direction = owner.TargetVital.position - owner.position;
                direction.Normalize();

                facingRight = direction.x > 0;
                velocity = new Vector3(velocity.x * direction.x, velocity.y * direction.y);
            }
            else
            {
                Log.Error("캐스터와 오너가 설정되어있지 않다. {0}", Name.ToLogString());
            }
        }

        private void SetDirectionToTargetPosition()
        {
            if (owner != null && owner.TargetVital != null)
            {
                targetPosition = owner.TargetVital.position;

                direction = targetPosition - owner.position;

                direction.Normalize();

                facingRight = direction.x > 0;

                velocity = new Vector3(velocity.x * direction.x, velocity.y * direction.y);
            }
            else
            {
                Log.Error("캐스터와 오너가 설정되어있지 않다. {0}", Name.ToLogString());
            }
        }

        private void SetDirectionToDirection()
        {
            if (caster != null)
            {
                facingRight = direction.x > 0;

                velocity = new Vector3(velocity.x * direction.x, velocity.y * direction.y);

                caster.ForceSetFlip(facingRight);
            }
            else
            {
                Log.Error("캐스터가 설정되어있지 않다. {0}", Name.ToLogString());
            }
        }

        private void SetDirectionDirectional(float directionalX)
        {
            if (owner != null)
            {
                if (false == Mathf.Approximately(directionalX, 0f))
                {
                    facingRight = directionalX > 0;
                }
                else
                {
                    facingRight = owner.FacingRight;
                }

                velocity = VectorEx.ApplyFacingRight(velocity, facingRight);
                owner.ForceSetFlip(facingRight);
            }
            else
            {
                Log.Error("오너가 설정되어있지 않다. {0}", Name.ToLogString());
            }
        }

        public void ApplyGravity()
        {
            if (velocity.IsZero())
            {
                return;
            }

            if (Data.UseGravity)
            {
                if (false == gravity.IsZero())
                {
                    velocity += Vector3.up * gravity * Time.fixedDeltaTime;
                }
            }
        }

        public void ApplyAcceleration()
        {
            if (velocity.IsZero())
            {
                return;
            }

            float velocityX = velocity.x;
            float velocityY = velocity.y;

            if (Data.UseAccelerationX)
            {
                float accelerationX = Data.AccelerationX * (elapsedTime / duration);
                if (false == facingRight)
                {
                    accelerationX *= -1;
                }

                velocityX = velocity.x + (accelerationX * Time.fixedDeltaTime);
            }

            if (Data.UseAccelerationY)
            {
                float accelerationY = Data.AccelerationY * (elapsedTime / duration);
                if (velocity.y < 0)
                {
                    accelerationY *= -1;
                }

                velocityY = velocity.y + accelerationY * Time.fixedDeltaTime;
            }

            velocity = new Vector3(velocityX, velocityY, velocity.z);
        }

        public void ApplyFriction()
        {
            if (velocity.IsZero())
            {
                return;
            }

            if (false == Data.UseForceFriction)
            {
                return;
            }

            float velocityX = velocity.x;

            float friction = Data.Friction;

            if (velocity.x > 0)
            {
                friction *= -1;
            }

            if (Mathf.Abs(velocity.x) - Mathf.Abs(friction * Time.fixedDeltaTime) <= 0)
            {
                velocityX = 0;
            }
            else
            {
                velocityX = velocity.x + friction * Time.fixedDeltaTime;
            }

            velocity = new Vector3(velocityX, velocity.y);
        }

        public void ApplyAirResistInGround()
        {
            if (velocity.IsZero())
            {
                return;
            }

            if (Data.UseAirResist)
            {
                float velocityX = velocity.x;

                float airResist = Data.AirResist;

                if (velocity.x > 0)
                {
                    airResist *= -1;
                }

                if (Mathf.Abs(velocity.x) - Mathf.Abs(airResist * Time.fixedDeltaTime) <= 0)
                {
                    velocityX = 0;
                }
                else
                {
                    velocityX = velocity.x + airResist * Time.fixedDeltaTime;
                }

                velocity = new Vector3(velocityX, velocity.y);
            }
        }

        public void ApplyAirResistInAir()
        {
            if (velocity.IsZero())
            {
                return;
            }

            if (Data.UseAirResist)
            {
                float velocityX = velocity.x;
                float velocityY = velocity.y;

                float airResistX = Data.AirResistFlyingMonster;
                float airResistY = Data.AirResistFlyingMonster;

                if (velocity.x > 0)
                {
                    airResistX = -Mathf.Abs(Data.AirResistFlyingMonster);
                }

                if (Mathf.Abs(velocity.x) - Mathf.Abs(airResistX * Time.fixedDeltaTime) <= 0)
                {
                    velocityX = 0;
                }
                else
                {
                    velocityX = velocity.x + airResistX * Time.fixedDeltaTime;
                }

                if (velocity.y > 0)
                {
                    airResistY = -Mathf.Abs(Data.AirResistFlyingMonster);
                }

                if (Mathf.Abs(velocity.y) - Mathf.Abs(airResistY * Time.fixedDeltaTime) <= 0)
                {
                    velocityY = 0;
                }
                else
                {
                    velocityY = velocity.y + airResistY * Time.fixedDeltaTime;
                }

                velocity = new Vector3(velocityX, velocityY);
            }
        }

        public void OnCollision()
        {
            if (owner == null)
            {
                return;
            }

            if (owner.collisionController.IsCeiling)
            {
                OnCelling();
            }

            if (owner.collisionController.IsGrounded)
            {
                OnGrounded();
            }

            if (owner.collisionController.IsCollideX)
            {
                OnCollideX();
            }
        }

        public void OnCelling()
        {
            velocity = new Vector3(velocity.x, gravity * Time.fixedDeltaTime);
        }

        public void OnGrounded()
        {
            if (Data.StopXOnHitGround)
            {
                velocity = new Vector3(0f, velocity.y);
            }

            if (Data.StopYOnHitGround)
            {
                velocity = new Vector3(velocity.x, 0f);
            }
        }

        public void OnCollideX()
        {
            if (Data.StopXOnHitWall)
            {
                velocity = new Vector3(0f, velocity.y);

                if (owner != null)
                {
                    Log.Info(LogTags.Physics, "{0}이 {1}로 인해 벽에 부딪혔습니다.", owner.Name.ToLogString(), Name.ToLogString());

                    GlobalEvent<SID, FVNames>.Send(GlobalEventType.CHARACTER_COLLIDE_WITH_WALL, owner.SID, Name);
                }
            }
        }

        public ForceVelocityMover Clone()
        {
            return new ForceVelocityMover()
            {
                TID = TID,
                Name = Name,
                Data = Data,
                caster = caster,
                owner = owner,
                ownerProjectile = ownerProjectile,
                buffTID = buffTID,
                elapsedTime = elapsedTime,
                gravity = gravity,
                direction = direction,
                velocity = velocity,
                targetPosition = targetPosition,
                facingRight = facingRight,
                duration = duration,
            };
        }

        public float ElapsedTime => elapsedTime;

        public float Duration => duration;

        public Vector3 Velocity => velocity;

        public bool FacingRight => facingRight;
    }
}