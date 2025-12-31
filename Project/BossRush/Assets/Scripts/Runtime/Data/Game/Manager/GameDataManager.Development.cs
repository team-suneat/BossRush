using System.IO;
using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 개발 도구를 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        /// <summary>
        /// 개발용 빌드의 세이브 Json 파일을 불러와 DAT 파일로 변환합니다.
        /// </summary>
        public static void ReadAndWriteEncrypt()
        {
            string loadFilePath = string.Format("{0}/{1}{2}_Dev.json", Application.persistentDataPath, Application.productName, 1);
            if (File.Exists(loadFilePath))
            {
                string chunk = File.ReadAllText(loadFilePath);
                string symmetricKey = AES.Encrypt(GameSymmetricIdentifier(), "pub");
                string chunkAED = AES.Encrypt(chunk, symmetricKey);

                if (!string.IsNullOrEmpty(chunkAED))
                {
                    string saveFilePath = string.Format("{0}/{1}{2}_Dev.dat", Application.persistentDataPath, Application.productName, 1);
                    File.WriteAllText(saveFilePath, chunkAED);

                    Log.Info("개발용 빌드의 세이브 Json 파일을 불러와 DAT 파일로 변환합니다");
                }
            }
            else
            {
                Log.Warning("개발용 빌드의 세이브 Json 파일을 불러와 DAT 파일로 변환할 수 없습니다");
            }
        }

        /// <summary>
        /// 개발용 빌드의 세이브 DAT 파일을 불러와 Json 파일로 변환합니다.
        /// </summary>
        public static void ReadAndWriteDecrypt()
        {
            string loadFilePath = string.Format("{0}/{1}{2}_Dev.dat", Application.persistentDataPath, Application.productName, 1);
            if (File.Exists(loadFilePath))
            {
                string chunkAES = File.ReadAllText(loadFilePath);
                string symmetricKey = AES.Encrypt(GameSymmetricIdentifier(), "pub");
                string chunk = AES.Decrypt(chunkAES, symmetricKey);

                if (!string.IsNullOrEmpty(chunk))
                {
                    string saveFilePath = string.Format("{0}/{1}{2}_Dev.json", Application.persistentDataPath, Application.productName, 1);
                    File.WriteAllText(saveFilePath, chunk);

                    Log.Info("개발용 빌드의 세이브 DAT 파일을 불러와 Json 파일로 변환합니다");
                }
            }
            else
            {
                Log.Warning("개발용 빌드의 세이브 DAT 파일을 불러와 Json 파일로 변환할 수 없습니다.");
            }
        }
    }
}