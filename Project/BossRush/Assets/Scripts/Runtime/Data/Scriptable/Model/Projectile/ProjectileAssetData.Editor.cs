using UnityEngine;
using TeamSuneat;

namespace TeamSuneat.Data
{
    public partial class ProjectileAssetData
    {
        private void CustomLog()
        {
#if UNITY_EDITOR
            if (Name == ProjectileNames.None)
            {
                Log.Error("발사체의 이름이 설정되지 않았습니다: {0}", Name);
            }
#endif
        }

#if UNITY_EDITOR

        #region Inspector Color Methods

        // 필요시 추가 색상 메서드

        #endregion Inspector Color Methods

#endif
    }
}

