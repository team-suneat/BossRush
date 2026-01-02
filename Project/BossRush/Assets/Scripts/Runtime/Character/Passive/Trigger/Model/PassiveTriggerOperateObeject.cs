using TeamSuneat.Data;
using TeamSuneat.Stage;


namespace TeamSuneat.Passive
{
    public class PassiveTriggerOperateObeject : PassiveTriggerReceiver
    {
        private MapObjectNames _mapObjectName;

        private MapTypes _currentMapType;

        protected override PassiveTriggers Trigger => PassiveTriggers.PlayerOperateMapObject;

        protected override void Register()
        {
            base.Register();

            GlobalEvent<MapObjectNames>.Register(GlobalEventType.PLAYER_CHARACTER_OPERATE_MAP_OBJECT, OnGlobalEvent);
        }

        protected override void Unregister()
        {
            base.Unregister();

            GlobalEvent<MapObjectNames>.Unregister(GlobalEventType.PLAYER_CHARACTER_OPERATE_MAP_OBJECT, OnGlobalEvent);
        }

        private void OnGlobalEvent(MapObjectNames name)
        {
            _mapObjectName = name;

            StageNames stageName = GameApp.GetSelectedProfile().Stage.CurrentStage;
            StageAsset stageAsset = ScriptableDataManager.Instance.FindStage(stageName);
            if (stageAsset.IsValid())
            {
                _currentMapType = stageAsset.Data.Type;
            }

            if (TryExecute())
            {
                Execute();
            }
        }

        public override bool TryExecute()
        {
            if (!base.TryExecute())
            {
                return false;
            }
            else if (!Checker.CheckTriggerMapObject(_mapObjectName))
            {
                return false;
            }
            else if (!Checker.CheckTriggerMapType(_currentMapType))
            {
                return false;
            }

            return CheckConditions();
        }
    }
}