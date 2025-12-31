using System.Linq;
using Sirenix.OdinInspector;
using TeamSuneat;
using UnityEngine;

namespace TeamSuneat.Data
{
    [CreateAssetMenu(fileName = "GameDefine", menuName = "TeamSuneat/Scriptable/Define")]
    public class GameDefineAsset : ScriptableObject
    {
        public GameDefineAssetData Data;
    }

    [System.Serializable]
    public class GameDefineAssetData
    {
        [Title("빌드 (Build)")]
        [LabelText("에디터에서 사용하는 빌드 타입")]
        public BuildTypes EDITOR_BUILD_TYPE = BuildTypes.Editor;

        [LabelText("구글 시트 동기화를 사용할 빌드 타입 목록")]
        public BuildTypes[] SUPPORTED_BUILD_TYPES_FOR_GOOGLE_SHEET;
    }

    /// <summary>
    /// GameDefineAsset의 확장 메서드 및 프로퍼티
    /// </summary>
    public static class GameDefineAssetExtensions
    {
        /// <summary>
        /// 현재 빌드 타입에서 구글 시트 동기화가 활성화되어 있는지 확인합니다.
        /// </summary>
        public static bool IsGoogleSheetSyncEnabled(this GameDefineAsset asset)
        {
            if (asset == null || asset.Data == null)
            {
                return false;
            }

            BuildTypes[] supportedTypes = asset.Data.SUPPORTED_BUILD_TYPES_FOR_GOOGLE_SHEET;
            if (!supportedTypes.IsValidArray())
            {
                return false;
            }

#if UNITY_EDITOR
            return supportedTypes.Contains(asset.Data.EDITOR_BUILD_TYPE);
#else
            if (GameDefine.IS_DEVELOPMENT_BUILD)
            {
                return supportedTypes.Contains(BuildTypes.Development);
            }

            return supportedTypes.Contains(BuildTypes.Live);
#endif
        }
    }
}