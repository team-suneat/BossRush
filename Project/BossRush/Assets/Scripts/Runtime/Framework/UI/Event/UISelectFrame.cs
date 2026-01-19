using System.Collections;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

namespace TeamSuneat.UserInterface
{
    public class UISelectFrame : XBehaviour, IPoolable
    {
        [SerializeField] private Image _frameImage;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            _frameImage = GetComponentInChildren<Image>();
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
        }

        public void Despawn()
        {
            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject);
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();
            Despawn();
        }

        public void Show()
        {
            if (_frameImage != null)
            {
                _frameImage.SetAlpha(1);
            }
        }

        public void Hide()
        {
            if (_frameImage != null)
            {
                _frameImage.SetAlpha(0);
            }
        }

        public void AttachTo(Transform target, Vector2 sizeDelta, Vector3 offset, Transform parent = null)
        {
            if (target == null)
            {
                return;
            }

            transform.SetParent(target);
            rectTransform.sizeDelta = sizeDelta;
            transform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector2.zero;
            rectTransform.anchoredPosition3D += offset;

            if (parent != null)
            {
                transform.SetParent(parent);
            }

            Show();
        }

        public Coroutine AttachToAsync(MonoBehaviour coroutineOwner, Transform target, Vector2 sizeDelta, Vector3 offset, Transform parent = null)
        {
            if (coroutineOwner == null)
            {
                Log.Warning(LogTags.UI_SelectEvent, "AttachToAsync: coroutineOwner가 null입니다.");
                AttachTo(target, sizeDelta, offset, parent);
                return null;
            }

            return coroutineOwner.StartCoroutine(AttachToCoroutine(target, sizeDelta, offset, parent));
        }

        private IEnumerator AttachToCoroutine(Transform target, Vector2 sizeDelta, Vector3 offset, Transform parent)
        {
            yield return null;

            AttachTo(target, sizeDelta, offset, parent);
        }
    }
}