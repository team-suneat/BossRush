using DG.Tweening;
using Lean.Pool;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIFloatyText : XBehaviour, IPoolable
    {
        public UILocalizedText Text;
        public Image TextImage;
        public AutoDespawn Despawner;
        public UIFollowObject FollowObject;

        private Tweener _moveTweener;
        private Tweener _scaleTweener;
        private Tweener _fadeTextTweener;
        private Tweener _fadeTextImageTweener;
        private Sequence _sequence;
        private bool _isFacingRight;
        private FloatyAsset _asset;
        private UnityAction<UIFloatyText> _despawnCallback;

        public string Content => Text != null ? Text.Content : string.Empty;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Despawner ??= GetComponent<AutoDespawn>();
            FollowObject ??= GetComponent<UIFollowObject>();
            Text ??= GetComponentInChildren<UILocalizedText>();
            TextImage ??= GetComponentInChildren<Image>();
        }

        public static UIFloatyMoveNames ConvertToName(DamageResult damageResult, VitalResourceTypes vitalResourceType)
        {
            if (damageResult.TargetCharacter != null && damageResult.TargetCharacter.IsPlayer)
            {
                if (vitalResourceType == VitalResourceTypes.Health)
                {
                    return UIFloatyMoveNames.PlayerDamaged;
                }

                if (vitalResourceType == VitalResourceTypes.Shield)
                {
                    return UIFloatyMoveNames.ChargeShield;
                }
            }

            switch (damageResult.DamageType)
            {
                case DamageTypes.Heal:
                case DamageTypes.HealOverTime:
                    return UIFloatyMoveNames.HealHealth;

                case DamageTypes.RestoreMana:
                case DamageTypes.RestoreManaOverTime:
                    return UIFloatyMoveNames.RestoreMana;

                case DamageTypes.ChargeShield:
                    return UIFloatyMoveNames.ChargeShield;

                case DamageTypes.Normal:
                case DamageTypes.DamageOverTime:
                case DamageTypes.Thorns:
                    if (damageResult.DamageValue == float.MaxValue)
                    {
                        return UIFloatyMoveNames.Execution;
                    }

                    if (damageResult.IsCritical)
                    {
                        return UIFloatyMoveNames.CriticalDamage;
                    }

                    return UIFloatyMoveNames.Damage;

                case DamageTypes.Execution:
                    return UIFloatyMoveNames.Execution;

                default:
                    return UIFloatyMoveNames.Content;
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            StopFadeTweeners();
            StopMoveTweener();
            StopPunchScale();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            CallDespawnEvent();
            ResetText();
        }

        public void Despawn()
        {
            Despawner?.Despawn();
        }

        public void RegisterDespawnEvent(UnityAction<UIFloatyText> onDespawn)
        {
            _despawnCallback = onDespawn;
        }

        private void CallDespawnEvent()
        {
            if (_despawnCallback != null)
            {
                _despawnCallback.Invoke(this);
                _despawnCallback = null;
            }
        }

        //

        public void Setup(string content, UIFloatyMoveNames moveType)
        {
            _asset = ScriptableDataManager.Instance.FindFloaty(moveType);
            if (!_asset.IsValid())
            {
                Log.Warning("Float 에셋을 찾을 수 없습니다: {0}", moveType);
                return;
            }

            SetText(content);
            ApplyFontColor();
            ApplyFont();
            ApplySpawnPosition();
            FadeOut();
            StartPunchScale();
        }

        #region Text

        private void ResetText()
        {
            if (Text != null)
            {
                Text.ResetText();
            }
        }

        private void SetText(string content)
        {
            if (Text != null)
            {
                Text.SetText(content);
            }
        }

        private void ApplyFontColor()
        {
            if (Text != null && _asset != null)
            {
                Text.SetTextColor(_asset.TextColor);
            }
        }

        private void ApplyFont()
        {
            if (Text != null && _asset != null)
            {
                Text.FontType = _asset.FontType;
                Text.FontTypeString = _asset.FontTypeString;
                Text.Refresh(GameSetting.Instance.Language.Name);
            }
        }

        #endregion Text

        #region Position

        private void ApplySpawnPosition()
        {
            if (FollowObject == null || _asset == null)
            {
                return;
            }

            if (!_asset.SpawnArea.IsZero())
            {
                Vector3 offset = RandomEx.GetVector3Value(_asset.SpawnArea);
                offset += _asset.SpawnOffset;

                FollowObject.SetWorldOffset(offset);
            }
        }

        #endregion Position

        #region Move

        public void SetFacingRight(Vector3 damageDirection)
        {
            if (_asset == null)
            {
                return;
            }

            if (_asset.RandomFace)
            {
                _isFacingRight = RandomEx.GetBoolValue();
                return;
            }

            if (damageDirection.x > 0)
            {
                _isFacingRight = true;
            }
            else if (damageDirection.x < 0)
            {
                _isFacingRight = false;
            }
            else
            {
                _isFacingRight = true;
            }
        }

        public void SetFacingRight(bool isFacingRight)
        {
            if (_asset == null)
            {
                return;
            }

            if (_asset.RandomFace)
            {
                _isFacingRight = RandomEx.GetBoolValue();
            }
            else
            {
                _isFacingRight = isFacingRight;
            }
        }

        public void StartMove()
        {
            if (_moveTweener != null || _sequence != null)
            {
                return;
            }

            ResetTextPosition();

            if (_asset == null)
            {
                Log.Warning("UIFloatyText의 Asset을 찾을 수 없습니다.");
                return;
            }

            if (_asset.Type == UIFloatyMoveTypes.Velocity)
            {
                StartVelocityMove(GetVelocity(), _asset.Duration);
            }
            else if (_asset.Type == UIFloatyMoveTypes.Jump)
            {
                StartJumpMove();
            }
        }

        private Vector2 GetVelocity()
        {
            if (_asset == null)
            {
                return Vector2.zero;
            }

            if (_isFacingRight)
            {
                return new Vector2(Mathf.Abs(_asset.Velocity.x), _asset.Velocity.y);
            }

            return new Vector2(-Mathf.Abs(_asset.Velocity.x), _asset.Velocity.y);
        }

        private void StartVelocityMove(Vector2 velocity, float duration)
        {
            if (velocity.IsZero() || Text == null || _asset == null)
            {
                return;
            }

            _moveTweener = Text.transform.DOLocalMove(velocity, duration);
            if (_moveTweener != null)
            {
                _moveTweener.onComplete += OnMoved;
                if (_asset.DelayTime > 0)
                {
                    _moveTweener.SetDelay(_asset.DelayTime);
                }
            }
        }

        private void StartJumpMove()
        {
            if (Text == null || _asset == null)
            {
                return;
            }

            Vector3 jumpEndValue = _isFacingRight ? _asset.JumpEndValue : _asset.JumpEndValue.FlipX();
            _sequence = Text.transform.DOLocalJump(jumpEndValue, _asset.JumpPower, _asset.NumberOfJumps, _asset.Duration);

            if (_sequence != null)
            {
                _sequence.onComplete += OnMoved;
                if (_asset.DelayTime > 0)
                {
                    _sequence.SetDelay(_asset.DelayTime);
                }
            }
        }

        private void StopMoveTweener()
        {
            if (_moveTweener != null)
            {
                _moveTweener.Kill();
                _moveTweener = null;
            }
        }

        private void ResetTextPosition()
        {
            if (Text != null)
            {
                Text.transform.localPosition = Vector3.zero;
            }
        }

        private void OnMoved()
        {
            _moveTweener = null;
            _sequence = null;

            ResetTextPosition();

            Despawn();
        }

        #endregion Move

        #region Fade

        private void FadeOut()
        {
            if (_asset == null || !_asset.UseFadeOut)
            {
                return;
            }

            if (Text != null)
            {
                _fadeTextTweener = Text.FadeOut(_asset.FadeTargetAlpha, _asset.Duration, _asset.DelayTime);
                if (_fadeTextTweener != null)
                {
                    _fadeTextTweener.onComplete = () => { _fadeTextTweener = null; };
                }
            }

            if (TextImage != null)
            {
                _fadeTextImageTweener = TextImage.FadeOut(_asset.FadeTargetAlpha, _asset.Duration, _asset.DelayTime);
                if (_fadeTextImageTweener != null)
                {
                    _fadeTextImageTweener.onComplete = () => { _fadeTextImageTweener = null; };
                }
            }
        }

        private void StopFadeTweeners()
        {
            if (_fadeTextTweener != null)
            {
                _fadeTextTweener.Kill();
                _fadeTextTweener = null;
            }

            if (_fadeTextImageTweener != null)
            {
                _fadeTextImageTweener.Kill();
                _fadeTextImageTweener = null;
            }
        }

        #endregion Fade

        #region Scale

        private void StartPunchScale()
        {
            if (_asset == null || !_asset.UsePunchScale || _asset.PunchScaleDuration <= 0)
            {
                return;
            }

            _scaleTweener = Text.transform.DOPunchScale(_asset.PunchScale, _asset.PunchScaleDuration, _asset.PunchScaleVibrato)
                .SetEase(_asset.PunchEase);
            if (_scaleTweener != null)
            {
                _scaleTweener.onComplete += OnCompletedPunchScale;
            }
        }

        private void OnCompletedPunchScale()
        {
            _scaleTweener = null;
        }

        private void StopPunchScale()
        {
            if (_scaleTweener != null)
            {
                _scaleTweener.Kill();
                _scaleTweener = null;
            }
        }

        #endregion Scale
    }
}