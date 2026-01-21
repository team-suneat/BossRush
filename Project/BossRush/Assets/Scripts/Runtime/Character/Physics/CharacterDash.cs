using Sirenix.OdinInspector;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(CharacterPhysicsCore))]
    public class CharacterDash : MonoBehaviour
    {
        [Title("Dash")]
        [SerializeField] private float _dashCooldown = 0.5f;
        [SerializeField] private bool _airDashEnabled = true;

        private CharacterPhysicsCore _physics;
        private CharacterForceVelocity _forceVelocity;
        private Vital _vital;
        private Character _character;
        private float _dashCooldownRemaining;
        private bool _wasOnCooldown;

        public bool IsDashing => _forceVelocity != null && _forceVelocity.IsProcessing;
        public bool CanDash => _dashCooldownRemaining <= 0f && !IsDashing && HasPulse();
        public bool IsAirDashEnabled => _airDashEnabled;
        public float DashCooldownRemaining => _dashCooldownRemaining;

        private void Awake()
        {
            _physics = GetComponent<CharacterPhysicsCore>();
            _forceVelocity = GetComponent<CharacterForceVelocity>();
            _vital = GetComponentInChildren<Vital>();
            _character = GetComponentInParent<Character>();
        }

        // 방향 없이 대시 요청 (캐릭터가 바라보는 방향으로 대시)
        public void RequestDash()
        {
            Vector2 direction = new Vector2(_physics != null ? _physics.FacingDirection : 1f, 0f);
            RequestDash(direction);
        }

        private void RequestDash(Vector2 direction)
        {
            if (!CanDash) { return; }
            if (_physics == null) { return; }
            if (_physics.IsKnockback) { return; }
            if (!_airDashEnabled && !_physics.IsGrounded) { return; }

            ExecuteDash(direction);
        }

        private void ExecuteDash(Vector2 direction)
        {
            if (_physics == null || _forceVelocity == null) { return; }

            // 방향 정규화
            if (direction.magnitude < 0.01f)
            {
                direction = new Vector2(_physics.FacingDirection, 0f);
            }
            else
            {
                direction.Normalize();
            }

            // 펄스 소모
            if (!ConsumePulse()) { return; }

            // ForceVelocityAsset 데이터 가져오기
            ForceVelocityAssetData dashAssetData = ScriptableDataManager.Instance?.FindForceVelocityClone(FVNames.PlayerDash);
            if (dashAssetData == null)
            {
                Log.Warning(LogTags.Physics, "PlayerDash ForceVelocity 데이터를 찾을 수 없습니다. {0}", this.GetHierarchyPath());
                return;
            }

            // 캐릭터가 바라보는 방향 확인
            bool isFacingRight = _physics.FacingDirection > 0;

            // ForceVelocity 시작
            _forceVelocity.StartForceVelocity(dashAssetData, isFacingRight, this);

            // 대시 flicker 효과 적용
            StartDashFlicker();

            _dashCooldownRemaining = _dashCooldown;
            _wasOnCooldown = true;
        }

        public void SetAirDashEnabled(bool enabled)
        {
            _airDashEnabled = enabled;
        }

        public void AbilityTick()
        {
            // 캐릭터가 살아있지 않으면 ForceVelocity 중지
            if (_vital != null && !_vital.IsAlive)
            {
                if (IsDashing && _forceVelocity != null)
                {
                    _forceVelocity.StopForceVelocity(this, FVNames.PlayerDash);
                }
                return;
            }

            // 쿨다운 관리 (실제 대시는 ForceVelocity가 처리)
            if (_dashCooldownRemaining > 0f)
            {
                _dashCooldownRemaining -= Time.fixedDeltaTime;
                _wasOnCooldown = true;
            }
            else if (_wasOnCooldown)
            {
                // 쿨다운이 끝난 순간에만 flicker 효과 적용
                StartDashCooldownFlicker();
                _wasOnCooldown = false;
            }
        }

        private bool HasPulse()
        {
            if (_vital == null) { return false; }
            if (_vital.Pulse == null) { return false; }

            return _vital.Pulse.Current >= 1 && !_vital.Pulse.IsBurnout;
        }

        private bool ConsumePulse()
        {
            if (_vital == null) { return false; }

            return _vital.UseDash();
        }

        private void StartDashFlicker()
        {
            if (_character == null || _character.CharacterRenderer == null) { return; }

            _character.CharacterRenderer.StartFlickerCoroutine(RendererFlickerNames.Dash);
        }

        private void StartDashCooldownFlicker()
        {
            if (_character == null || _character.CharacterRenderer == null) { return; }

            _character.CharacterRenderer.StartFlickerCoroutine(RendererFlickerNames.Dash);
        }
    }
}