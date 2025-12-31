using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TeamSuneat
{
    [RequireComponent(typeof(CollisionController))]
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class PhysicsController : XBehaviour
    {
        public bool CheckAppliedAnyForceVelocity()
        {
            if (FVMover == null)
            {
                return false;
            }

            if (FVMover.Name == FVNames.None)
            {
                return false;
            }

            if (FVMover.Velocity.IsZero())
            {
                return false;
            }

            return true;
        }

        public bool CheckAppliedOwnerForceVelocity()
        {
            if (FVMover == null)
            {
                return false;
            }

            if (FVMover.Data == null)
            {
                return false;
            }

            if (FVMover.Name == FVNames.None)
            {
                return false;
            }

            if (FVMover.Velocity.IsZero())
            {
                return false;
            }

            if (FVMover.Data.Subject != FVSubjects.Owner)
            {
                return false;
            }

            return true;
        }

        public bool CheckAppliedForceVelocityHasInputDirection()
        {
            if (FVMover == null)
            {
                return false;
            }

            if (FVMover.Data == null)
            {
                return false;
            }

            if (FVMover.Data.Direction != FVDirections.DirectionalInput)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity({0})의 방향이 입력값에 따라 달라지지 않으므로 PhysicsController의 점프입력을 무시한다. {1}",
                    FVMover.Name.ToLogString(), FVMover.Data.Direction);

                return false;
            }

            return true;
        }

        public bool CheckAppliedForceVelocityPriorityIsZero()
        {
            if (FVMover == null)
            {
                return false;
            }

            if (FVMover.Data == null)
            {
                return false;
            }

            if (FVMover.Data.Priority == 0)
            {
                Log.Info(LogTags.Physics, "FV의 우선순위가 0인 경우에는 점프에 FV가 삭제된다.");

                return true;
            }

            return false;
        }

        public bool CheckAppliedForceVelocity(FVNames FVName)
        {
            return CheckAppliedForceVelocity(BitConvert.Enum32ToInt(FVName));
        }

        public bool CheckAppliedForceVelocity(int FVTID)
        {
            if (FVMover == null)
            {
                return false;
            }

            if (FVMover.Data == null)
            {
                return false;
            }

            if (FVMover.TID != FVTID)
            {
                return false;
            }

            return CheckAppliedAnyForceVelocity();
        }

        public bool TryForceVelocity(ForceVelocityMover mover)
        {
            if (mover == null)
            {
                Log.Warning(LogTags.Physics, "Failed to set forceVelocity. FVInfo is null.");

                return false;
            }

            if (mover.Name == FVNames.None)
            {
                Log.Warning(LogTags.Physics, "Failed to set forceVelocity. FVInfo`s Name is None.");

                return false;
            }

            if (FVMover != null)
            {
                if (false == FVMover.Velocity.IsZero())
                {
                    if (FVMover.Data.Priority > mover.Data.Priority)
                    {
                        Log.Warning(LogTags.Physics, "{0}, 우선순위가 밀려 FV를 적용하지 않습니다. Current:{1}, New: {2}",
                            Owner.Name.ToLogString(), FVMover.Name.ToLogString(), mover.Name.ToLogString());

                        return false;
                    }
                    else if (FVMover.Name == mover.Name)
                    {
                        if (FVMover.Data.Application == ApplicationTypes.Ignore)
                        {
                            Log.Warning(LogTags.Physics, "{0}, 같은 FV를 적용하지 않습니다. {1}", Owner.Name.ToLogString(), FVMover.Name.ToLogString());

                            return false;
                        }
                    }
                }

                if (FVMover.Duration > 0)
                {
                    StopForceVelocityDuration();

                    StartForceVelocityDuration();
                }
            }
            return true;
        }

        public void SetForceVelocity(ForceVelocityMover info)
        {
            if (FVMover != null)
            {
                ResetForceVelocity();

                CallForceVelocityCompltedEvent();

                StopForceVelocityDuration();

                RemoveForceVelocityBuff();
            }

            FVMover = info.Clone();

            ResetVelocity();

            FVMover.ResetElapsedTime();

            AddForceVelocityBuff();

            if (false == FVMover.Data.UseCustomGravity)
            {
                FVMover.SetGravity(gravity);
            }

            if (FVMover.Duration > 0)
            {
                StopForceVelocityDuration();

                StartForceVelocityDuration();
            }

            SetIgnorePlatform(FVMover.Data.IgnorePlatform);

            Log.Info(LogTags.Physics, "{0}, 강제이동을 설정합니다. {1}, 지속시간: {2}, 값: {3}, 길이: {4}",
                Owner.Name.ToLogString(), FVMover.Name.ToLogString(), FVMover.Duration, FVMover.Velocity, FVMover.Velocity.magnitude);

            MoveForceVelocity();
        }

        private void RemoveForceVelocityBuff()
        {
            if (FVMover == null)
            {
                return;
            }

            if (FVMover.Data == null)
            {
                return;
            }

            if (FVMover.Data.Buff == BuffNames.None)
            {
                return;
            }

            if (Owner == null)
            {
                return;
            }

            if (Owner.buffSystem == null)
            {
                return;
            }

            if (Owner.buffSystem.ContainsKey(FVMover.Data.Buff))
            {
                Owner.buffSystem.Remove(FVMover.Data.Buff);
            }
        }

        private void AddForceVelocityBuff()
        {
            if (FVMover == null)
            {
                return;
            }

            if (FVMover.Data == null)
            {
                return;
            }

            if (FVMover.Data.Buff == BuffNames.None)
            {
                return;
            }

            if (Owner == null)
            {
                return;
            }

            if (Owner.buffSystem == null)
            {
                return;
            }

            Owner.buffSystem.Insert(FVMover.Data.Buff);
        }

        public void ResetForceVelocityExtern()
        {
            ResetForceVelocity();

            CallForceVelocityCompltedEvent();

            RemoveForceVelocityBuff();
        }

        public void ResetForceVelocityExtern(FVNames forceVelocityName)
        {
            if (FVMover == null)
            {
                return;
            }

            if (FVMover.Name == forceVelocityName)
            {
                ResetForceVelocity();

                CallForceVelocityCompltedEvent();

                RemoveForceVelocityBuff();
            }
        }

        protected void ResetForceVelocity()
        {
            if (FVMover != null && FVMover.Name != FVNames.None)
            {
                Log.Info(LogTags.Physics, "{0}, 강제이동을 초기화합니다. {1}", Owner.Name.ToLogString(), FVMover.Name.ToLogString());
            }

            StopForceVelocityDuration();

            CallForceVelocityCompltedEvent();

            RemoveForceVelocityBuff();

            ResetForceVelocityMover();

            ResetIgnorePlatform();
        }

        private void ResetForceVelocityMover()
        {
            if (FVMover == null)
            {
                return;
            }

            FVMover.Reset();
        }

        public void SetForceVelocityCompltedEvent(UnityAction<int> action)
        {
            if (action != null)
            {
                ForceVelocityCompletedEvent = action;
            }
        }

        public void CallForceVelocityCompltedEvent()
        {
            if (ForceVelocityCompletedEvent != null)
            {
                ForceVelocityCompletedEvent(FVMover.TID);

                ForceVelocityCompletedEvent = null;
            }
        }

        private void StartForceVelocityDuration()
        {
            if (_forceCoroutine == null)
            {
                _forceCoroutine = StartXCoroutine(ProcessForceVelocityDuration());
            }
        }

        private void StopForceVelocityDuration()
        {
                StopXCoroutine(ref _forceCoroutine);
        }

        private IEnumerator ProcessForceVelocityDuration()
        {
            yield return new WaitForSeconds(FVMover.Duration);

            ResetForceVelocity();

            CallForceVelocityCompltedEvent();

            RemoveForceVelocityBuff();

            _forceCoroutine = null;
        }

        private void MoveForceVelocity()
        {
            if (FVMover.TryMove())
            {
                FVMover.Move(m_directionalInput);
            }

            FVMover.AddElapsedTime();

            if (FVMover.ElapsedTime > 0.1f)
            {
                CollisionOnMoveForceVelocity();
            }
        }

        private void CollisionOnMoveForceVelocity()
        {
            if (FVMover == null)
            {
                return;
            }

            if (Controller == null)
            {
                return;
            }

            if (Controller.IsCeiling)
            {
                FVMover.OnCelling();
            }

            if (Controller.IsGrounded)
            {
                FVMover.OnGrounded();
            }

            if (Controller.IsCollideX)
            {
                FVMover.OnCollideX();
            }
        }
    }
}