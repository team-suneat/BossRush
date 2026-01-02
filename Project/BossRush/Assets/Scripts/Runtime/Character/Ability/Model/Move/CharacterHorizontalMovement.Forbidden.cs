using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace TeamSuneat
{
    public partial class CharacterHorizontalMovement
    {
        private readonly List<object> _lockSoures = new();
        private bool _isLockedMovement;

        public bool MovementForbidden
        {
            get
            {
                if (_isLockedMovement)
                {
                    return true;
                }

                if (_lockSoures.IsValid())
                {
                    return true;
                }

                return false;
            }
        }

        public void LockMovement(object source)
        {
            if (!_lockSoures.Contains(source))
            {
                _lockSoures.Add(source);
                LogLockMovement(source);
            }
        }

        public void LockMovement(FVNames source)
        {
            if (!_lockSoures.Contains(source))
            {
                _lockSoures.Add(source);
                LogLockMovement(source);
            }
        }

        public void UnlockMovement(Component source)
        {
            if (_lockSoures.Contains(source))
            {
                _lockSoures.Remove(source);
                LogUnlockMovement(source);
            }
        }

        public void UnlockMovement(FVNames source)
        {
            if (_lockSoures.Contains(source))
            {
                _lockSoures.Remove(source);
                LogUnlockMovement(source);
            }
        }

        public void UnlockAllMovement()
        {
            _isLockedMovement = false;
            _lockSoures.Clear();
            LogUnlockAllMovement();
        }

        private void LogLockMovement(object newSource)
        {
            if (Log.LevelInfo)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append("캐릭터의 수평 움직임을 금지합니다.");
                stringBuilder.AppendLine($" MovementForbidden: {MovementForbidden.ToBoolString()}");

                if (MovementForbidden)
                {
                    if (_isLockedMovement)
                    {
                        stringBuilder.AppendLine($"IsLockedMovement: {_isLockedMovement.ToBoolString()}");
                    }
                    if (_lockSoures.IsValid())
                    {
                        stringBuilder.AppendLine($"Lock Source Count: {_lockSoures.Count}");
                        foreach (object lockSource in _lockSoures)
                        {
                            if (lockSource == newSource)
                            {
                                stringBuilder.AppendLine($"Lock Source: {newSource}, {newSource.GetType()}");
                            }
                            else
                            {
                                stringBuilder.AppendLine($"Lock Source: {lockSource}, {lockSource.GetType()}");
                            }
                        }
                    }
                }

                LogInfo(stringBuilder.ToString());
            }
        }

        private void LogUnlockMovement(object newSource)
        {
            if (Log.LevelInfo)
            {
                StringBuilder stringBuilder = new StringBuilder();

                stringBuilder.Append("캐릭터의 수평 움직임 금지를 해제합니다.");
                stringBuilder.AppendLine($" MovementForbidden: {MovementForbidden.ToBoolString()}");

                if (MovementForbidden)
                {
                    if (_isLockedMovement)
                    {
                        stringBuilder.AppendLine($"IsLockedMovement: {_isLockedMovement.ToBoolString()}");
                    }
                    if (_lockSoures.IsValid())
                    {
                        stringBuilder.AppendLine($"Lock Source Count: {_lockSoures.Count}");
                        foreach (object lockSource in _lockSoures)
                        {
                            if (lockSource == newSource)
                            {
                                stringBuilder.AppendLine($"Lock Source: {newSource}, {newSource.GetType()}");
                            }
                            else
                            {
                                stringBuilder.AppendLine($"Lock Source: {lockSource}, {lockSource.GetType()}");
                            }
                        }
                    }
                }

                LogInfo(stringBuilder.ToString());
            }
        }

        private void LogUnlockAllMovement()
        {
            LogInfo("캐릭터의 모든 수평 움직임 금지를 해제합니다. MovementForbidden: {0}", MovementForbidden.ToBoolString());
        }
    }
}