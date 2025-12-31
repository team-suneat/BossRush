using System.Collections.Generic;

namespace TeamSuneat
{
    /// <summary>
    /// 구글 시트 한 행(Dictionary)을 강타입 모델로 변환하는 인터페이스.
    /// 실패 시 false를 반환하고 warnings에 이유를 남깁니다.
    /// </summary>
    public interface IGoogleSheetRowConverter<TModel>
    {
        bool TryConvert(Dictionary<string, string> row, out TModel model);
    }
}