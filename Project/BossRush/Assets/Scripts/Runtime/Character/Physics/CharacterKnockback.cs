using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterKnockback : MonoBehaviour
    {
        [Title("Knockback")]
        [SerializeField][Range(0f, 1f)] private float _bounceMultiplier = 0.5f;
        [SerializeField][Range(0f, 1f)] private float _verticalRatio = 0.3f;
        [SerializeField] private float _minReflectedForce = 1f;

        private CharacterPhysicsCore _physics;
        private CharacterForceVelocity _forceVelocity;
        private Vector2 _lastKnockbackDirection;
        private float _lastKnockbackForce;
        private float _lastVerticalRatio;

        public bool IsKnockback => _forceVelocity != null && _forceVelocity.IsProcessingForName(FVNames.PlayerKnockback);

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            _forceVelocity = GetComponent<CharacterForceVelocity>();
        }

        public void ApplyKnockback(Vector2 direction)
        {
            if (_physics == null || _forceVelocity == null)
            {
                return;
            }

            if (direction.magnitude < 0.01f)
            {
                direction = new Vector2(-_physics.FacingDirection, 0f);
            }
            else
            {
                direction.Normalize();
            }

            // ForceVelocityAsset 데이터 가져오기
            ForceVelocityAssetData knockbackAssetData = ScriptableDataManager.Instance?.FindForceVelocityClone(FVNames.PlayerKnockback);
            if (knockbackAssetData == null)
            {
                Log.Warning(LogTags.Physics, "PlayerKnockback ForceVelocity 데이터를 찾을 수 없습니다. {0}", this.GetHierarchyPath());
                return;
            }

            // 방향에 맞게 ForceVelocity 설정
            // direction이 왼쪽(-1)이면 isFacingRight = false, 오른쪽(1)이면 isFacingRight = true
            bool isFacingRight = direction.x > 0f;

            // 방향과 힘을 저장 (벽 충돌 반사용)
            _lastKnockbackDirection = direction;
            float baseForce = Mathf.Abs(knockbackAssetData.ForceVelocity.x);
            _lastKnockbackForce = baseForce;
            _lastVerticalRatio = _verticalRatio;

            // ForceVelocity의 ForceVelocity 벡터를 방향에 맞게 조정
            // 수평 힘은 direction.x 방향으로, 수직 힘은 verticalRatio를 적용하여 direction.y 방향으로
            ForceVelocityAssetData clonedData = knockbackAssetData.Clone();
            clonedData.ForceVelocity = new Vector2(
                baseForce * direction.x,
                baseForce * _verticalRatio * direction.y
            );

            // ForceVelocity 시작
            _forceVelocity.StartForceVelocity(clonedData, isFacingRight, this);
        }

        public void OnWallCollision()
        {
            if (!IsKnockback || _forceVelocity == null)
            {
                return;
            }

            // 반사된 힘 계산
            float reflectedForce = _lastKnockbackForce * _bounceMultiplier;

            // 최소 반사 힘 체크 - 너무 작으면 반사하지 않음
            if (reflectedForce < _minReflectedForce)
            {
                Log.Info(LogTags.Physics, "반사 힘이 최소값보다 작아 반사를 중지합니다. {0}, 반사 힘: {1}, 최소값: {2}", this.GetHierarchyPath(), reflectedForce, _minReflectedForce);
                _forceVelocity.StopForceVelocity(this, FVNames.PlayerKnockback);
                return;
            }

            // 현재 ForceVelocity 중지
            _forceVelocity.StopForceVelocity(this, FVNames.PlayerKnockback);

            // 반사된 방향 계산
            Vector2 reflectedDirection = new Vector2(-_lastKnockbackDirection.x, _lastKnockbackDirection.y);
            reflectedDirection.Normalize();

            // ForceVelocityAsset 데이터 가져오기
            ForceVelocityAssetData knockbackAssetData = ScriptableDataManager.Instance?.FindForceVelocityClone(FVNames.PlayerKnockback);
            if (knockbackAssetData == null)
            {
                Log.Warning(LogTags.Physics, "PlayerKnockback ForceVelocity 데이터를 찾을 수 없습니다. {0}", this.GetHierarchyPath());
                return;
            }

            // 반사된 방향과 힘으로 새로운 ForceVelocity 설정
            bool isFacingRight = reflectedDirection.x > 0f;

            ForceVelocityAssetData clonedData = knockbackAssetData.Clone();
            clonedData.ForceVelocity = new Vector2(
                reflectedForce * reflectedDirection.x,
                reflectedForce * _lastVerticalRatio * reflectedDirection.y
            );

            // 저장된 값 업데이트
            _lastKnockbackDirection = reflectedDirection;
            _lastKnockbackForce = reflectedForce;

            // 반사된 ForceVelocity 시작
            _forceVelocity.StartForceVelocity(clonedData, isFacingRight, this);
        }

        public void AbilityTick()
        {
            // 실제 Knockback 처리는 ForceVelocity가 처리
            // 여기서는 특별한 처리가 필요 없음
        }
    }
}