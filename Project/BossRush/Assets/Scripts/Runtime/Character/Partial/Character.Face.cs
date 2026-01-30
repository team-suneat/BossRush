using UnityEngine;

namespace TeamSuneat
{
    public partial class Character
    {
        protected void UpdateModelDirection()
        {
            // 패리 상태일 때는 방향 전환 차단
            if (StateMachine != null
                && StateMachine.CurrentState == CharacterState.Parry)
            {
                return;
            }

            // ForceVelocity가 적용 중일 때는 방향 전환 차단
            if (Physics != null && Physics.IsForceVelocity)
            {
                return;
            }

            // 입력값이 0이 아니면 방향 변경, 0이면 이전 방향 유지
            // (입력 레벨에서 이미 threshold 필터링이 적용됨)
            if (Mathf.Abs(Command.HorizontalInput) > 0f)
            {
                FacingDirections facingDirection = Command.HorizontalInput > 0
                    ? FacingDirections.Right
                    : FacingDirections.Left;

                Face(facingDirection);
                SetFacingDirection(facingDirection == FacingDirections.Right ? 1 : -1);
            }
        }

        public void Face(FacingDirections facingDirection)
        {
            bool shouldFaceRight = facingDirection == FacingDirections.Right;
            if (IsFacingRight == shouldFaceRight)
            {
                return;
            }

            if (TryFlip())
            {
                LogProgress("(Face) 목표 방향을 바라봅니다. {0}", facingDirection.ToString());
            }
        }

        public void ForceFace(Vector3 targetPosition)
        {
            bool needFlip = (IsFacingRight && position.x > targetPosition.x)
                || (!IsFacingRight && position.x < targetPosition.x);
            if (!needFlip)
            {
                return;
            }

            LogProgress("(Face) 강제로 목표 방향을 바라봅니다. {0}", IsFacingRight ? "Left" : "Right");
            ForceFlip();
        }

        public void ForceFace(FacingDirections facingDirection)
        {
            bool needFlip = (IsFacingRight && facingDirection == FacingDirections.Left)
                || (!IsFacingRight && facingDirection == FacingDirections.Right);
            if (!needFlip)
            {
                return;
            }

            LogProgress("(Face) 목표 방향을 바라봅니다. {0}", facingDirection.ToString());
            ForceFlip();
        }

        public void FaceToTarget()
        {
            if (Target == null)
            {
                return;
            }

            bool needFlip = (IsFacingRight && position.x > Target.position.x)
                || (!IsFacingRight && position.x < Target.position.x);
            if (!needFlip)
            {
                return;
            }

            if (TryFlip())
            {
                LogProgress("(Face) 목표를 바라봅니다. {0}", IsFacingRight ? "Left" : "Right");
            }
        }

        public bool TryFlip()
        {
            if (Physics != null && Physics.IsForceVelocity)
            {
                LogWarning("(Face) ForceVelocity 적용 중에는 캐릭터를 반전시킬 수 없습니다.");
                return false;
            }

            if (!CanFlip)
            {
                LogWarning("(Face) 캐릭터를 반전시킬 수 없습니다. 반전을 허용하지 않습니다.");
                return false;
            }

            FlipModel();
            SyncFacingDirection();
            LogProgress("(Face) 캐릭터를 반전시킵니다. IsFacingRight: {0}", IsFacingRight.ToBoolString());
            return true;
        }

        public void ForceFlip()
        {
            FlipModel();
            SyncFacingDirection();
            LogProgress("(Face) 캐릭터를 강제로 반전시킵니다. IsFacingRight: {0}", IsFacingRight.ToBoolString());
        }

        private void SyncFacingDirection()
        {
            SetFacingDirection(IsFacingRight ? 1 : -1);
        }

        public void FlipModel()
        {
            if (CharacterModel == null)
            {
                return;
            }

            Vector3 flipValue = new(-1, 1, 1);
            CharacterModel.transform.localScale = Vector3.Scale(CharacterModel.transform.localScale, flipValue);
            SetFacingDirection(IsFacingRight ? 1 : -1);
        }

        public void LockFlip()
        {
            CanFlip = false;
            LogProgress("(Face) 캐릭터의 반전을 허용하지 않습니다.");
        }

        public void UnlockFlip()
        {
            CanFlip = true;
            LogProgress("(Face) 캐릭터의 반전을 허용합니다.");
        }
    }
}