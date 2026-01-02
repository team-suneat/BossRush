using Sirenix.OdinInspector;

using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileRenderer : XBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
        private Projectile projectile;

        public string SortingLayerName = "Projectile";
        public bool UseFlipX;
        public bool UseFlipY;
        public bool UseFlipByDynimic;
        public bool UseMultipleMaterialByCore;

        [InfoBox("발사체의 Rotation이 Identity일 때 발사체 랜더러의 Rotation도 초기화시켜준다.")]
        public bool UseInitQuaternion;
        private Quaternion _defaultQuaternion;

        private int _defaultSortingOrder;

#if UNITY_EDITOR

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            projectile = this.FindFirstParentComponent<Projectile>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();
            gameObject.SetLayer(GameLayers.Projectiles);
            if (false == string.IsNullOrEmpty(SortingLayerName))
            {
                _spriteRenderer.SetSortingLayer(SortingLayerName);
            }
        }

        public override void AutoNaming()
        {
            if (projectile != null)
            {
                string gameObjectName = string.Format("Renderer ({0})", projectile.Name);
                SetGameObjectName(gameObjectName);
            }
        }

#endif

        protected virtual void Awake()
        {
            _defaultQuaternion = transform.localRotation;

            _defaultSortingOrder = _spriteRenderer.sortingOrder;

            projectile = this.FindFirstParentComponent<Projectile>();
        }

        public void ResetQuaternion(Quaternion parentRotation)
        {
            if (UseInitQuaternion)
            {
                if (parentRotation == Quaternion.identity)
                {
                    transform.rotation = Quaternion.identity;
                }
                else
                {
                    transform.localRotation = _defaultQuaternion;
                }
            }
        }

        public void SetOrder(int order)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.sortingOrder = _defaultSortingOrder * 100 + order;
            }
        }

        public void SetFlip(bool facingRight)
        {
            if (facingRight)
            {
                if (UseFlipX)
                {
                    _spriteRenderer.flipX = false;
                }
                if (UseFlipY)
                {
                    _spriteRenderer.flipY = false;
                }
            }
            else
            {
                if (UseFlipX)
                {
                    _spriteRenderer.flipX = true;
                }
                if (UseFlipY)
                {
                    _spriteRenderer.flipY = true;
                }
            }
        }

        public void SetFlipByDynimic(float x)
        {
            if (Mathf.Abs(x) < 0.1f)
            {
                return;
            }

            if (x > 0)
            {
                _spriteRenderer.flipX = true;
            }
            else if (x < 0)
            {
                _spriteRenderer.flipX = false;
            }
        }
    }
}