namespace TeamSuneat
{
    public class MonsterCharacter : Character
    {
        public override LogTags LogTag => LogTags.Monster;

        public override void Initialize()
        {
            base.Initialize();

            CharacterManager.Instance.Register(this);
        }

        public override void OnDespawn()
        {
            base.OnDespawn();

            CharacterManager.Instance.Unregister(this);
        }

        //

        public override void LateLogicUpdate()
        {
            if (!ActiveSelf || IsBlockInput)
            {
                return;
            }

            // 상태 머신 업데이트 (입력 처리 및 상태 전환)
            if (StateMachine != null)
            {
                StateMachine.LogicUpdate();
            }

            // 상태 머신 업데이트 이후 초기화
            base.LateLogicUpdate();
        }

        public override void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            base.PhysicsUpdate();

            // 1. 상태 머신 FixedUpdate
            if (StateMachine != null)
            {
                StateMachine.PhysisUpdate();
            }

            // 2. 이동 속도 적용 (대시 중일 때는 일반 이동 입력 무시)
            if (Physics != null)
            {
                if (!Physics.IsDashing)
                {
                    // 공격 중 이동 잠금 확인
                    bool isMovementLocked = CharacterAnimator != null && CharacterAnimator.IsMovementLocked;
                    if (!isMovementLocked)
                    {
                        // 즉각적인 반응: 입력에 바로 속도 적용 (가속/감속 없음)
                        float targetVelocityX = Command.HorizontalInput * Physics.MoveSpeed;

                        // CharacterPhysics를 통해 수평 속도 적용 (Y축 속도는 자동으로 유지됨)
                        Physics.ApplyHorizontalInput(targetVelocityX);
                    }
                    else
                    {
                        // 이동 잠금 중에는 수평 속도를 0으로 설정
                        Physics.ApplyHorizontalInput(0f);
                    }
                }
            }
        }

        public override void AddCharacterStats()
        {
            Stat.AddWithSourceInfo(StatNames.Attack, 1, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.AttackSpeed, 1f, this, NameString, "CharacterBase");
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.Unregister(this);
            transform.SetParent(null);
            CharacterAnimator?.PlayDeathAnimation();

            GlobalEvent<Character>.Send(GlobalEventType.MONSTER_CHARACTER_DEATH, this);
        }
    }
}