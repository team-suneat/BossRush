using Lean.Pool;
using System;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    // Notice UI 기본 클래스 - 공통 기능 제공
    public abstract class UINoticeBase : XBehaviour, IPoolable
    {
        [SerializeField]
        protected UILocalizedText _titleText;

        [SerializeField]
        protected UICanvasGroupFader _fader;

        public event Action OnCompleted;

        public void OnSpawn()
        { }

        public void OnDespawn()
        { }

        public void Despawn()
        {
            ResourcesManager.Despawn(gameObject);
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _titleText ??= GetComponentInChildren<UILocalizedText>(true);
            _fader ??= GetComponentInChildren<UICanvasGroupFader>(true);
        }

        public virtual void SetContent(string content)
        {
            if (_titleText != null)
            {
                _titleText.SetText(content);
            }
        }

        public virtual void SetStringKey(string stringKey)
        {
            if (_titleText != null)
            {
                _titleText.SetStringKey(stringKey);
            }
        }

        public void Show()
        {
            if (_fader != null)
            {
                _fader.KillFade();
                _fader.SetCompletedCallback(OnFadeOutComplete);
                _fader.FadeInOut();
            }
            else
            {
                OnFadeOutComplete();
            }
        }

        protected virtual void OnFadeOutComplete()
        {
            _fader?.SetCompletedCallback(null);
            OnCompleted?.Invoke();
            Despawn();
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            _fader?.KillFade();
        }
    }
}