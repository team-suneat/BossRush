using TeamSuneat.Audio;

using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileGuideLine : XBehaviour
    {
        public Character Owner;
        public Transform Point;
        public Transform Line;
        public Animator Animator;
        public float CenterPointerX;

        public SoundNames ShowSoundName;
        public string ShowSoundNameString;

        public Vector3 Direction { get; private set; }

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();
            Owner = this.FindFirstParentComponent<Character>();
            Point = this.FindComponent<Transform>("#Point");
            Line = this.FindComponent<Transform>("#Point/Line Renderer");
            Animator = this.FindComponent<Animator>("#Point/Line Renderer");
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (ShowSoundName != SoundNames.None)
            {
                ShowSoundNameString = ShowSoundName.ToString();
            }
        }

        private void OnValidate()
        {
            EnumEx.ConvertTo(ref ShowSoundName, ShowSoundNameString);
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (Point != null)
            {
                Point.gameObject.SetActive(false);
            }
        }

        public void SetDirection(Vector3 direction)
        {
            Direction = direction;
        }

        public void SetDirectionToPlayer()
        {
            if (CharacterManager.Instance.Player != null)
            {
                Direction = CharacterManager.Instance.Player.transform.position - transform.position;
            }
        }

        public void Show()
        {
            Log.Info(LogTags.Projectile, "발사체 가이드 라인이 생성됩니다.");

            ShowGuideLine();
            PlayAnimation();
            PlaySFX();
        }

        private void ShowGuideLine()
        {
            if (Point != null)
            {
                if (Owner != null)
                {
                    if (Owner.IsFacingRight)
                    {
                        Point.transform.right = Direction;
                    }
                    else
                    {
                        Point.transform.right = -Direction;
                    }
                }
                else
                {
                    Point.transform.right = Direction;
                }

                Point.gameObject.SetActive(true);
            }
        }

        private void PlayAnimation()
        {
            if (Animator != null)
            {
                Animator.Play("Start", -1, 0);
            }
        }

        private void PlaySFX()
        {
            if (ShowSoundName != SoundNames.None)
            {
                AudioManager.Instance.PlaySFXOneShotScaled(ShowSoundName, position);
            }
        }
    }
}