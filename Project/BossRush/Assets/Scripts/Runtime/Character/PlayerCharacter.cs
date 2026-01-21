using System.Collections.Generic;
using TeamSuneat.CameraSystem.Core;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public class PlayerCharacter : Character
    {
        private PlayerInput _input;
        private Transform _modelTransform;

        public CharmSystem Charm { get; set; }

        public override LogTags LogTag => LogTags.Player;

        protected override void Awake()
        {
            base.Awake();
            Charm = GetComponentInChildren<CharmSystem>();
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DESPAWNED);
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

        protected override void ApplyCharacterCharms()
        {
            // 게임 데이터에서 플레이어 부적 정보 가져오기
            var profile = GameApp.GetSelectedProfile();
            if (profile == null)
            {
                Log.Warning(LogTag, "플레이어 프로필을 찾을 수 없습니다.");
                return;
            }

            var charmData = profile.Charm;
            if (charmData == null)
            {
                Log.Warning(LogTag, "플레이어 부적 데이터를 찾을 수 없습니다.");
                return;
            }

            var slotCharmNames = charmData.SlotCharmNames;
            if (slotCharmNames == null || slotCharmNames.Count == 0)
            {
                LogInfo("적용할 플레이어 부적이 없습니다.");
                return;
            }

            // 부적 효과 적용
            if (Charm == null)
            {
                Log.Warning(LogTag, "부적 시스템을 찾을 수 없습니다.");
                return;
            }

            foreach (CharmName charmName in slotCharmNames)
            {
                Charm.AddCharm(charmName);
            }
        }

        public override void BattleReady()
        {
            base.BattleReady();

            CharacterManager.Instance.RegisterPlayer(this);
            SetupAnimatorLayerWeight();
            IsBattleReady = true;

            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY);

            CameraManager.Instance.SetFollowTarget(transform);
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
                Physics?.RequestDownJump();
            }

            // 4. 점프 키를 떼면 가변 점프 처리 (아래 점프가 아닐 때만)
            if (_input != null && _input.IsJumpReleased && !_input.IsDownInputPressed)
            {
                Physics?.ReleaseJump();
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

            // 1. 상태 머신 FixedUpdate
            if (StateMachine != null)
            {
                StateMachine.PhysisUpdate();
            }

            // 2. 이동 속도 적용 (대시 중일 때는 일반 이동 입력 무시)
            if (Physics != null)
            {
                // ForceVelocity가 적용 중일 때는 입력 무시
                if (!Physics.IsDashing && !Physics.IsForceVelocity)
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

  


        //

        protected override void OnDamage(DamageResult damageResult)
        {
            base.OnDamage(damageResult);

            ApplySlowMotion();
            ApplyVibration();
            ApplyCameraShake(damageResult.AttackPosition);
        }

        private void ApplySlowMotion()
        {
            GameTimeManager.Instance?.StartSlowMotion(0.1f, 0.01f);
        }

        private void ApplyVibration()
        {
            if (GameSetting.Instance?.Play?.Vibration != true)
            {
                return;
            }

            Rewired.Player inputPlayer = TSInputManager.Instance?.InputPlayer;
            if (inputPlayer == null)
            {
                return;
            }

            inputPlayer.SetVibration(0, 0.6f, 0.15f);
            inputPlayer.SetVibration(1, 0.6f, 0.15f);
        }

        private void ApplyCameraShake(Vector3 attackPosition)
        {
            if (CameraManager.Instance == null)
            {
                return;
            }

            // 공격자 위치를 기준으로 방향 결정
            Vector3 defenderPosition = position;
            Vector3 direction = (attackPosition - defenderPosition).normalized;

            // X축 방향에 따라 GameImpulseType 결정
            GameImpulseType shakeType = direction.x > 0f
                ? GameImpulseType.Horizontal_Right
                : GameImpulseType.Horizontal_Left;

            CameraImpulseAsset asset = ScriptableDataManager.Instance?.GetCameraImpulseAsset(shakeType);
            if (asset == null)
            {
                return;
            }

            CameraManager.Instance.ShakeAtPosition(position, asset);
        }

        protected override void OnDeath(DamageResult damageResult)
        {
            base.OnDeath(damageResult);

            // 모든 부적 효과 해제
            Charm?.ClearAll();

            CharacterManager.Instance.UnregisterPlayer(this);

            GlobalEvent.Send(GlobalEventType.PLAYER_CHARACTER_DEATH);
        }
    }
}