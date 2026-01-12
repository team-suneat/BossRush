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
    }
}