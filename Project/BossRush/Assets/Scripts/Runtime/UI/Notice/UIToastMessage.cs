using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UIToastMessage : XBehaviour, IPoolable
    {
        private readonly float _punchScaleValue = 0.05f;
        private Tweener _scaleTweener;
        private UnityAction<UIToastMessage> _onDespawnEvent;

        public string Content { get; private set; }
        [field: SerializeField]
        public UICanvasGroupFader CanvasGroupFader { get; private set; }
        [field: SerializeField]
        public Image BackgroundImage { get; private set; }
        [field: SerializeField]
        public UILocalizedText NoticeText { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            CanvasGroupFader ??= GetComponent<UICanvasGroupFader>();
            BackgroundImage ??= GetComponentInChildren<Image>();

            NoticeText ??= this.FindComponent<UILocalizedText>("Content/Notice Text");
        }

        public void OnSpawn()
        {
            transform.localScale = Vector3.one;
            _scaleTweener = null;
            _onDespawnEvent = null;
        }

        public void OnDespawn()
        {
            _scaleTweener?.Kill();
            _scaleTweener = null;
            _onDespawnEvent = null;
        }

        private void OnDisable()
        {
            CanvasGroupFader?.KillFade();
            StopPunchScale();
        }

        public void Despawn()
        {
            CallDespawnEvent();

            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject);
            }
        }

        public void Setup(string content, float duration = 2f)
        {
            Content = content;

            SetText();
            FadeOut(duration);
        }

        public void ActivateBackground()
        {
            BackgroundImage?.SetActive(true);
        }

        public void DeactivateBackground()
        {
            BackgroundImage?.SetActive(false);
        }

        private void SetText()
        {
            if (NoticeText != null)
            {
                NoticeText.SetText(Content);
                _ = CoroutineNextFrame(StartPunchScale);
            }
        }

        public void Restart()
        {
            FadeOut(0);
            StopPunchScale();
            StartPunchScale();

            transform.SetAsLastSibling();
        }

        private void FadeOut(float duration)
        {
            if (CanvasGroupFader != null)
            {
                CanvasGroupFader.KillFade();

                if (!duration.IsZero())
                {
                    CanvasGroupFader.FadeOutDelayTime = duration;
                }

                CanvasGroupFader.FadeOut(Despawn);
            }
        }

        private void StartPunchScale()
        {
            if (_scaleTweener != null)
            {
                return;
            }

            transform.localScale = Vector3.one;

            Vector3 punch = Vector3.one * _punchScaleValue;
            float duration = 0.2f;

            _scaleTweener = transform.DOPunchScale(punch, duration);
            _ = _scaleTweener.SetUpdate(true);
            _ = _scaleTweener.SetEase(Ease.OutQuad);
            _ = _scaleTweener.OnComplete(OnCompletedPunchScale);
        }

        private void OnCompletedPunchScale()
        {
            transform.localScale = Vector3.one;
            _scaleTweener = null;
        }

        private void StopPunchScale()
        {
            if (_scaleTweener != null)
            {
                transform.localScale = Vector3.one;

                _scaleTweener.Kill();
                _scaleTweener = null;
            }
        }

        public void RegisterDespawnEvent(UnityAction<UIToastMessage> onDespawn)
        {
            if (onDespawn == null)
            {
                return;
            }

            _onDespawnEvent = onDespawn;
        }

        private void CallDespawnEvent()
        {
            _onDespawnEvent?.Invoke(this);
            _onDespawnEvent = null;
        }
    }
}