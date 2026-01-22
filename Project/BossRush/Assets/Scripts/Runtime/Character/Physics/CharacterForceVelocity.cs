using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterForceVelocity : MonoBehaviour
    {
        private struct ForceVelocityItem
        {
            public ForceVelocityAssetData AssetData;
            public object Source;
            public int Priority;
            public Coroutine Coroutine;
            public bool IsFacingRight;
            public float ElapsedTime;
            public Vector2 AppliedForce;
        }

        private CharacterPhysicsCore _physics;
        private Vital _vital;
        private Character _character;
        private CharacterAnimator _characterAnimator;
        private ForceVelocityItem? _currentForceVelocity = null;
        private float? _originalGravityScale;

        public bool IsProcessing
        {
            get
            {
                return _currentForceVelocity.HasValue && _currentForceVelocity.Value.Coroutine != null;
            }
        }

        public bool IsProcessingForName(FVNames name)
        {
            return _currentForceVelocity.HasValue &&
                   _currentForceVelocity.Value.AssetData != null &&
                   _currentForceVelocity.Value.AssetData.Name == name &&
                   _currentForceVelocity.Value.Coroutine != null;
        }

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            _vital = GetComponentInChildren<Vital>();
            _character = GetComponentInParent<Character>();
            _characterAnimator = _character != null ? _character.CharacterAnimator : null;
        }

        private void StopCurrentForceVelocity()
        {
            if (!_currentForceVelocity.HasValue)
            {
                return;
            }

            if (_currentForceVelocity.Value.Coroutine != null)
            {
                StopCoroutine(_currentForceVelocity.Value.Coroutine);
            }

            RestoreGravity();
            UpdateAnimatorParameters(Vector2.zero);
            _currentForceVelocity = null;
        }

        public void StartForceVelocity(ForceVelocityAssetData assetData, bool isFacingRight, object source = null)
        {
            if (assetData == null)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity를 시작할 수 없습니다. assetData가 null입니다. {0}", this.GetHierarchyPath());
                return;
            }
            if (_physics == null)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity를 시작할 수 없습니다. CharacterPhysicsCore가 null입니다. {0}", this.GetHierarchyPath());
                return;
            }

            // 기존 ForceVelocity가 있다면 완전히 취소
            if (_currentForceVelocity.HasValue)
            {
                Log.Info(LogTags.Physics, "기존 ForceVelocity를 취소합니다. {0}, {1}",
                    this.GetHierarchyPath(), _currentForceVelocity.Value.AssetData.Name.ToLogString());
                StopCurrentForceVelocity();
            }

            // 새로운 ForceVelocity 설정
            ForceVelocityItem newItem = new()
            {
                AssetData = assetData,
                Source = source,
                Priority = assetData.Priority,
                Coroutine = null,
                IsFacingRight = isFacingRight,
                ElapsedTime = 0f,
                AppliedForce = Vector2.zero
            };

            _currentForceVelocity = newItem;

            // 새로운 FV 시작 전 Rigidbody 속도 초기화 (이전 FV의 잔여 속도 제거)
            _physics.ApplyVelocity(Vector2.zero);

            Log.Info(LogTags.Physics, "ForceVelocity를 시작합니다. {0}, {1}", this.GetHierarchyPath(), assetData.Name.ToLogString());

            // 코루틴 시작
            Coroutine newCoroutine = StartCoroutine(ProcessForceVelocity());
            newItem.Coroutine = newCoroutine;
            _currentForceVelocity = newItem;
        }

        public void StopForceVelocity(object source = null, FVNames? name = null)
        {
            if (!_currentForceVelocity.HasValue)
            {
                return;
            }

            // source와 name이 모두 null이면 모든 FV 중지
            if (source == null && name == null)
            {
                StopCurrentForceVelocity();
                return;
            }

            // 현재 FV가 조건에 맞는지 확인
            bool shouldStop = false;
            var currentItem = _currentForceVelocity.Value;

            if (source != null && currentItem.Source == source)
            {
                shouldStop = true;
            }
            else if (name.HasValue && currentItem.AssetData != null && currentItem.AssetData.Name == name.Value)
            {
                shouldStop = true;
            }

            if (shouldStop)
            {
                string fvName = currentItem.AssetData != null ? currentItem.AssetData.Name.ToLogString() : "Unknown";
                Log.Info(LogTags.Physics, "ForceVelocity를 중지합니다. {0}, {1}", this.GetHierarchyPath(), fvName);
                StopCurrentForceVelocity();
            }
        }

        public void AbilityTick()
        {
            if (_physics == null)
            {
                return;
            }

            if (_vital != null && !_vital.IsAlive)
            {
                if (IsProcessing)
                {
                    Log.Info(LogTags.Physics, "캐릭터가 사망하여 모든 ForceVelocity를 중지합니다. {0}", this.GetHierarchyPath());
                    StopForceVelocity();
                }
                return;
            }
        }

        private IEnumerator ProcessForceVelocity()
        {
            if (!_currentForceVelocity.HasValue)
            {
                yield break;
            }

            ForceVelocityItem item = _currentForceVelocity.Value;
            ForceVelocityAssetData assetData = item.AssetData;

            if (assetData == null || _physics == null || _physics.Rigidbody == null)
            {
                yield break;
            }

            if (assetData.Delay > 0f)
            {
                Log.Info(LogTags.Physics, "ForceVelocity Delay 대기 중. {0}, {1}, Delay: {2}초", this.GetHierarchyPath(), assetData.Name.ToLogString(), assetData.Delay);
                yield return new WaitForSeconds(assetData.Delay);
            }

            // 현재 FV가 여전히 유효한지 확인
            if (!_currentForceVelocity.HasValue || _currentForceVelocity.Value.AssetData != assetData)
            {
                yield break;
            }

            ForceVelocityState state = InitializeForceVelocity(assetData);
            object itemSource = item.Source;

            while (state.ElapsedTime < assetData.Duration)
            {
                // 현재 FV가 여전히 유효한지 확인
                if (!_currentForceVelocity.HasValue || _currentForceVelocity.Value.AssetData != assetData)
                {
                    yield break;
                }
                item = _currentForceVelocity.Value;

                if (!ValidateForceVelocityComponents(assetData))
                {
                    yield break;
                }

                if (!ValidateCharacterAlive(assetData))
                {
                    yield break;
                }

                state.ElapsedTime += Time.fixedDeltaTime;
                UpdateForceVelocityFrame(assetData, ref state);

                Vector2 appliedForce = ApplyForceVelocity(assetData, state.CurrentVelocity, state.IsFirstFrame);
                state.IsFirstFrame = false;

                // 현재 FV 업데이트
                item.ElapsedTime = state.ElapsedTime;
                item.AppliedForce = appliedForce;
                _currentForceVelocity = item;

                UpdateAnimatorParameters(appliedForce);

                yield return new WaitForFixedUpdate();
            }

            Log.Info(LogTags.Physics, "ForceVelocity 적용 완료. {0}, {1}, 경과 시간: {2}초", this.GetHierarchyPath(), assetData.Name.ToLogString(), state.ElapsedTime);
            FinalizeForceVelocity(assetData, itemSource);
        }

        private struct ForceVelocityState
        {
            public Vector2 CurrentVelocity;
            public Vector2 Acceleration;
            public int LastFacingDirection;
            public float ElapsedTime;
            public bool IsFirstFrame;
        }

        private ForceVelocityState InitializeForceVelocity(ForceVelocityAssetData assetData)
        {
            Vector2 forceVelocity = assetData.ForceVelocity;
            int facingDirection = _physics != null ? _physics.FacingDirection : 1;
            int lastFacingDirection = facingDirection;

            if (assetData.DirectionalType == FVDirectionalType.Facing)
            {
                forceVelocity.x *= facingDirection;
            }
            else if (assetData.DirectionalType == FVDirectionalType.Reverse)
            {
                forceVelocity.x *= -facingDirection;
            }

            ApplyGravitySettings(assetData);

            Vector2 acceleration = assetData.Acceleration;
            if (assetData.DirectionalType == FVDirectionalType.Facing)
            {
                acceleration.x *= lastFacingDirection;
            }
            else if (assetData.DirectionalType == FVDirectionalType.Reverse)
            {
                acceleration.x *= -lastFacingDirection;
            }

            Log.Info(LogTags.Physics, "ForceVelocity 적용 시작. {0}, {1}, Duration: {2}초, 초기 속도: ({3}, {4})",
                this.GetHierarchyPath(), assetData.Name.ToLogString(), assetData.Duration, forceVelocity.x, forceVelocity.y);

            return new ForceVelocityState
            {
                CurrentVelocity = forceVelocity,
                Acceleration = acceleration,
                LastFacingDirection = lastFacingDirection,
                ElapsedTime = 0f,
                IsFirstFrame = true
            };
        }

        private void ApplyGravitySettings(ForceVelocityAssetData assetData)
        {
            if (assetData.GravityType == FVGravityType.None)
            {
                if (!_originalGravityScale.HasValue)
                {
                    _originalGravityScale = _physics.Rigidbody.gravityScale;
                }
                _physics.Rigidbody.gravityScale = 0f;
                Log.Info(LogTags.Physics, "ForceVelocity 중력 비활성화. {0}, {1}, 원본 중력: {2}", this.GetHierarchyPath(), assetData.Name.ToLogString(), _originalGravityScale.Value);
            }
            else if (assetData.GravityType == FVGravityType.UseCustomGravity)
            {
                if (!_originalGravityScale.HasValue)
                {
                    _originalGravityScale = _physics.Rigidbody.gravityScale;
                }
                _physics.Rigidbody.gravityScale = assetData.Gravity;
                Log.Info(LogTags.Physics, "ForceVelocity 커스텀 중력 적용. {0}, {1}, 중력: {2}, 원본 중력: {3}", this.GetHierarchyPath(), assetData.Name.ToLogString(), assetData.Gravity, _originalGravityScale.Value);
            }
        }

        private bool ValidateForceVelocityComponents(ForceVelocityAssetData assetData)
        {
            if (_physics == null || _physics.Rigidbody == null || assetData == null)
            {
                Log.Warning(LogTags.Physics, "ForceVelocity 처리 중 필수 컴포넌트가 null입니다. 중지합니다. {0}", this.GetHierarchyPath());
                return false;
            }
            return true;
        }

        private bool ValidateCharacterAlive(ForceVelocityAssetData assetData)
        {
            if (_vital != null && !_vital.IsAlive)
            {
                Log.Info(LogTags.Physics, "캐릭터가 사망하여 ForceVelocity를 중지합니다. {0}, {1}", this.GetHierarchyPath(), assetData.Name.ToLogString());
                return false;
            }
            return true;
        }

        private void UpdateForceVelocityFrame(ForceVelocityAssetData assetData, ref ForceVelocityState state)
        {
            int currentFacingDirection = _physics != null ? _physics.FacingDirection : 1;
            if (currentFacingDirection != state.LastFacingDirection)
            {
                Log.Info(LogTags.Physics, "ForceVelocity FacingDirection 변경. {0}, {1}, {2} -> {3}",
                    this.GetHierarchyPath(), assetData.Name.ToLogString(), state.LastFacingDirection, currentFacingDirection);
                state.LastFacingDirection = currentFacingDirection;

                if (assetData.DirectionalType == FVDirectionalType.Facing)
                {
                    state.Acceleration.x = assetData.Acceleration.x * currentFacingDirection;
                }
                else if (assetData.DirectionalType == FVDirectionalType.Reverse)
                {
                    state.Acceleration.x = assetData.Acceleration.x * -currentFacingDirection;
                }
            }

            if ((assetData.AccelerationType & FVAccelerationType.UseAccelerationX) != 0)
            {
                state.CurrentVelocity.x += state.Acceleration.x * Time.fixedDeltaTime;
            }
            if ((assetData.AccelerationType & FVAccelerationType.UseAccelerationY) != 0)
            {
                state.CurrentVelocity.y += state.Acceleration.y * Time.fixedDeltaTime;
            }

            if ((assetData.FrictionType & FVFrictionType.UseForceFriction) != 0 && _physics.IsGrounded)
            {
                state.CurrentVelocity.x *= 1f - (assetData.Friction * Time.fixedDeltaTime);
            }

            if ((assetData.FrictionType & FVFrictionType.UseAirResist) != 0 && !_physics.IsGrounded)
            {
                float airResist = assetData.AirResist;
                state.CurrentVelocity *= 1f - (airResist * Time.fixedDeltaTime);
            }

            if ((assetData.StopOnCollisionType & FVStopOnCollisionType.StopXOnHitWall) != 0 && _physics.IsCollideX)
            {
                state.CurrentVelocity.x = 0f;
                Log.Info(LogTags.Physics, "ForceVelocity 벽 충돌로 X축 속도 중지. {0}, {1}", this.GetHierarchyPath(), assetData.Name.ToLogString());
            }

            if (_physics.IsGrounded)
            {
                if ((assetData.StopOnCollisionType & FVStopOnCollisionType.StopXOnHitGround) != 0)
                {
                    state.CurrentVelocity.x = 0f;
                    Log.Info(LogTags.Physics, "ForceVelocity 지면 충돌로 X축 속도 중지. {0}, {1}", this.GetHierarchyPath(), assetData.Name.ToLogString());
                }
                if ((assetData.StopOnCollisionType & FVStopOnCollisionType.StopYOnHitGround) != 0)
                {
                    state.CurrentVelocity.y = 0f;
                    Log.Info(LogTags.Physics, "ForceVelocity 지면 충돌로 Y축 속도 중지. {0}, {1}", this.GetHierarchyPath(), assetData.Name.ToLogString());
                }
            }
        }

        private Vector2 ApplyForceVelocity(ForceVelocityAssetData assetData, Vector2 currentVelocity, bool isFirstFrame)
        {
            if (assetData.GravityType == FVGravityType.None)
            {
                // 중력이 없으면 전체 velocity를 직접 설정
                _physics.ApplyVelocity(currentVelocity);
                return currentVelocity;
            }
            else
            {
                // 중력이 있으면 Rigidbody의 중력이 작용
                Vector2 currentRigidbodyVelocity = _physics.RigidbodyVelocity;
                // 첫 프레임에만 y값을 설정하고, 이후에는 Rigidbody의 y를 유지 (중력이 자연스럽게 작용)
                float velocityY = isFirstFrame ? currentVelocity.y : currentRigidbodyVelocity.y;
                Vector2 newVelocity = new(currentVelocity.x, velocityY);
                _physics.ApplyVelocity(newVelocity);
                return newVelocity;
            }
        }

        private void FinalizeForceVelocity(ForceVelocityAssetData assetData, object itemSource)
        {
            // 현재 FV가 완료되었으므로 null로 설정
            _currentForceVelocity = null;

            // 중력 복원 및 애니메이터 파라미터 리셋
            RestoreGravity();
            UpdateAnimatorParameters(Vector2.zero);

            Log.Info(LogTags.Physics, "ForceVelocity가 완료되어 중력이 복원되었습니다. {0}", this.GetHierarchyPath());
        }

        private void StopAllForceVelocities()
        {
            if (!_currentForceVelocity.HasValue)
            {
                return;
            }

            string fvName = _currentForceVelocity.Value.AssetData != null ? _currentForceVelocity.Value.AssetData.Name.ToLogString() : "Unknown";
            Log.Info(LogTags.Physics, "현재 ForceVelocity를 중지합니다. {0}, {1}", this.GetHierarchyPath(), fvName);

            StopCurrentForceVelocity();
        }


        private void RestoreGravity()
        {
            if (_physics != null && _physics.Rigidbody != null && _originalGravityScale.HasValue)
            {
                _physics.Rigidbody.gravityScale = _originalGravityScale.Value;
                Log.Info(LogTags.Physics, "중력 복원. {0}, 중력 스케일: {1}", this.GetHierarchyPath(), _originalGravityScale.Value);
                _originalGravityScale = null;
            }
        }

        private void UpdateAnimatorParameters(Vector2 forceVelocity)
        {
            if (_characterAnimator != null)
            {
                _characterAnimator.SetForceSpeedX(forceVelocity.x);
                _characterAnimator.SetForceSpeedY(forceVelocity.y);
            }
        }
    }
}