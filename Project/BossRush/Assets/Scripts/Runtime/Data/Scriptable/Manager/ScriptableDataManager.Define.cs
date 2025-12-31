namespace TeamSuneat.Data
{
    public partial class ScriptableDataManager
    {
        public GameDefineAsset GetGameDefine()
        {
            return _gameDefine;
        }

        private bool LoadGameDefineSync(string filePath)
        {
            if (!filePath.Contains("GameDefine"))
            {
                return false;
            }

            _gameDefine = ResourcesManager.LoadResource<GameDefineAsset>(filePath);
            if (_gameDefine != null)
            {
                Log.Progress("스크립터블 데이터를 읽어왔습니다. Path: {0}", filePath);
                return true;
            }

            return false;
        }
    }
}