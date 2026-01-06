using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacter : Character
    {
        [SerializeField] private float _moveSpeed = 5f;

        private PlayerInput _input;
        private Transform _modelTransform;

        public override LogTags LogTag => LogTags.Player;

        public override void OnDespawn()
        {
            base.OnDespawn();
            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DESPAWNED);
        }

        public override void Initialize()
        {
            base.Initialize();
            SetupLevel();
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitializePlayerController();
        }

        private void InitializePlayerController()
        {
            // PlayerInput 초기화
            _input = GetComponent<PlayerInput>();
            if (_input == null)
            {
                _input = gameObject.AddComponent<PlayerInput>();
            }

            SetupModel();
        }

        private void SetupModel()
        {
            // CharacterModel 필드가 있으면 우선 사용
            if (CharacterModel != null)
            {
                _modelTransform = CharacterModel.transform;
            }
            else
            {
                // "Model" 자식 오브젝트 찾기
                Transform modelChild = transform.Find("Model");
                if (modelChild != null)
                {
                    _modelTransform = modelChild;
                }
                else
                {
                    Log.Warning(LogTag, "플레이어 캐릭터의 모델 Transform을 찾을 수 없습니다: {0}", this.GetHierarchyName());
                }
            }
        }

        public override void BattleReady()
        {
            base.BattleReady();

            CharacterManager.Instance.RegisterPlayer(this);
            SetupAnimatorLayerWeight();

            IsBattleReady = true;
        }

        //

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();
        }

        //

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (!ActiveSelf || IsBlockInput)
            {
                return;
            }

            // 1. 입력 업데이트 (가장 먼저)
            if (_input != null)
            {
                _input.LogicUpdate();
            }

            // 2. 상태 머신 업데이트 (입력 처리 및 상태 전환)
            if (StateMachine != null)
            {
                StateMachine.LogicUpdate();
            }

            // 3. 점프 입력 감지 (아래 점프만 처리, 일반 점프는 상태 머신에서 처리)
            if (_input != null && _input.IsJumpPressed && _input.IsDownInputPressed)
            {
                // 아래 점프는 상태 머신을 거치지 않고 직접 처리
                if (PhysicsController != null)
                {
                    PhysicsController.RequestDownJump();
                }
            }

            // 4. 점프 키를 떼면 가변 점프 처리 (아래 점프가 아닐 때만)
            if (_input != null && _input.IsJumpReleased && !_input.IsDownInputPressed)
            {
                if (PhysicsController != null)
                {
                    PhysicsController.ReleaseJump();
                }
            }

            // 5. Model 스프라이트 방향 반전
            UpdateModelDirection();
        }

        public override void PhysicsUpdate()
        {
            if (!ActiveSelf)
            {
                return;
            }

            base.PhysicsUpdate();

            // 1. 물리 시스템 업데이트 (바닥 감지, 대시 처리 등)
            if (PhysicsController != null)
            {
                PhysicsController.PhysisUpdate();
            }

            // 2. 상태 머신 FixedUpdate
            if (StateMachine != null)
            {
                StateMachine.PhysisUpdate();
            }

            // 3. 이동 속도 적용 (대시 중일 때는 일반 이동 입력 무시)
            if (PhysicsController != null && _input != null)
            {
                if (!PhysicsController.IsDashing)
                {
                    // 즉각적인 반응: 입력에 바로 속도 적용 (가속/감속 없음)
                    float targetVelocityX = _input.HorizontalInput * _moveSpeed;

                    // PlayerPhysics를 통해 수평 속도 적용 (Y축 속도는 자동으로 유지됨)
                    PhysicsController.ApplyHorizontalVelocity(targetVelocityX);
                }
            }
        }

        private void UpdateModelDirection()
        {
            if (_modelTransform == null || _input == null)
            {
                return;
            }

            // 입력값이 0이 아니면 방향 변경, 0이면 이전 방향 유지
            if (Mathf.Abs(_input.HorizontalInput) > 0.01f)
            {
                Vector3 scale = _modelTransform.localScale;
                scale.x = _input.HorizontalInput > 0 ? 1f : -1f;
                _modelTransform.localScale = scale;
            }
        }

        public override void AddCharacterStats()
        {
            PlayerCharacterStatConfigAsset asset = ScriptableDataManager.Instance.GetPlayerCharacterStatAsset();
            if (asset != null)
            {
                ApplyBaseStats(asset);
                LogInfo("캐릭터 스탯이 스크립터블 데이터에서 적용되었습니다. 캐릭터: {0}", Name);
            }
        }

        private void ApplyBaseStats(PlayerCharacterStatConfigAsset asset)
        {
            if (!asset.IsValid()) return;
            Stat.AddWithSourceInfo(StatNames.Life, asset.BaseLife, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Attack, asset.BaseAttack, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.AttackSpeed, asset.BaseAttackSpeed, this, NameString, "CharacterBase");
            Stat.AddWithSourceInfo(StatNames.Mana, asset.BaseMana, this, NameString, "CharacterBase");
        }

        //

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            CharacterManager.Instance.UnregisterPlayer(this);

            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DEATH);
        }
    }
}