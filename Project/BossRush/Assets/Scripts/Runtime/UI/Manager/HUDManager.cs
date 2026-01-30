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

        [FoldoutGroup("HUD-Boss")]
        [SerializeField] private HUDBossGauge _bossHUD;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            _hudCanvasGroupFader ??= GetComponentInChildren<UICanvasGroupFader>();
            _normalStageGroup ??= this.FindGameObject("2. Center Group/Normal Stage Group");
            _playerHUD ??= GetComponentInChildren<HUDPlayer>();
            _bossHUD ??= GetComponentInChildren<HUDBossGauge>();
        }

        private void Awake()
        {
            SubscribeToPlayerEvents();
            SubscribeToBossEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromPlayerEvents();
            UnsubscribeFromBossEvents();
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

        private void SubscribeToBossEvents()
        {
            GlobalEvent<BossCharacter>.Register(GlobalEventType.BOSS_CHARACTER_BATTLE_READY, OnBossBattleReady);
            GlobalEvent<BossCharacter>.Register(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
        }

        private void UnsubscribeFromBossEvents()
        {
            GlobalEvent<BossCharacter>.Unregister(GlobalEventType.BOSS_CHARACTER_BATTLE_READY, OnBossBattleReady);
            GlobalEvent<BossCharacter>.Unregister(GlobalEventType.BOSS_CHARACTER_DEATH, OnBossDeath);
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

        private void OnBossBattleReady(BossCharacter boss)
        {
            if (_bossHUD != null && boss != null)
            {
                _bossHUD.Bind(boss);
            }
        }

        private void OnBossDeath(BossCharacter boss)
        {
            if (_bossHUD != null)
            {
                _bossHUD.Unbind();
            }
        }
    }
}