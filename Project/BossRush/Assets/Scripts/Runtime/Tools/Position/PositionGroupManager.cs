using System.Collections.Generic;

namespace TeamSuneat
{
    public class PositionGroupManager : Singleton<PositionGroupManager>
    {
        private readonly ListMultiMap<PositionGroupNames, PositionGroup> _positionGroups = new();
        private readonly ListMultiMap<PositionGroupNames, ParentPositionGroup> _parentPositionGroups = new();

        #region Register

        public bool Register(PositionGroupNames keyName, PositionGroup positionGroup)
        {
            if (keyName == PositionGroupNames.None)
            {
                return false;
            }
            if (!_positionGroups.ContainsKey(keyName))
            {
                _positionGroups.Add(keyName, positionGroup);
                Log.Progress(LogTags.PositionGroup, "PositionGroup({0}) 을 등록합니다.", keyName.ToLogString());
                return true;
            }
            return false;
        }

        public bool Register(PositionGroupNames keyName, ParentPositionGroup parentGroup)
        {
            if (keyName == PositionGroupNames.None)
            {
                return false;
            }
            if (!_parentPositionGroups.ContainsKey(keyName))
            {
                _parentPositionGroups.Add(keyName, parentGroup);
                Log.Progress(LogTags.PositionGroup, "ParentPositionGroup({0}) 을 등록합니다.", keyName.ToLogString());
                return true;
            }
            return false;
        }

        #endregion Register

        #region Unregister

        public bool Unregister(PositionGroup positionGroup)
        {
            if (positionGroup.PositionGroupName == PositionGroupNames.None)
            {
                return false;
            }
            if (_positionGroups.ContainsKey(positionGroup.PositionGroupName))
            {
                _positionGroups.Remove(positionGroup.PositionGroupName, positionGroup);
                return true;
            }
            return false;
        }

        public bool Unregister(ParentPositionGroup parentGroup)
        {
            if (parentGroup.PositionGroupName == PositionGroupNames.None)
            {
                return false;
            }
            if (_parentPositionGroups.ContainsKey(parentGroup.PositionGroupName))
            {
                _parentPositionGroups.Remove(parentGroup.PositionGroupName, parentGroup);
                return true;
            }
            return false;
        }

        #endregion Unregister

        public void Clear()
        {
            _positionGroups.Clear();
            _parentPositionGroups.Clear();
        }

        public PositionGroup Find(PositionGroupNames keyName)
        {
            if (_positionGroups.TryGetValue(keyName, out List<PositionGroup> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    PositionGroup item = list[i];
                    return item;
                }
            }

            return null;
        }
    }
}