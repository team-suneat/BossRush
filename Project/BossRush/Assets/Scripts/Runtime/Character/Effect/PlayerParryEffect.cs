using Rewired;
using Sirenix.OdinInspector;
using TeamSuneat.Audio;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    [RequireComponent(typeof(PlayerCharacter))]
    public class PlayerParryEffect : XBehaviour
    {
        [FoldoutGroup("#PlayerParryEffect-VFX")]
        [SerializeField]
        [Tooltip("패리 성공 시 재생할 VFX 프리팹")]
        private GameObject _parrySuccessVFXPrefab;

        [FoldoutGroup("#PlayerParryEffect-Knockback")]
        [SerializeField]
        [Tooltip("패리 성공 시 넉백 타입")]
        private KnockbackType _knockbackType = KnockbackType.Both;

        [FoldoutGroup("#PlayerParryEffect-Knockback")]
        [SerializeField]
        [Tooltip("슬로우 모션 지속 시간")]
        private float _slowMotionDuration = 0.05f;

        [FoldoutGroup("#PlayerParryEffect-SlowMotion")]
        [SerializeField]
        [Tooltip("슬로우 모션 배율 (0.01 = 1% 속도)")]
        private float _slowMotionFactor = 0.01f;

        [FoldoutGroup("#PlayerParryEffect-SlowMotion")]
        [SerializeField]
        [Tooltip("패리 성공 시 왼쪽 모터 진동 강도 (0.0 ~ 1.0)")]
        private float _vibrationLeftMotorIntensity = 0.6f;

        [FoldoutGroup("#PlayerParryEffect-Vibration")]
        [SerializeField]
        [Tooltip("패리 성공 시 오른쪽 모터 진동 강도 (0.0 ~ 1.0)")]
        private float _vibrationRightMotorIntensity = 0.6f;

        [FoldoutGroup("#PlayerParryEffect-Vibration")]
        [SerializeField]
        [Tooltip("패리 성공 시 진동 지속 시간 (초)")]
        private float _vibrationDuration = 0.15f;

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
            ApplySound();
            ApplySlowMotion();
            ApplyVibration();
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

        private void ApplySound()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            Vector3 playerPosition = _character.transform.position;
            _ = AudioManager.Instance.PlaySFXOneShotScaled(SoundNames.Parry_Success, playerPosition);
        }

        public void ApplySlowMotion()
        {
            _slowMotionCoroutine ??= StartXCoroutine(GameTimeManager.Instance.ActivateSlowMotion(_slowMotionDuration, _slowMotionFactor, OnCompletedSlowMotion));
        }

        private void OnCompletedSlowMotion()
        {
            _slowMotionCoroutine = null;
        }

        private void ApplyVibration()
        {
            // if (GameSetting.Instance?.Play?.Vibration != true)
            // {
            //     return;
            // }

            Player inputPlayer = TSInputManager.Instance?.InputPlayer;
            if (inputPlayer == null)
            {
                return;
            }

            inputPlayer.SetVibration(0, _vibrationLeftMotorIntensity, _vibrationDuration);
            inputPlayer.SetVibration(1, _vibrationRightMotorIntensity, _vibrationDuration);
        }
    }
}