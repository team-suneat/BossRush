using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamSuneat.Data
{
    /// <summary>
    /// JsonDataManager의 비동기 불러오기 구현
    /// </summary>
    public partial class JsonDataManager
    {
        #region Async Methods

        /// <summary>
        /// 비동기로 JSON 시트들을 불러옵니다.
        /// </summary>
        public static async Task LoadJsonSheetsAsync()
        {
            ClearAll();

            await LoadJsonSheetsByLabelAsync();

            OnLoadJsonDataAll();
            LogWarningParseJsonData();
            LogWarningParseJsonData();
        }

        /// <summary>
        /// Addressable의 "JSON" 라벨로 모든 TextAsset을 한 번에 불러와 파싱합니다.
        /// </summary>
        public static async Task LoadJsonSheetsByLabelAsync()
        {
            ClearAll();

            // Addressable에서 "JSON" 라벨로 모든 TextAsset을 한 번에 불러오기 (ResourcesManager 통해 호출)
            IList<TextAsset> assets = await ResourcesManager.LoadResourcesByLabelAsync<TextAsset>(AddressableLabels.Json);

            if (assets == null || assets.Count == 0)
            {
                Log.Warning("'JSON' 라벨로 불러온 TextAsset이 없습니다.");
                return;
            }

            for (int i = 0; i < assets.Count; i++)
            {
                TextAsset asset = assets[i];
                if (asset == null || string.IsNullOrEmpty(asset.name) || string.IsNullOrEmpty(asset.text))
                {
                    continue;
                }

                // 파일명에서 시트명 추출 (예: Character.json → Character)
                string sheetName = asset.name;
                if (sheetName.EndsWith(".json"))
                    sheetName = sheetName.Substring(0, sheetName.Length - 5);

                if (Enum.TryParse<_Sheet>(sheetName, out var sheet))
                {
                    ParseJsonData(sheet, asset.text);
                }
                else
                {
                    Log.Warning("알 수 없는 Sheet입니다: {0}", asset.name);
                }
            }

            OnLoadJsonDataAll();
            LogWarningParseJsonData();
            LogWarningParseJsonData();
        }

        #endregion Async Methods
    }
}