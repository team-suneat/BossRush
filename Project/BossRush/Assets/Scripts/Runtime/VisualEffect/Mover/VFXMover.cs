using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class VFXMover : MonoBehaviour
    {
        public bool RotateDirection;
        public float Duration;
        public bool UseEaseBoth;

        [EnableIf("UseEaseBoth")]
        public Ease MoveEase;

        [DisableIf("UseEaseBoth")]
        public Ease MoveEaseX;

        [DisableIf("UseEaseBoth")]
        public Ease MoveEaseY;

        private Vector3 _originPosition;
        private Vector3 _targetPosition;

        private Tweener _tweener;
        private Tweener _tweenerX;
        private Tweener _tweenerY;

        private void OnDisable()
        {
            OnCompletedTweener();
            OnCompletedTweenerX();
            OnCompletedTweenerY();
        }

        public void SetOriginPosition(Vector3 originPosition)
        {
            _originPosition = originPosition;
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public void StartMove()
        {
            transform.position = _originPosition;

            if (UseEaseBoth)
            {
                if (_tweener == null)
                {
                    _tweener = transform.DOMove(_targetPosition, Duration).SetEase(MoveEase);
                    _tweener.onComplete += OnCompletedTweener;
                }
            }
            else
            {
                if (_tweenerX == null)
                {
                    _tweenerX = transform.DOMoveX(_targetPosition.x, Duration).SetEase(MoveEaseX);
                    _tweenerX.onComplete += OnCompletedTweenerX;
                }
                if (_tweenerY == null)
                {
                    _tweenerY = transform.DOMoveY(_targetPosition.y, Duration).SetEase(MoveEaseY);
                    _tweenerY.onComplete += OnCompletedTweenerY;
                }
            }
        }

        private void OnCompletedTweener()
        {
            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }
        }

        private void OnCompletedTweenerX()
        {
            if (_tweenerX != null)
            {
                _tweenerX.Kill();
                _tweenerX = null;
            }
        }

        private void OnCompletedTweenerY()
        {
            if (_tweenerY != null)
            {
                _tweenerY.Kill();
                _tweenerY = null;
            }
        }
    }
}