using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public class HUDManager : XBehaviour
    {
        [FoldoutGroup("HUD-Normal")]
        [SerializeField] private GameObject _normalStageGroup;

        [FoldoutGroup("HUD-Normal")]
        [SerializeField] private UICanvasGroupFader _hudCanvasGroupFader;

        [FoldoutGroup("HUD-Player")]
        [SerializeField] private HUDPlayer _playerHUD;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _hudCanvasGroupFader ??= GetComponentInChildren<UICanvasGroupFader>();
            _normalStageGroup ??= this.FindGameObject("2. Center Group/Normal Stage Group");
            _playerHUD ??= GetComponentInChildren<HUDPlayer>();
        }

        private void Awake()
        {
            SubscribeToPlayerEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromPlayerEvents();
        }

        private void SubscribeToPlayerEvents()
        {
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerBattleReady);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawned);
            GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_DEATH, OnPlayerDeath);
        }

        private void UnsubscribeFromPlayerEvents()
        {
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerBattleReady);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DESPAWNED, OnPlayerDespawned);
            GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_DEATH, OnPlayerDeath);
        }

        private void OnPlayerBattleReady()
        {
            PlayerCharacter player = CharacterManager.Instance?.Player;
            if (_playerHUD != null && player != null)
            {
                _playerHUD.Bind(player);
            }
        }

        private void OnPlayerDespawned()
        {
            if (_playerHUD != null)
            {
                _playerHUD.Unbind();
            }
        }

        private void OnPlayerDeath()
        {
            if (_playerHUD != null)
            {
                _playerHUD.Unbind();
            }
        }

        public void OnBossDied()
        {
        }
    }
}