using Lean.Pool;
using TeamSuneat.Assets.Scripts.Runtime.Transform;
using UnityEngine;

namespace TeamSuneat
{
    public partial class Projectile
    {
        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Vital = GetComponentInChildren<Vital>();
            Animator = GetComponentInChildren<ProjectileAnimator>();
            Renderer = GetComponentInChildren<ProjectileRenderer>();
            Detectors = GetComponentsInChildren<ProjectileDetector>();
            RotationController = GetComponentInChildren<ProjectileRotationController>();
            FlipController = GetComponentInChildren<FlipController>();
            AttackSystem = GetComponentInChildren<ProjectileAttackSystem>();
            FXController = GetComponentInChildren<ProjectileFXController>();
            Body = GetComponent<XRigidbody>();
            m_collider2D = GetComponent<Collider2D>();
            m_boxCollider2D = GetComponent<BoxCollider2D>();
            m_circleCollider2D = GetComponent<CircleCollider2D>();
        }

        public override void AutoAddComponents()
        {
            base.AutoAddComponents();

            LoadProjectileData();

            AddPhysicsComponent();

            if (AttackSystem == null)
            {
                GameObject newGameObject = GameObjectEx.CreateGameObject("Attack", transform);
                AttackSystem = newGameObject.AddComponent<ProjectileAttackSystem>();
            }

            GameObject rendererGameObject;
            if (Renderer == null)
            {
                rendererGameObject = GameObjectEx.CreateGameObject(transform);
            }
            else
            {
                rendererGameObject = Renderer.gameObject;
            }

            if (Animator == null)
            {
                Animator = rendererGameObject.AddComponent<ProjectileAnimator>();
            }

            if (Renderer == null)
            {
                Renderer = rendererGameObject.AddComponent<ProjectileRenderer>();
            }
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            LoadProjectileData();

            SetLayers();

            gameObject.SetLayer(GameLayers.Projectiles);
        }

        private void AddPhysicsComponent()
        {
            if (_projectileData == null || _projectileData.Name == ProjectileNames.None)
            {
                return;
            }

            if (_projectileData.MotionType == ProjectileMotionTypes.Physics)
            {
                if (Body == null)
                {
                    Body = gameObject.AddComponent<XRigidbody>();
                    Body.AutoGetComponents();

                    Body.SetBodyType(RigidbodyType2D.Dynamic);
                    Body.SetGravity(GameDefine.DEFAULT_PHYSICS_DEFAULT_GRAVITY_IN_RIGIDBODY * _projectileData.GravityRate);
                    Body.SetFreezeRotation(true);
                }

                if (m_collider2D == null)
                {
                    m_boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
                    m_boxCollider2D.isTrigger = false;
                }
            }
            else if (_projectileData.IsAttackOnHit)
            {
                if (Body == null)
                {
                    Body = gameObject.AddComponent<XRigidbody>();
                    Body.SetBodyType(RigidbodyType2D.Kinematic);
                    Body.ResetGravity();
                    Body.SetFreezeRotation(true);
                }

                if (m_collider2D == null)
                {
                    m_boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
                    m_boxCollider2D.isTrigger = false;
                }
            }
        }

        public override void AutoNaming()
        {
            base.AutoNaming();

            LoadProjectileData();

            if (_projectileData != null)
            {
                SetGameObjectName(_projectileData.PrefabName);
            }
        }

        protected virtual void Awake()
        {
            

            GetComponents();

            LoadProjectileData();

            if (AttackSystem != null)
            {
                AttackSystem.Caching();
            }

            SetLayers();

            if (Vital != null && _projectileData != null)
            {
                if (_projectileData.DamageResult != ProjectileResults.None)
                {
                    Vital.dieEvent.AddListener(DestroyByAttack);
                }
            }
        }

        private void GetComponents()
        {
            if (Vital == null)
            {
                Vital = GetComponentInChildren<Vital>();
            }

            if (Animator == null)
            {
                Animator = GetComponentInChildren<ProjectileAnimator>();
            }

            if (Renderer == null)
            {
                Renderer = GetComponentInChildren<ProjectileRenderer>();
            }

            if (Detectors == null)
            {
                Detectors = GetComponentsInChildren<ProjectileDetector>();
            }

            if (RotationController == null)
            {
                RotationController = GetComponentInChildren<ProjectileRotationController>();
            }

            if (FlipController == null)
            {
                FlipController = GetComponentInChildren<FlipController>();
            }

            if (AttackSystem == null)
            {
                AttackSystem = GetComponentInChildren<ProjectileAttackSystem>();
            }

            if (FXController == null)
            {
                FXController = GetComponentInChildren<ProjectileFXController>();
            }

            if (Body == null)
            {
                Body = GetComponent<XRigidbody>();
            }

            if (m_collider2D == null)
            {
                m_collider2D = GetComponent<Collider2D>();
            }

            if (m_boxCollider2D == null)
            {
                m_boxCollider2D = GetComponent<BoxCollider2D>();
            }

            if (m_circleCollider2D == null)
            {
                m_circleCollider2D = GetComponent<CircleCollider2D>();
            }
        }
    }
}