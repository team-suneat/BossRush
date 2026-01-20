using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        public void Face(Vector3 targetPosition)
        {
            if (IsFacingRight)
            {
                if (position.x > targetPosition.x)
                {
                    LogProgress("목표 방향을 바라봅니다. Left");
                    TryFlip();
                }
            }
            else
            {
                if (position.x < targetPosition.x)
                {
                    LogProgress("목표 방향을 바라봅니다. Right");
                    TryFlip();
                }
            }
        }

        public void Face(FacingDirections facingDirection)
        {
            // 모델의 실제 방향을 기준으로 판단 (IsFacingRight는 모델의 localScale.x 기반)
            bool shouldFaceRight = facingDirection == FacingDirections.Right;
            bool currentlyFacingRight = IsFacingRight;
            
            if (currentlyFacingRight != shouldFaceRight)
            {
                if (TryFlip())
                {
                    LogProgress("목표 방향을 바라봅니다. {0}", facingDirection.ToString());
                }
            }
        }

        public void ForceFace(Vector3 targetPosition)
        {
            if (IsFacingRight)
            {
                if (position.x > targetPosition.x)
                {
                    LogProgress("강제로 목표 방향을 바라봅니다. Left");

                    ForceFlip();
                }
            }
            else
            {
                if (position.x < targetPosition.x)
                {
                    LogProgress("강제로 목표 방향을 바라봅니다. Right");

                    ForceFlip();
                }
            }
        }

        public void ForceFace(FacingDirections facingDirection)
        {
            if (IsFacingRight)
            {
                if (facingDirection == FacingDirections.Left)
                {
                    LogProgress("목표 방향을 바라봅니다. Left");

                    ForceFlip();
                }
            }
            else
            {
                if (facingDirection == FacingDirections.Right)
                {
                    LogProgress("목표 방향을 바라봅니다. Right");

                    ForceFlip();
                }
            }
        }

        public void FaceToTarget()
        {
            if (Target == null)
            {
                return;
            }

            if (IsFacingRight)
            {
                if (position.x > Target.position.x)
                {
                    LogProgress("목표를 바라봅니다. Left");

                    TryFlip();
                }
            }
            else
            {
                if (position.x < Target.position.x)
                {
                    LogProgress("목표를 바라봅니다. Right");

                    TryFlip();
                }
            }
        }

        public void FaceOppositeTarget()
        {
            if (Target != null)
            {
                if (IsFacingRight)
                {
                    if (position.x < Target.position.x)
                    {
                        LogProgress("목표를 바라보지 않습니다. Right");

                        TryFlip();
                    }
                }
                else
                {
                    if (position.x > Target.position.x)
                    {
                        LogProgress("목표를 바라보지 않습니다. Left");

                        TryFlip();
                    }
                }
            }
        }

        public void CompelFaceTarget()
        {
            if (Target == null)
            {
                return;
            }

            if (IsFacingRight)
            {
                if (position.x > Target.position.x)
                {
                    LogProgress("목표를 강제로 바라봅니다. Left");

                    ForceFlip();
                }
            }
            else
            {
                if (position.x < Target.position.x)
                {
                    LogProgress("목표를 강제로 바라봅니다. Right");

                    ForceFlip();
                }
            }
        }

        public bool TryFlip()
        {
            // ForceVelocity가 적용 중일 때는 방향 전환 차단
            if (Physics != null && Physics.IsForceVelocity)
            {
                LogWarning("ForceVelocity 적용 중에는 캐릭터를 반전시킬 수 없습니다.");
                return false;
            }

            if (CanFlip)
            {
                FlipModel();
                SyncFacingDirection();

                LogProgress("캐릭터를 반전시킵니다. IsFacingRight: {0}", IsFacingRight.ToBoolString());

                return true;
            }
            else
            {
                LogWarning("캐릭터를 반전시킬 수 없습니다. 반전을 허용하지 않습니다.");

                return false;
            }
        }

        public void ForceFlip()
        {
            FlipModel();
            SyncFacingDirection();

            LogProgress("캐릭터를 강제로 반전시킵니다. IsFacingRight: {0}", IsFacingRight.ToBoolString());
        }

        private void SyncFacingDirection()
        {
            // FacingDirection을 IsFacingRight와 동기화
            // FlipModel()에서 이미 FacingDirection이 업데이트되므로 여기서는 불필요
            // 하지만 방어적 프로그래밍을 위해 유지
            SetFacingDirection(IsFacingRight ? 1 : -1);
        }

        public void FlipModel()
        {
            if (CharacterModel != null)
            {
                Vector3 flipValue = new(-1, 1, 1);
                CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, flipValue);
                
                // FlipModel() 호출 시 FacingDirection을 모델의 실제 방향과 동기화
                // IsFacingRight는 모델의 localScale.x를 기반으로 계산되므로 순환 참조 없음
                SetFacingDirection(IsFacingRight ? 1 : -1);
            }
        }

        protected void ForceSpawnDirection()
        {
            if (DirectionOnSpawn == SpawnFacingDirections.Left)
            {
                Face(FacingDirections.Left);
            }
            else if (DirectionOnSpawn == SpawnFacingDirections.Right)
            {
                Face(FacingDirections.Right);
            }
        }

        public void LockFlip()
        {
            CanFlip = false;

            LogProgress("캐릭터의 반전을 허용하지 않습니다.");
        }

        public void UnlockFlip()
        {
            CanFlip = true;

            LogProgress("캐릭터의 반전을 허용합니다.");
        }
    }
}