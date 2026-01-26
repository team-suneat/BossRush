using Newtonsoft.Json;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    public partial class GameDataManager
    {
        private string SerializeObject(GameData gameData)
        {
            try
            {
                return JsonConvert.SerializeObject(gameData, Formatting.Indented, _serializeSettings);
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 직렬화할 수 없습니다. \nException Massage:{0}", ex.Message.ToString());

                return null;
            }
        }

        private GameData Deserialize(string chunk)
        {
            try
            {
                if (!string.IsNullOrEmpty(chunk))
                {
                    // 마이그레이션 시스템을 통한 로드
                    GameData migratedData = MigrateAndLoad(chunk);

                    if (migratedData != null)
                    {
                        // 마이그레이션 성공 시 현재 버전으로 저장
                        migratedData.SaveVersion = CURRENT_SAVE_VERSION;
                        return migratedData;
                    }
                    else
                    {
                        Debug.LogError("마이그레이션을 통한 로드에 실패했습니다.");
                    }
                }
                else
                {
                    Debug.LogError("게임 데이터의 chunk가 null이어서 역직렬화할 수 없습니다.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 역직렬화할 수 없습니다.\nException Massage:{0}\nChunk:{1}", ex.Message.ToString(), chunk);
            }

            return null;
        }
    }
}