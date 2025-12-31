using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class ParticleEffect2D : XBehaviour
    {
        [Title("Particle Effect 2D")]
        public ParticleSystem ParticleSystem;

        public ParticleSystemRenderer ParticleSystemRenderer;

        [SuffixLabel("스프라이트 랜더러 사용")]
        public bool UseSpriteRenderer;

        [SuffixLabel("설정된 방향으로 Scale X값 적용")]
        public bool UseScale;

        [SuffixLabel("설정된 방향으로 Shape의 Scale X값 적용")]
        public bool UseShape;

        [SuffixLabel("설정된 방향으로 Flip")]
        public bool UseFlip;

        [SuffixLabel("설정된 방향의 반대방향으로 Flip")]
        public bool UseReverseFlip;

        [SuffixLabel("충돌체의 무작위 크기 사용")]
        public bool UseRandomCollisionScale;

        public float CollisionMinScale;
        public float CollisionMaxScale;
        public int PixelPerUnit;
        public int SpriteWidth;

#if UNITY_EDITOR

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            ParticleSystem = GetComponentInChildren<ParticleSystem>();
            ParticleSystemRenderer = GetComponentInChildren<ParticleSystemRenderer>();
        }

#endif

        public void Play()
        {
            if (ParticleSystem != null)
            {
                SetRandomCollisionScale();
                ParticleSystem.Play(true);
            }
        }

        public void Stop()
        {
            if (ParticleSystem != null)
            {
                ParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        public void SetDirection(bool isFacingRight)
        {
            SetScaleByDirection(isFacingRight);
            SetShapeByDirection(isFacingRight);
            SetFlipByDirection(isFacingRight);
            SetReverseFlipByDirection(isFacingRight);
        }

        private void SetScaleByDirection(bool isFacingRight)
        {
            if (UseScale)
            {
                transform.localScale = isFacingRight
                    ? new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z)
                    : new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        private void SetShapeByDirection(bool isFacingRight)
        {
            if (UseShape)
            {
                var shapeModule = ParticleSystem.shape;
                shapeModule.scale = isFacingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1);
            }
        }

        private void SetFlipByDirection(bool isFacingRight)
        {
            if (UseFlip)
            {
                ParticleSystemRenderer.flip = isFacingRight ? new Vector3(1, 0, 0) : new Vector3(-1, 0, 0);
            }
        }

        private void SetReverseFlipByDirection(bool isFacingRight)
        {
            if (UseReverseFlip)
            {
                ParticleSystemRenderer.flip = isFacingRight ? new Vector3(-1, 0, 0) : new Vector3(1, 0, 0);
            }
        }

        public void SetRandomCollisionScale()
        {
            if (UseRandomCollisionScale)
            {
                float scale = GetRandomCollisionScale();
                ApplyCollisionScale(scale);
            }
        }

        private float GetRandomCollisionScale()
        {
            return RandomEx.Range(CollisionMinScale, CollisionMaxScale);
        }

        private void ApplyCollisionScale(float scale)
        {
            ParticleSystem.CollisionModule myCollisionModule = ParticleSystem.collision;
            myCollisionModule.radiusScale = scale;
        }

        public void SetSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            if (UseSpriteRenderer)
            {
                if (ParticleSystem != null)
                {
                    ParticleSystem.ShapeModule shape = ParticleSystem.shape;
                    shape.spriteRenderer = spriteRenderer;
                }
            }
        }

        public void SetLoop(bool isLoop)
        {
            var particleMain = ParticleSystem.main;
            particleMain.loop = isLoop;
        }

        public void SetStartColor(Color color)
        {
            var particleMain = ParticleSystem.main;
            particleMain.startColor = color;
        }

        public void SetStartSizeWithPixelPerUnit()
        {
            // 이미지 한장의 크기를 픽셀 퍼 유닛으로 나누어 사이즈를 설정합니다.
            float startSize = SpriteWidth.SafeDivide(PixelPerUnit);
            ParticleSystem.MainModule main = ParticleSystem.main;
            main.startSize = new ParticleSystem.MinMaxCurve(startSize);
        }
    }
}