using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class VitalManager : Singleton<VitalManager>
    {
        private readonly List<Vital> _vitals = new List<Vital>();

        public int Count => _vitals.Count;

        public void Add(Vital vital)
        {
            if (vital != null)
            {
                if (!_vitals.Contains(vital))
                {
                    _vitals.Add(vital);

                    Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 생명체 바이탈을 등록합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
                }
                else
                {
                    Log.Warning(LogTags.Vital, "[Manager] 이미 등록된 생명체 바이탈을 중복 등록할 수 없습니다. {0}", vital.GetHierarchyPath());
                }
            }
        }

        public void Remove(Vital vital)
        {
            if (vital != null)
            {
                if (_vitals.Contains(vital))
                {
                    _vitals.Remove(vital);

                    Log.Info(LogTags.Vital, "[Manager] {0}(SID: {1}) 생명체 바이탈을 제거합니다.", vital.GetHierarchyName(), vital.SID.ToSelectString());
                }
                else
                {
                    Log.Warning(LogTags.Vital, "[Manager] 등록되지 않은 생명체 바이탈을 제거할 수 없습니다. {0}", vital.GetHierarchyPath());
                }
            }
        }

        public void Clear()
        {
            _vitals.Clear();

            Log.Info(LogTags.Vital, "[Manager] 모든 생명체 바이탈을 초기화/제거합니다.");
        }
    }
}