namespace TeamSuneat.UserInterface
{
    public class HUDBossButton : UIButton
    {
        protected override void OnClickSucceeded()
        {
            base.OnClickSucceeded();

            if (GameApp.Instance?.gameManager?.CurrentStageSystem != null)
            {
                GameApp.Instance.gameManager.CurrentStageSystem.EnterBossMode();
            }
        }
    }
}