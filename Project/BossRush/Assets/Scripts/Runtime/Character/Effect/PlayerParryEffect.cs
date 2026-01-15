using Rewired;
using Sirenix.OdinInspector;
using TeamSuneat.Audio;
using TeamSuneat.CameraSystem.Core;
using TeamSuneat.Data;
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

        [FoldoutGroup("#PlayerParryEffect-SlowMotion")]
        [SerializeField]
        [Tooltip("슬로우 모션 지속 시간")]
        private float _slowMotionDuration = 0.05f;

        [FoldoutGroup("#PlayerParryEffect-SlowMotion")]
        [SerializeField]
        [Tooltip("슬로우 모션 배율 (0.01 = 1% 속도)")]
        private float _slowMotionFactor = 0.01f;

        [FoldoutGroup("#PlayerParryEffect-Vibration")]
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

        private void Awake()
        {
            _character = GetComponent<Character>();
            _physics = _character?.Physics;
            _vital = _character?.MyVital;
        }

        public void OnParrySuccess(Character attacker, Character targetCharacter, Vector3 attackPosition, bool applyStun = true)
        {
            if (_character == null)
            {
                return;
            }

            if (_knockbackType != KnockbackType.None && attacker != null)
            {
                ApplyKnockback(attacker);
            }

            // 공격자에게 1초 기절 적용 (패링 타입에 따라 결정)
            if (applyStun && attacker != null)
            {
                attacker.ApplyStun(1f);
            }

            SpawnVFX(attackPosition);

            ApplyPulseReward();
            ApplySound();
            ApplySlowMotion();
            ApplyVibration();
            ApplyParryRendererEffect(targetCharacter);
            ApplyCameraShake(attackPosition);
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
            GameTimeManager.Instance?.StartSlowMotion(_slowMotionDuration, _slowMotionFactor);
        }

        private void ApplyVibration()
        {
            if (GameSetting.Instance?.Play?.Vibration != true)
            {
                return;
            }

            Player inputPlayer = TSInputManager.Instance?.InputPlayer;
            if (inputPlayer == null)
            {
                return;
            }

            inputPlayer.SetVibration(0, _vibrationLeftMotorIntensity, _vibrationDuration);
            inputPlayer.SetVibration(1, _vibrationRightMotorIntensity, _vibrationDuration);
        }

        private void ApplyParryRendererEffect(Character targetCharacter)
        {
            if (targetCharacter.CharacterRenderer != null)
            {
                targetCharacter.CharacterRenderer.StartFlickerCoroutine(RendererFlickerNames.Parry);
            }
        }

        private void ApplyCameraShake(Vector3 attackPosition)
        {
            if (CameraManager.Instance == null)
            {
                return;
            }

            // 공격자 위치를 기준으로 방향 결정
            Vector3 defenderPosition = _character.transform.position;
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

            CameraManager.Instance.ShakeAtPosition(_character.transform.position, asset);
        }
    }
}