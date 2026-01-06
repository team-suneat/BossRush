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
            if (IsFacingRight)
            {
                if (facingDirection == FacingDirections.Left)
                {
                    if (TryFlip())
                    {
                        LogProgress("목표 방향을 바라봅니다. Left");
                    }
                }
            }
            else
            {
                if (facingDirection == FacingDirections.Right)
                {
                    if (TryFlip())
                    {
                        LogProgress("목표 방향을 바라봅니다. Right");
                    }
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

        public void UnfaceTarget()
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

        public void MoveToTarget()
        {
            if (Target != null)
            {
                if (IsFacingRight && position.x > Target.position.x)
                {
                    //  if (HorizontalMovement != null)
                    //  {
                    //      HorizontalMovement.SetHorizontalMove(-1);
                    //  }
                    //
                    //  if (Fly != null)
                    //  {
                    //      Fly.SetHorizontalMove(-1);
                    //  }
                }
                else if (!IsFacingRight && position.x < Target.position.x)
                {
                    // if (HorizontalMovement != null)
                    // {
                    //     HorizontalMovement.SetHorizontalMove(1);
                    // }
                    //
                    // if (Fly != null)
                    // {
                    //     Fly.SetHorizontalMove(1);
                    // }
                }
            }
        }

        public bool TryFlip()
        {
            if (CanFlip)
            {
                FlipModel();

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

            LogProgress("캐릭터를 강제로 반전시킵니다. IsFacingRight: {0}", IsFacingRight.ToBoolString());
        }

        public void FlipModel()
        {
            if (CharacterModel != null)
            {
                Vector3 flipValue = new(-1, 1, 1);
                CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, flipValue);
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