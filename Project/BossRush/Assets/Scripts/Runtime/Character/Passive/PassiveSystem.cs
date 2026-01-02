using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TeamSuneat.Data;

namespace TeamSuneat.Passive
{
    public partial class PassiveSystem : XBehaviour
    {
        [Title("#PassiveSystem")]
        public Character Owner;

        [Title("#PassiveSystem", "RestTime")]
        [InfoBox("패시브의 RestTime을 통합 관리하는 컨트롤러")]
        private PassiveRestTimeController _restTimeController;

        private Dictionary<PassiveNames, PassiveEntity> _entities = new();

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
        }

        public override void AutoNaming()
        {
            SetGameObjectName("#Passive(" + Owner.Name + ")");
        }

        protected void Awake()
        {
            InitializeRestTimeController();
            RegisterEntities();
        }

        public void LogicUpdate()
        {
            // 매 프레임 RestTime 상태 업데이트
            // PassiveSystem은 캐릭터와 함께 활성화/비활성화되므로 캐릭터가 활성화된 동안에는 RestTime이 계속 관리됨
            _restTimeController.UpdateRestTimes();
        }

        private void RegisterEntities()
        {
            PassiveEntity[] entities = GetComponentsInChildren<PassiveEntity>();
            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    Add(entities[i], 1);
                }
            }
        }

        public PassiveEntity GetPassive(PassiveNames passiveName)
        {
            if (_entities.ContainsKey(passiveName))
            {
                return _entities[passiveName];
            }

            return null;
        }

        public void Add(PassiveNames passiveName, int passiveLevel)
        {
            PassiveAsset passiveAsset = ScriptableDataManager.Instance.FindPassive(passiveName);
            if (!passiveAsset.IsValid())
            {
                LogFailedAdd(passiveName);
                return;
            }

            if (!_entities.ContainsKey(passiveAsset.Name))
            {
                PassiveEntity passiveEntity = ResourcesManager.SpawnPassiveEntity(transform);
                if (passiveEntity != null)
                {
                    passiveEntity.Name = passiveAsset.Name;
                    passiveEntity.AutoNaming();
                    _entities.Add(passiveAsset.Name, passiveEntity);

                    LogCreateOrRegister(passiveAsset, passiveEntity.Level, passiveLevel);
                }
            }
            else
            {
                PassiveEntity passiveEntity = _entities[passiveAsset.Name];

                LogAlreadyRegistered(passiveAsset, passiveEntity.Level, passiveLevel);
            }

            if (_entities.ContainsKey(passiveAsset.Name))
            {
                _entities[passiveAsset.Name].SetOwner(Owner);
                _entities[passiveAsset.Name].SetAsset(passiveAsset);
                _entities[passiveAsset.Name].SetLevel(passiveLevel);
                _entities[passiveAsset.Name].Activate();

                // 기존 RestTime 상태 확인 및 로깅
                CheckExistingRestTime(passiveAsset);
            }
        }

        public void Add(PassiveEntity passiveEntity, int passiveLevel)
        {
            if (passiveEntity != null)
            {
                PassiveAsset passiveAsset = ScriptableDataManager.Instance.FindPassive(passiveEntity.Name);
                if (!passiveAsset.IsValid())
                {
                    LogFailedAdd(passiveEntity.Name);
                    return;
                }

                if (!_entities.ContainsKey(passiveEntity.Name))
                {
                    _entities.Add(passiveEntity.Name, passiveEntity);
                    LogAdd(passiveAsset, passiveLevel);
                }
                else
                {
                    LogAlreadyRegistered(passiveAsset, passiveEntity.Level, passiveLevel);
                }

                passiveEntity.SetOwner(Owner);
                passiveEntity.SetAsset(passiveAsset);
                passiveEntity.SetLevel(passiveLevel); // 에셋을 불러와 레벨을 설정합니다.
                passiveEntity.Activate();
            }
        }

        public void Remove(PassiveNames passiveName)
        {
            if (_entities.ContainsKey(passiveName))
            {
                _entities[passiveName].Despawn();
                _ = _entities.Remove(passiveName);

                // RestTime 정보는 유지 (장비 착용/해제와 무관하게)
                // ClearPassiveRestTime(passiveName); // 제거

                LogRemove(passiveName);
            }
            else
            {
                LogFailedRemove(passiveName);
            }
        }

        public void Clear()
        {
            PassiveEntity[] entities = _entities.Values.ToArray();
            for (int i = entities.Length - 1; i >= 0; i--)
            {
                PassiveEntity passiveEntity = entities[i];
                passiveEntity.Despawn();

                LogRemove(passiveEntity.Name);
            }

            _entities.Clear();

            // 모든 RestTime 정보도 함께 제거
            ClearAllPassiveRestTimes();
        }

        public void DeactivateAll()
        {
            foreach (KeyValuePair<PassiveNames, PassiveEntity> item in _entities)
            {
                item.Value.Deactivate();
            }
        }

        #region RestTime Management

        /// <summary>
        /// RestTime Controller를 초기화합니다.
        /// </summary>
        private void InitializeRestTimeController()
        {
            _restTimeController = new PassiveRestTimeController(Owner);
        }

        /// <summary>
        /// 패시브가 현재 RestTime 상태인지 확인합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        /// <returns>RestTime 상태 여부</returns>
        public bool IsPassiveResting(PassiveNames passiveName)
        {
            return _restTimeController.IsResting(passiveName);
        }

        /// <summary>
        /// 패시브의 RestTime을 시작합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        /// <param name="restTime">지속시간 (초)</param>
        public void StartPassiveRestTime(PassiveNames passiveName, float restTime)
        {
            _restTimeController.StartRestTime(passiveName, restTime);
        }

        /// <summary>
        /// 패시브의 RestTime을 강제로 종료합니다.
        /// </summary>
        /// <param name="passiveName">패시브 이름</param>
        public void StopPassiveRestTime(PassiveNames passiveName)
        {
            _restTimeController.StopRestTime(passiveName);
        }

        /// <summary>
        /// 모든 패시브의 RestTime 정보를 제거합니다.
        /// </summary>
        public void ClearAllPassiveRestTimes()
        {
            _restTimeController.ClearAllRestTimes();
        }

        /// <summary>
        /// 패시브 추가 시 기존 RestTime 상태를 확인하고 로깅합니다.
        /// </summary>
        /// <param name="passiveName">확인할 패시브 이름</param>
        private void CheckExistingRestTime(PassiveAsset passiveAsset)
        {
            if (_restTimeController.IsResting(passiveAsset.Name))
            {
                float remainingTime = _restTimeController.GetRemainingTime(passiveAsset.Name);

                LogRestTimeInfo(passiveAsset, remainingTime);
            }
        }

        #endregion RestTime Management
    }
}