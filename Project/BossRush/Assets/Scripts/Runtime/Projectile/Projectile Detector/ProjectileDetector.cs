using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileDetector : XBehaviour
    {
        public Projectile Projectile;

        public BoxCollider2D TriggerCollider;

        public Rigidbody2D TriggerRigidbody2D;

        public SpriteRenderer TriggerRenderer;

        public bool UseDetectLayerMask;

        [ShowIf("UseDetectLayerMask")]
        public GameTags TargetTag;

        [ShowIf("UseDetectLayerMask")]
        public LayerMask TargetLayerMask;

        public bool UseDetectCharacter;

        [ShowIf("UseDetectCharacter")]
        public CharacterTypes[] DetectCharacterTypes;

        [ShowIf("UseDetectCharacter")]
        public bool UseAliveCharacter;

        public bool UseDetectProjectile;

        [ShowIf("UseDetectProjectile")]
        public ProjectileNames[] DetectProjectiles;

        public bool DefaultLockOnSpawn;

        public bool UseOnce;

        public bool UseAttackOnEnter;

        public bool UseAnotherOnEnter;

        public bool UseStopMoveOnEnter;

        public bool UseDestroyOnEnter;

        public bool UseLockDetectOnEnter;

        public bool UseDespawnOnEnter;

        public override void AutoSetting()
        {
            base.AutoSetting();

            gameObject.SetLayer(GameLayers.Detectable);

            if (TriggerCollider != null)
            {
                TriggerCollider.isTrigger = true;

                if (TriggerRenderer != null)
                {
                    if (TriggerCollider.enabled)
                    {
                        TriggerRenderer.size = TriggerCollider.size;
                    }
                }
            }
        }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Projectile = this.FindFirstParentComponent<Projectile>();

            TriggerCollider = GetComponent<BoxCollider2D>();

            TriggerRigidbody2D = GetComponent<Rigidbody2D>();

            TriggerRenderer = GetComponent<SpriteRenderer>();
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();

            if (TriggerCollider == null)
            {
                TriggerCollider = gameObject.AddComponent<BoxCollider2D>();

                TriggerCollider.isTrigger = true;
            }

            if (TriggerRigidbody2D == null)
            {
                TriggerRigidbody2D = gameObject.AddComponent<Rigidbody2D>();

                TriggerRigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        protected virtual void Awake()
        {
            

            if (Projectile == null)
            {
                Projectile = this.FindFirstParentComponent<Projectile>();
            }

            if (TriggerCollider == null)
            {
                TriggerCollider = GetComponent<BoxCollider2D>();
            }

            if (TriggerRigidbody2D == null)
            {
                TriggerRigidbody2D = GetComponent<Rigidbody2D>();
            }

            if (TriggerRenderer == null)
            {
                TriggerRenderer = GetComponent<SpriteRenderer>();
            }

            if (TriggerRenderer != null)
            {
                TriggerRenderer.enabled = false;
            }
        }

        public void OnSpawn()
        {
            if (DefaultLockOnSpawn)
            {
                Lock();
            }
            else
            {
                Unlock();
            }
        }

        public void OnDespawn()
        {
            Lock();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (CheckTrigger(collision))
            {
                if (UseAttackOnEnter)
                {
                    Projectile.StartAttackByDetector();
                }

                if (UseAnotherOnEnter)
                {
                    Projectile.DoAnotherAttack();
                }

                if (UseStopMoveOnEnter)
                {
                    Projectile.StopMove();
                }

                if (UseDestroyOnEnter)
                {
                    Projectile.Destroy();
                }

                if (UseLockDetectOnEnter)
                {
                    Projectile.LockDetector();
                }

                if (UseDespawnOnEnter)
                {
                    Projectile.Despawn();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (CheckTrigger(collision))
            {
                if (UseOnce)
                {
                    Lock();
                }
            }
        }

        private bool CheckTrigger(Collider2D collision)
        {
            if (CompareCharacter(collision))
            {
                return true;
            }

            if (CompareProjectile(collision))
            {
                return true;
            }

            if (CompareCollision(collision))
            {
                return true;
            }

            return false;
        }

        private bool CompareCollision(Collider2D collision)
        {
            if (UseDetectLayerMask)
            {
                if (collision.CompareTag(TargetTag))
                {
                    if (LayerEx.IsInMask(collision.gameObject.layer, TargetLayerMask))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CompareCharacter(Collider2D collision)
        {
            if (false == UseDetectCharacter)
            {
                return false;
            }

            Character character = CharacterManager.Instance.Find(collision);

            if (character == null)
            {
                return false;
            }

            if (UseAliveCharacter && character.MyVital.IsAlive)
            {
                return false;
            }

            if (DetectCharacterTypes == null)
            {
                return false;
            }

            for (int i = 0; i < DetectCharacterTypes.Length; i++)
            {
                if (DetectCharacterTypes[i] == CharacterTypes.None)
                {
                    return true;
                }

                if (DetectCharacterTypes[i] == character.Type)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CompareProjectile(Collider2D collision)
        {
            if (UseDetectProjectile)
            {
                Projectile projectile = ProjectileManager.Instance.Find(collision);

                if (projectile != null)
                {
                    if (DetectProjectiles != null)
                    {
                        for (int i = 0; i < DetectProjectiles.Length; i++)
                        {
                            if (DetectProjectiles[i] == projectile.Name)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public void Lock()
        {
            if (TriggerCollider != null)
            {
                TriggerCollider.enabled = false;
            }
        }

        public void Unlock()
        {
            if (TriggerCollider != null)
            {
                TriggerCollider.enabled = true;
            }
        }
    }
}