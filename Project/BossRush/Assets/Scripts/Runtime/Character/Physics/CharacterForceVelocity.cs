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
        private readonly List<ForceVelocityItem> _activeForceVelocities = new();
        private float? _originalGravityScale;

        public bool IsProcessing
        {
            get
            {
                if (_activeForceVelocities.Count <= 0)
                {
                    return false;
                }

                ForceVelocityItem item = GetTopPriorityItem();
                if (item.Coroutine == null)
                {
                    return false;
                }

                return true;
            }
        }

        public bool IsProcessingForName(FVNames name)
        {
            if (_activeForceVelocities.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < _activeForceVelocities.Count; i++)
            {
                ForceVelocityItem item = _activeForceVelocities[i];
                if (item.AssetData != null && item.AssetData.Name == name && item.Coroutine != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            _vital = GetComponentInChildren<Vital>();
            _character = GetComponentInParent<Character>();
            _characterAnimator = _character != null ? _character.CharacterAnimator : null;
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

            int priority = assetData.Priority;
            bool wasProcessing = IsProcessing;
            ForceVelocityItem previousTopItem = wasProcessing ? GetTopPriorityItem() : default;

            ForceVelocityItem newItem = new()
            {
                AssetData = assetData,
                Source = source,
                Priority = priority,
                Coroutine = null,
                IsFacingRight = isFacingRight,
                ElapsedTime = 0f,
                AppliedForce = Vector2.zero
            };

            int insertIndex = 0;
            for (int i = 0; i < _activeForceVelocities.Count; i++)
            {
                if (priority > _activeForceVelocities[i].Priority)
                {
                    insertIndex = i;
                    break;
                }
                if (priority == _activeForceVelocities[i].Priority)
                {
                    insertIndex = i;
                    break;
                }
                insertIndex = i + 1;
            }
            _activeForceVelocities.Insert(insertIndex, newItem);

            Log.Info(LogTags.Physics, "ForceVelocity를 시작합니다. {0}, {1}, Priority: {2}", this.GetHierarchyPath(), assetData.Name.ToLogString(), priority);

            ForceVelocityItem currentTopItem = GetTopPriorityItem();
            if (wasProcessing && previousTopItem.Coroutine != null && currentTopItem.AssetData != previousTopItem.AssetData)
            {
                Log.Info(LogTags.Physics, "높은 우선순위 ForceVelocity가 이전 항목을 일시정지합니다. {0}, {1} -> {2}",
                    this.GetHierarchyPath(), previousTopItem.AssetData.Name.ToLogString(), currentTopItem.AssetData.Name.ToLogString());
                StopCoroutine(previousTopItem.Coroutine);
            }

            if (currentTopItem.AssetData == assetData)
            {
                Coroutine newCoroutine = StartCoroutine(ProcessForceVelocity(insertIndex));
                ForceVelocityItem updatedItem = currentTopItem;
                updatedItem.Coroutine = newCoroutine;
                _activeForceVelocities[0] = updatedItem;
            }
        }

        public void StopForceVelocity(object source = null, FVNames? name = null)
        {
            if (_activeForceVelocities.Count == 0)
            {
                return;
            }

            if (source == null && name == null)
            {
                StopAllForceVelocities();
                return;
            }

            List<int> indicesToRemove = new();
            for (int i = _activeForceVelocities.Count - 1; i >= 0; i--)
            {
                ForceVelocityItem item = _activeForceVelocities[i];
                bool shouldRemove = false;

                if (source != null && item.Source == source)
                {
                    shouldRemove = true;
                }
                else if (name.HasValue && item.AssetData != null && item.AssetData.Name == name.Value)
                {
                    shouldRemove = true;
                }

                if (shouldRemove)
                {
                    indicesToRemove.Add(i);
                }
            }

            foreach (int index in indicesToRemove)
            {
                ForceVelocityItem item = _activeForceVelocities[index];
                string fvName = item.AssetData != null ? item.AssetData.Name.ToLogString() : "Unknown";

                Log.Info(LogTags.Physics, "ForceVelocity를 중지합니다. {0}, {1}", this.GetHierarchyPath(), fvName);

                if (item.Coroutine != null)
                {
                    StopCoroutine(item.Coroutine);
                }

                _activeForceVelocities.RemoveAt(index);
            }

            if (indicesToRemove.Contains(0) && _activeForceVelocities.Count > 0)
            {
                RestoreGravity();
                UpdateAnimatorParameters(Vector2.zero);

                ForceVelocityItem nextItem = GetTopPriorityItem();
                if (nextItem.AssetData != null && nextItem.Coroutine == null)
                {
                    Coroutine nextCoroutine = StartCoroutine(ProcessForceVelocity(0));
                    nextItem.Coroutine = nextCoroutine;
                    _activeForceVelocities[0] = nextItem;
                    Log.Info(LogTags.Physics, "다음 우선순위 ForceVelocity로 전환합니다. {0}, {1}", this.GetHierarchyPath(), nextItem.AssetData.Name.ToLogString());
                }
            }
            else if (_activeForceVelocities.Count == 0)
            {
                RestoreGravity();
                UpdateAnimatorParameters(Vector2.zero);
                Log.Info(LogTags.Physics, "모든 ForceVelocity가 제거되어 중력이 복원되었습니다. {0}", this.GetHierarchyPath());
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

        private IEnumerator ProcessForceVelocity(int itemIndex)
        {
            if (itemIndex < 0 || itemIndex >= _activeForceVelocities.Count)
            {
                yield break;
            }

            ForceVelocityItem item = _activeForceVelocities[itemIndex];
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

            if (itemIndex >= _activeForceVelocities.Count || _activeForceVelocities[itemIndex].AssetData != assetData)
            {
                yield break;
            }

            ForceVelocityState state = InitializeForceVelocity(assetData);
            object itemSource = item.Source;

            while (state.ElapsedTime < assetData.Duration)
            {
                itemIndex = FindItemIndex(assetData, itemSource);
                if (itemIndex < 0)
                {
                    yield break;
                }
                item = _activeForceVelocities[itemIndex];

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

                Vector2 appliedForce = ApplyForceVelocity(assetData, state.CurrentVelocity);
                item.ElapsedTime = state.ElapsedTime;
                item.AppliedForce = appliedForce;
                _activeForceVelocities[itemIndex] = item;

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
                ElapsedTime = 0f
            };
        }

        private void ApplyGravitySettings(ForceVelocityAssetData assetData)
        {
            if ((assetData.GravityType & FVGravityType.UseGravity) == 0)
            {
                if (!_originalGravityScale.HasValue)
                {
                    _originalGravityScale = _physics.Rigidbody.gravityScale;
                }
                _physics.Rigidbody.gravityScale = 0f;
                Log.Info(LogTags.Physics, "ForceVelocity 중력 비활성화. {0}, {1}, 원본 중력: {2}", this.GetHierarchyPath(), assetData.Name.ToLogString(), _originalGravityScale.Value);
            }
            else if ((assetData.GravityType & FVGravityType.UseCustomGravity) != 0)
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

        private Vector2 ApplyForceVelocity(ForceVelocityAssetData assetData, Vector2 currentVelocity)
        {
            if ((assetData.GravityType & FVGravityType.UseGravity) == 0)
            {
                _physics.ApplyVelocity(currentVelocity);
                return currentVelocity;
            }
            else
            {
                Vector2 currentRigidbodyVelocity = _physics.RigidbodyVelocity;
                Vector2 newVelocity = new(currentVelocity.x, currentRigidbodyVelocity.y);
                _physics.ApplyVelocity(newVelocity);
                return newVelocity;
            }
        }

        private void FinalizeForceVelocity(ForceVelocityAssetData assetData, object itemSource)
        {
            int itemIndex = FindItemIndex(assetData, itemSource);
            if (itemIndex >= 0)
            {
                _activeForceVelocities.RemoveAt(itemIndex);
            }

            if (_activeForceVelocities.Count == 0)
            {
                RestoreGravity();
                UpdateAnimatorParameters(Vector2.zero);
            }
            else
            {
                ForceVelocityItem nextItem = GetTopPriorityItem();
                if (nextItem.AssetData != null && nextItem.Coroutine == null)
                {
                    Coroutine nextCoroutine = StartCoroutine(ProcessForceVelocity(0));
                    nextItem.Coroutine = nextCoroutine;
                    _activeForceVelocities[0] = nextItem;
                    Log.Info(LogTags.Physics, "다음 우선순위 ForceVelocity로 전환합니다. {0}, {1}", this.GetHierarchyPath(), nextItem.AssetData.Name.ToLogString());
                }
            }
        }

        private void StopAllForceVelocities()
        {
            if (_activeForceVelocities.Count == 0)
            {
                return;
            }

            ForceVelocityItem topItem = GetTopPriorityItem();
            string fvName = topItem.AssetData != null ? topItem.AssetData.Name.ToLogString() : "Unknown";
            Log.Info(LogTags.Physics, "모든 ForceVelocity를 중지합니다. {0}, {1}", this.GetHierarchyPath(), fvName);

            for (int i = 0; i < _activeForceVelocities.Count; i++)
            {
                ForceVelocityItem item = _activeForceVelocities[i];
                if (item.Coroutine != null)
                {
                    StopCoroutine(item.Coroutine);
                }
            }

            _activeForceVelocities.Clear();
            RestoreGravity();
            UpdateAnimatorParameters(Vector2.zero);
        }

        private ForceVelocityItem GetTopPriorityItem()
        {
            if (_activeForceVelocities.Count == 0)
            {
                return default;
            }
            return _activeForceVelocities[0];
        }

        private int FindItemIndex(ForceVelocityAssetData assetData, object source)
        {
            for (int i = 0; i < _activeForceVelocities.Count; i++)
            {
                ForceVelocityItem item = _activeForceVelocities[i];
                if (item.AssetData == assetData && item.Source == source)
                {
                    return i;
                }
            }
            return -1;
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