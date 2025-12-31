using UnityEngine;

namespace TeamSuneat.Data.Game
{
    /// <summary>
    /// GameDataManager의 암호화/복호화 시스템을 담당하는 partial 클래스
    /// </summary>
    public partial class GameDataManager
    {
        /// <summary>
        /// 게임 대칭키 식별자를 반환합니다.
        /// </summary>
        /// <returns>대칭키 식별자</returns>
        private static string GameSymmetricIdentifier()
        {
            return "n4+uvfeFi+VzurbrvJCfLIlfeQhPrlHkfwDxeejJo8UfTwCcgOvl2Ta+D8OmXdxnMAdfQ0zGI5FqT2PqhdMoFOtiaKlsZfMCfHj1DMGcAwAX0vy/lrDpqIks64wcXD";
        }

        /// <summary>
        /// AES 암호화를 적용할지 확인합니다.
        /// </summary>
        /// <returns>AES 적용 여부</returns>
        private bool TryApplyAES()
        {
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                if (!GameDefine.USE_AES_EDITOR)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 데이터를 암호화합니다.
        /// </summary>
        /// <param name="chunk">암호화할 데이터</param>
        /// <returns>암호화된 데이터</returns>
        private string Encrypt(string chunk)
        {
            try
            {
                if (!string.IsNullOrEmpty(chunk))
                {
                    return AES.Encrypt(chunk, _symmetricKey);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 암호화할 수 없습니다.\nException Massage:{0}\nChunk:{1}", ex.Message.ToString(), chunk);
            }

            return null;
        }

        /// <summary>
        /// 데이터를 복호화합니다.
        /// </summary>
        /// <param name="chunkAES">복호화할 데이터</param>
        /// <returns>복호화된 데이터</returns>
        private string Decrypt(string chunkAES)
        {
            try
            {
                return AES.Decrypt(chunkAES, _symmetricKey);
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("게임 데이터를 복호화할 수 없습니다.\nException Massage:{0}\nChunk:{1}", ex.Message.ToString(), chunkAES);
            }

            return null;
        }
    }
}