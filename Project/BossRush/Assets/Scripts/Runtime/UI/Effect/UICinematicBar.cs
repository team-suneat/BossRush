using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using TeamSuneat.UserInterface;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat
{
    public class UICinematicBar : XBehaviour
    {
        [Title("#UI Cinematic Bar")]
        public Image TopImage;
        public Image BottomImage;
        public UIShortcutElement ShortcutElement;

        [Range(0.1f, 0.5f)]
        public float HeightRatioToScreen = 0.1f;
        public float TweenDuration = 1f;

        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerTopShow;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerBottomShow;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerTopHide;
        private TweenerCore<Vector3, Vector3, VectorOptions> _tweenerBottomHide;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            TopImage ??= this.FindComponent<Image>("Top Image");
            BottomImage ??= this.FindComponent<Image>("Bottom Image");
            ShortcutElement ??= GetComponentInChildren<UIShortcutElement>(true);
        }

        protected override void OnStart()
        {
            base.OnStart();

            Deactivate();
        }

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            GlobalEvent.Register(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            GlobalEvent.Unregister(GlobalEventType.MOVE_TO_STAGE, OnMoveToStage);
        }

        private void OnMoveToStage()
        {
            Deactivate();
        }

        private void Deactivate()
        {
            StopShowTweener();
            StopHideTweener();

            ShortcutElement?.SetActive(false);
        }

        //

        public void Show(bool ignoreSkip = false)
        {
            Show(HeightRatioToScreen, ignoreSkip);
        }

        public void Show(float heightRatioToScreen, bool ignoreSkip = false)
        {
            Log.Info(LogTags.Timeline, "타임라인의 시네마틱 바를 표시합니다.");

            StopHideTweener();
            StartShowTweener(heightRatioToScreen);

            if (ShortcutElement != null)
            {
                if (!ignoreSkip)
                {
                    ShortcutElement.SetActive(true);
                }
            }
        }

        public void Hide()
        {
            Log.Info(LogTags.Timeline, "타임라인의 시네마틱 바를 숨깁니다.");

            StopShowTweener();
            StartHideTweener();

            ShortcutElement?.SetActive(false);
        }

        //

        private void StopShowTweener()
        {
            KillTween(ref _tweenerTopShow);
            KillTween(ref _tweenerBottomShow);
        }

        private void StopHideTweener()
        {
            KillTween(ref _tweenerTopHide);
            KillTween(ref _tweenerBottomHide);
        }

        private void KillTween(ref TweenerCore<Vector3, Vector3, VectorOptions> tweener)
        {
            if (tweener != null)
            {
                tweener.Kill();
                tweener = null;
            }
        }

        //

        private void StartShowTweener(float heightRatioToScreen)
        {
            StartTopShowTween(heightRatioToScreen);
            StartBottomShowTween(heightRatioToScreen);
        }

        private void StartHideTweener()
        {
            StartTopHideTween();
            StartBottomHideTween();
        }

        private void StartTopShowTween(float heightRatioToScreen)
        {
            if (_tweenerTopShow == null && TopImage != null)
            {
                _tweenerTopShow = TopImage.rectTransform.DOScaleY(heightRatioToScreen, TweenDuration);
                _tweenerTopShow.onComplete += () => { _tweenerTopShow = null; };
            }
        }

        private void StartBottomShowTween(float heightRatioToScreen)
        {
            if (_tweenerBottomShow == null && BottomImage != null)
            {
                _tweenerBottomShow = BottomImage.rectTransform.DOScaleY(heightRatioToScreen, TweenDuration);
                _tweenerBottomShow.onComplete += () => { _tweenerBottomShow = null; };
            }
        }

        private void StartTopHideTween()
        {
            if (_tweenerTopHide == null && TopImage != null)
            {
                _tweenerTopHide = TopImage.rectTransform.DOScaleY(0f, TweenDuration);
                _tweenerTopHide.onComplete += () => { _tweenerTopHide = null; };
            }
        }

        private void StartBottomHideTween()
        {
            if (_tweenerBottomHide == null && BottomImage != null)
            {
                _tweenerBottomHide = BottomImage.rectTransform.DOScaleY(0f, TweenDuration);
                _tweenerBottomHide.onComplete += () => { _tweenerBottomHide = null; };
            }
        }
    }
}