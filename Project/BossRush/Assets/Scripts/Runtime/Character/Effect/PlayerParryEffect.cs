using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerParryEffect : XBehaviour
    {
        [FoldoutGroup("#PlayerParryEffect-Settings")]
        [SerializeField]
        [Tooltip("패리 성공 시 재생할 VFX 프리팹")]
        private GameObject _parrySuccessVFXPrefab;

        [FoldoutGroup("#PlayerParryEffect-Settings")]
        [SerializeField]
        [Tooltip("패리 성공 시 넉백 타입")]
        private KnockbackType _knockbackType = KnockbackType.Both;

        [FoldoutGroup("#PlayerParryEffect-Settings")]
        [SerializeField]
        [Tooltip("슬로우 모션 지속 시간")]
        private float _slowMotionDuration = 0.05f;

        [FoldoutGroup("#PlayerParryEffect-Settings")]
        [SerializeField]
        [Tooltip("슬로우 모션 배율 (0.01 = 1% 속도)")]
        private float _slowMotionFactor = 0.01f;

        private Character _character;
        private CharacterPhysics _physics;
        private Vital _vital;
        private Coroutine _slowMotionCoroutine;

        private void Awake()
        {
            _character = GetComponent<Character>();
            _physics = _character?.Physics;
            _vital = _character?.MyVital;
        }

        public void OnParrySuccess(Character attacker, Vector3 attackPosition)
        {
            if (_character == null)
            {
                return;
            }

            ApplyPulseReward();

            if (_knockbackType != KnockbackType.None && attacker != null)
            {
                ApplyKnockback(attacker);
            }

            SpawnVFX(attackPosition);
            ApplySlowMotion();
        }

        private void ApplyPulseReward()
        {
            if (_vital?.Pulse != null)
            {
                _vital.Pulse.OnParrySuccess();
                Log.Info(LogTags.Player, $"패링 성공! 펄스를 증가시킵니다. {_vital.Pulse.Current}/{_vital.Pulse.Max}");
            }

            _character.CharacterAnimator?.SetParrySuccess(true);
        }

        private void ApplyKnockback(Character attacker)
        {
            if (attacker == null || _character == null || _physics == null)
            {
                return;
            }

            Vector3 attackerPosition = attacker.transform.position;
            Vector3 defenderPosition = _character.transform.position;
            Vector3 direction = (attackerPosition - defenderPosition).normalized;

            Vector2 knockbackDirection = new Vector2(direction.x, 0f).normalized;

            switch (_knockbackType)
            {
                case KnockbackType.Defender:
                    _physics.ApplyKnockback(-knockbackDirection);
                    break;

                case KnockbackType.Attacker:
                    attacker.Physics?.ApplyKnockback(knockbackDirection);
                    break;

                case KnockbackType.Both:
                    attacker.Physics?.ApplyKnockback(knockbackDirection);
                    _physics.ApplyKnockback(-knockbackDirection);
                    break;

                case KnockbackType.None:
                default:
                    break;
            }
        }

        private void SpawnVFX(Vector3 attackPosition)
        {
            if (_parrySuccessVFXPrefab == null || _character == null)
            {
                return;
            }

            Vector3 playerPosition = _character.transform.position;
            Vector3 centerPosition = (attackPosition + playerPosition) * 0.5f;
            bool isFacingRight = _character.IsFacingRight;

            _ = VFXManager.Spawn(_parrySuccessVFXPrefab, centerPosition, isFacingRight);
        }

        public void ApplySlowMotion()
        {
            _slowMotionCoroutine ??= StartXCoroutine(GameTimeManager.Instance.ActivateSlowMotion(_slowMotionDuration, _slowMotionFactor, OnCompletedSlowMotion));
        }

        private void OnCompletedSlowMotion()
        {
            _slowMotionCoroutine = null;
        }
    }
}