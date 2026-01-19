namespace TeamSuneat
{
    public class MoveSpeedUpdateStrategy : BaseStatUpdateStrategy
    {
        public override void OnAdd(StatNames statName, float value)
        {
            RefreshMoveSpeed(System);
        }

        public override void OnRemove(StatNames statName, float value)
        {
            RefreshMoveSpeed(System);
        }

        private void RefreshMoveSpeed(StatSystem statSystem)
        {
            float moveSpeed = statSystem.FindValueOrDefault(StatNames.MoveSpeed);
            float moveSpeedMulti = statSystem.FindValueOrDefault(StatNames.MoveSpeedMulti);

            // MoveSpeedMulti가 0이거나 없는 경우 기본 MoveSpeed 값 사용
            if (moveSpeedMulti <= 0f)
            {
                moveSpeedMulti = 1f;
            }

            float finalMoveSpeed = moveSpeed * moveSpeedMulti;
            LogStatUpdate(StatNames.MoveSpeed, finalMoveSpeed);

            if (System.Owner.Physics != null)
            {
                System.Owner.Physics.SetMoveSpeed(finalMoveSpeed);
            }
        }
    }
}
