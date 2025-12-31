namespace TeamSuneat
{
    public class LightEffect2DGroup : XBehaviour
    {
        public LightEffect2D[] LightEffects;
        public float FadeOutDuration = 0.5f;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            LightEffects = GetComponentsInChildren<LightEffect2D>();
        }

        public void FadeOutAll()
        {
            for (int i = 0; i < LightEffects.Length; i++)
            {
                LightEffects[i].FadeOut(FadeOutDuration);
            }
        }

        public void Activate()
        {
            for (int i = 0; i < LightEffects.Length; i++)
            {
                LightEffects[i].Activate();
            }
        }
    }
}