using System.IO;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 파일 입출력 시스템을 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        private static string SaveFilePathFormat { get; set; }
        private static string BackupFilePathFormat { get; set; }

        public static void SetSaveFilePath()
        {
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                if (GameDefine.USE_AES_EDITOR)
                {
                    SaveFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}{"{0}"}_Dev.dat";
                }
                else
                {
                    SaveFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}{"{0}"}_Dev.json";
                }
            }
            else
            {
                SaveFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}{"{0}"}.dat";
            }

            // 백업 파일 포맷 설정
            BackupFilePathFormat = $"{Application.persistentDataPath}/{Application.productName}_Backup.dat";
        }

        /// <summary>
        /// 세이브 파일 경로를 반환합니다.
        /// </summary>
        /// <param name="index">세이브 파일 인덱스</param>
        /// <returns>세이브 파일 경로</returns>
        public static string GetSaveFilePath(int index)
        {
            return string.Format(SaveFilePathFormat, index + 1);
        }

        /// <summary>
        /// 백업 파일 경로를 반환합니다.
        /// </summary>
        /// <returns>백업 파일 경로</returns>
        public static string GetBackupFilePath()
        {
            return BackupFilePathFormat;
        }

        /// <summary>
        /// 파일에서 데이터를 읽습니다.
        /// </summary>
        /// <param name="saveFilePath">파일 경로</param>
        /// <returns>읽은 데이터</returns>
        private string Read(string saveFilePath)
        {
            if (!TryApplyAES())
            {
                return File.ReadAllText(saveFilePath);
            }
            else
            {
                string chunkAES = File.ReadAllText(saveFilePath);
                return Decrypt(chunkAES);
            }
        }

        /// <summary>
        /// 파일에 데이터를 씁니다.
        /// </summary>
        /// <param name="saveFilePath">파일 경로</param>
        /// <param name="chunk">쓸 데이터</param>
        /// <returns>성공 여부</returns>
        private bool Write(string saveFilePath, string chunk)
        {
            try
            {
                Log.Info(LogTags.GameData, "게임 데이터를 저장합니다. SaveFilePath: {0}\nChunk: {1}", saveFilePath, chunk);
                File.WriteAllText(saveFilePath, chunk);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 저장할 수 없습니다.\nException Massage:{0}", ex.Message.ToString());
                return false;
            }
        }

        /// <summary>
        /// 빌드용 세이브 파일을 삭제합니다.
        /// </summary>
        public static void DeleteSaveFileForBuild()
        {
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    Debug.LogFormat($"세이브 데이터를 삭제합니다. 세이브 데이터 경로: {saveFilePath}");
                }
                else
                {
                    Debug.LogFormat($"삭제할 세이브 데이터를 존재하지 않습니다. 세이브 데이터 경로: {saveFilePath}");
                }
            }
        }

        /// <summary>
        /// 에디터용 세이브 파일을 삭제합니다.
        /// </summary>
        public static void DeleteSaveFileForEditor()
        {
            for (int i = 0; i < GAME_DATA_COUNT; i++)
            {
                string saveFilePath = GetSaveFilePath(i);

                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);

                    Debug.Log($"로컬 세이브 파일을 삭제합니다. SaveFilePath: {saveFilePath}");
                }
                else
                {
                    Debug.Log($"로컬 세이브 파일이 이미 삭제되었습니다. SaveFilePath: {saveFilePath}");
                }
            }
        }
    }
}