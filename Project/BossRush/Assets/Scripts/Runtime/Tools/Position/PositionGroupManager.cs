using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamSuneat
{
    public class PositionGroupManager : Singleton<PositionGroupManager>
    {
        // 기존 PositionGroup용 multi-map
        private readonly ListMultiMap<HitmarkNames, PositionGroup> _hitmarkPositionGroups = new();

        // 추가: ParentPositionGroup용 multi-map
        private readonly ListMultiMap<HitmarkNames, ParentPositionGroup> _parentHitmarkPositionGroups = new();

        #region Register

        // 기존 PositionGroup 등록
        public bool Register<T>(T keyName, PositionGroup positionGroup) where T : Enum
        {
            if (typeof(T) == typeof(HitmarkNames))
            {
                HitmarkNames hitmarkName = EnumEx.ConvertTo<HitmarkNames>(keyName.ToString());
                if (hitmarkName == HitmarkNames.None)
                {
                    return false;
                }
                if (!_hitmarkPositionGroups.ContainsKey(hitmarkName))
                {
                    _hitmarkPositionGroups.Add(hitmarkName, positionGroup);
                    Log.Progress(LogTags.PositionGroup, "Hitmark({0}) PositionGroup 을 등록합니다.", hitmarkName.ToLogString());
                    return true;
                }
            }

            return false;
        }

        // 추가: ParentPositionGroup 등록
        public bool Register<T>(T keyName, ParentPositionGroup parentGroup) where T : Enum
        {
            if (typeof(T) == typeof(HitmarkNames))
            {
                HitmarkNames hitmarkName = EnumEx.ConvertTo<HitmarkNames>(keyName.ToString());
                if (hitmarkName == HitmarkNames.None)
                {
                    return false;
                }
                if (!_parentHitmarkPositionGroups.ContainsKey(hitmarkName))
                {
                    _parentHitmarkPositionGroups.Add(hitmarkName, parentGroup);
                    Log.Progress(LogTags.PositionGroup, "Hitmark({0}) ParentPositionGroup 을 등록합니다.", hitmarkName.ToLogString());
                    return true;
                }
            }

            return false;
        }

        #endregion Register

        #region Unregister

        // 기존 PositionGroup 해제
        public bool Unregister(PositionGroup positionGroup)
        {
            if (Unregister(positionGroup.HitmarkName, positionGroup))
            {
                return true;
            }
            return false;
        }

        private bool Unregister(HitmarkNames key, PositionGroup positionGroup)
        {
            if (key != HitmarkNames.None)
            {
                if (_hitmarkPositionGroups.ContainsKey(key))
                {
                    _hitmarkPositionGroups.Remove(key, positionGroup);
                    return true;
                }
            }
            return false;
        }

        // 추가: ParentPositionGroup 해제
        public bool Unregister(ParentPositionGroup parentGroup)
        {
            bool result = false;
            result |= Unregister(parentGroup.HitmarkName, parentGroup);
            return result;
        }

        private bool Unregister(HitmarkNames key, ParentPositionGroup parentGroup)
        {
            if (key != HitmarkNames.None)
            {
                if (_parentHitmarkPositionGroups.ContainsKey(key))
                {
                    _parentHitmarkPositionGroups.Remove(key, parentGroup);
                    return true;
                }
            }
            return false;
        }

        #endregion Unregister

        public void Clear()
        {
            _hitmarkPositionGroups.Clear();
            _parentHitmarkPositionGroups.Clear();
        }

        #region Find

        // 기존 PositionGroup 검색

        public PositionGroup Find(string key)
        {
            HitmarkNames hitmarkName = EnumEx.ConvertTo<HitmarkNames>(key);
            if (hitmarkName != HitmarkNames.None)
            {
                PositionGroup positionGroup = Find(hitmarkName);
                if (positionGroup != null)
                {
                    return positionGroup;
                }
            }

            Log.Error($"해당 키 값({key})으로 포지션 그룹을 찾을 수 없습니다.");
            return null;
        }

        public PositionGroup Find(HitmarkNames hitmarkName, PositionGroup.Types type)
        {
            List<PositionGroup> hitmarkPositionGroup;
            if (_hitmarkPositionGroups.TryGetValue(hitmarkName, out hitmarkPositionGroup))
            {
                for (int i = 0; i < hitmarkPositionGroup.Count; i++)
                {
                    PositionGroup item = hitmarkPositionGroup[i];
                    if (item.Type == type)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public PositionGroup Find(HitmarkNames hitmarkName)
        {
            return Find(hitmarkName, PositionGroup.Types.None);
        }

        // 추가: ParentPositionGroup 검색
        public ParentPositionGroup FindParent(HitmarkNames hitmarkName)
        {
            List<ParentPositionGroup> parentHitmarkPositionGroupList;
            if (_parentHitmarkPositionGroups.TryGetValue(hitmarkName, out parentHitmarkPositionGroupList))
            {
                // 여러 개 등록되어 있으면 첫 번째를 반환합니다.
                return parentHitmarkPositionGroupList.FirstOrDefault();
            }
            return null;
        }

        #endregion Find
    }
}