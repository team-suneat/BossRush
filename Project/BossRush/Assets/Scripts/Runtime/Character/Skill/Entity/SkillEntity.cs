using Lean.Pool;
using Sirenix.OdinInspector;
using System.Linq;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary> SkillEntity 클래스는 캐릭터의 기술을 관리하고 실행하는 역할을 수행합니다. </summary>
    public partial class SkillEntity : XBehaviour, IPoolable
    {
        [FoldoutGroup("#SkillEntity", 1)]
        public Character Owner; // 기술 소유자

        [FoldoutGroup("#SkillEntity", true, 1)]
        public SkillNames Name; // 기술 이름

        [FoldoutGroup("#SkillEntity", 1)]
        public string NameString; // 기술 이름 문자열

        [ReadOnly]
        [FoldoutGroup("#SkillEntity", 1)]
        public string NameContent; // 기술 내용

        //-------------------------------------------------------------------------------------

        [FoldoutGroup("#SkillEntity", 1)]
        [LabelText("캐릭터 생성 시 기술 자동 추가")]
        public bool AutoAddOnBattle;

        [FoldoutGroup("#SkillEntity", 1)]
        [LabelText("기술 중단 시 공격 독립체 초기화 미사용")]
        public bool PreserveAttackEntitiesOnInterrupt;

        //-------------------------------------------------------------------------------------

        /// <summary> 기술 시전 실패 유형을 나타내는 열거형 </summary>
        private enum CastFailTypes
        {
            None, // 실패 없음

            /// <summary> 재사용 대기 </summary>
            Cooldowning, // 쿨다운 중

            /// <summary> 자원 부족 </summary>
            LackOfResource, // 자원 부족

            /// <summary> 무기 없음 </summary>
            NoWeapon, // 무기 없음
        }

        /// <summary> 기술 시전 실패 사유 </summary>
        private CastFailTypes _castFailType;

        /// <summary> 현재 기술 사용 횟수 </summary>
        private int _skillCount;

        /// <summary> 최대 사용 횟수 </summary>
        private int _skillMaxCount;

        private SkillAssetData _skillData;

        private readonly SkillHandler _skillHandler = new();

        public float CastTime { get; private set; } // 기술 시전 시간

        public float ElapsedTimeOnCast { get; set; } // 기술 시전 경과 시간

        public int ResourceCost { get; private set; }

        protected VProfile ProfileInfo => GameApp.GetSelectedProfile(); // 현재 선택된 프로필 정보

        protected VCharacter CharacterInfo => GameApp.GetSelectedCharacter(); // 현재 선택된 캐릭터 정보

        public SkillCategories Category
        {
            get
            {
                if (_skillData.IsValid())
                {
                    return _skillData.Category;
                }

                return SkillCategories.None;
            }
        }

        public SkillApplications Application
        {
            get
            {
                if (_skillData.IsValid())
                {
                    return _skillData.Application;
                }

                return SkillApplications.None;
            }
        }

        //

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            UnregisterRefreshStatEvent();
            UnregisterReplacedAnimationSkill();
            UnregisterSkillAnimation();
            StopCooldown();
            StopRest();
        }

        public void Despawn()
        {
            if (!IsDestroyed)
            {
                ResourcesManager.Despawn(gameObject);
            }
        }

        public void Initialize()
        {
            StopCooldown();

            _ = TryLoadData(Name);
        }

        public void Setup(SkillNames skillName, int skillLevel, Character owner)
        {
            if (!TryLoadData(skillName))
            {
                return;
            }

            Name = skillName;
            Owner = owner;

            ApplyAdditionalLevelByStat(skillLevel);
            SetupCooldownTime();

            LogInfo("기술을 생성하고 설정합니다.");

            InitializeFeedbacks();
            AutoNaming();
            SetHUD();
            RefreshSkillMaxCount();
            RefillSkillCount();
            RefreshResourceCost();

            LoadAnimationAssets();
            SetupUsableAnimationAssets();
            SetOrderRange();

            RegisterSkillAnimation();
            RegisterRefreshStatEvent();
            RegisterReplacedAnimationSkill();

            if (CheckPassiveSetting())
            {
                AddPassives();
                RemoveOverlapPassives();
            }

            AddBuffsOnSetup();
            LoadActivatingStageBuffs();
            InitializeOrderSkillVFX();
        }

        public void SetupAnimationsWithCondition()
        {
            if (HasSkillCondition())
            {
                SetupUsableAnimationAssets();
                SetOrderRange();
            }
        }

        private bool TryLoadData(SkillNames skillName)
        {
            _skillData = ScriptableDataManager.Instance.FindSkillClone(skillName);
            if (_skillData.IsValid())
            {
                return true;
            }

            _skillData = new SkillAssetData();
            Log.Error("기술 독립체의 기술 데이터를 불러올 수 없습니다. {0}, {1}", skillName, this.GetHierarchyName());

            return false;
        }

        /// <summary> 스킬을 활성화할 수 있는지 여부를 확인합니다. </summary>
        public bool TryActivate()
        {
            if (!CheckIfWeaponEquipped())
            {
                LogProgress("무기를 장비하지 않으면 {0} 기술을 사용할 수 없습니다.", Name.ToLogString());
                _castFailType = CastFailTypes.NoWeapon;
                return false;
            }
            else if (IsResting)
            {
                return false;
            }
            else if (CheckCooldowning() && CheckIfUsingSkillCountReachedZero())
            {
                LogProgress("재사용 대기중인 기술을 남은 기술 횟수가 없을 때 사용할 수 없습니다.");
                if (_skillData.UseCancelDuringCooldowning)
                {
                    DeactivateAttackEntities();
                }
                _castFailType = CastFailTypes.Cooldowning;
                return false;
            }
            else if (CheckRestSkillAnimation())
            {
                LogProgress("기술 애니메이션 사용 후 유휴시간이 지나지 않았다면 사용할 수 없습니다: {0}", _castRestTime);
                return false;
            }
            else if (!CheckWaitingCombo() && !CheckResourceCost())
            {
                if (_skillMaxCount <= 1)
                {
                    LogProgress("연속하지 않는 기술은 자원이 부족할 때 기술을 사용할 수 없습니다.");
                    _castFailType = CastFailTypes.LackOfResource;
                    return false;
                }
                else if (_skillCount == 0)
                {
                    LogProgress("연속하지 않는 기술은 기술 횟수가 없고, 자원이 부족할 때 기술을 사용할 수 없습니다.");
                    _castFailType = CastFailTypes.LackOfResource;
                    return false;
                }
            }

            if (!TryActivateWithAnimation())
            {
                return false;
            }

            return true;
        }

        public void SpawnWarningSoliloquy()
        {
            switch (_castFailType)
            {
                case CastFailTypes.Cooldowning:
                    {
                        UIManager.Instance?.SpawnSoliloquyIngame(SoliloquyTypes.CanNotUsedYet);
                    }
                    break;

                case CastFailTypes.LackOfResource:
                    {
                        UIManager.Instance?.SpawnSoliloquyIngame(SoliloquyTypes.LackOfMana);
                    }
                    break;

                case CastFailTypes.NoWeapon:
                    {
                        UIManager.Instance?.SpawnSoliloquyIngame(SoliloquyTypes.EquipWeapon);
                    }
                    break;
            }
        }

        public void Activate()
        {
            SetCastTime(); // 시전 시간을 설정합니다.
            ResetElapsedTimeOnCast(); // 시전 시간을 초기화합니다.

            bool isGrounded = Owner.Controller.State.IsGrounded; // 캐릭터가 땅에 있는지 확인합니다.
            Order skillOrder = isGrounded ? _skillOrderOnGround : _skillOrderInAir; // 캐릭터의 상태에 따라 스킬 순서를 결정합니다.

            if (CheckMaxOrderValid(skillOrder)) // 최대 순서가 유효한지 확인합니다.
            {
                HandleSkillOrder(skillOrder, isGrounded);
            }
            else
            {
                HandleInvalidSkillOrder(isGrounded);
            }

            FinalizeActivation(); // 최종 활성화 단계를 수행합니다.

            LogInfo("기술 독립체를 활성화 완료합니다. {0}", Name.ToLogString());
        }

        private void HandleSkillOrder(Order skillOrder, bool isGrounded)
        {
            if (skillOrder.CheckMin()) // 최소 순서를 확인합니다.
            {
                if (_skillData.UseCostOnCast)
                {
                    UseVitalResourceOnActivate(); // 자원을 사용합니다.
                }
                if (skillOrder.CheckMax() && _skillData.UseCooldownOnCast)
                {
                    StartCooldown(); // 쿨다운을 시작합니다.
                }
            }
            else if (skillOrder.CheckMax()) // 최대 순서를 확인합니다.
            {
                UseSkillCount(); // 스킬 카운트를 사용합니다.

                if (_skillData.UseCooldownOnCast)
                {
                    StartCooldown(); // 쿨다운을 시작합니다.
                }

                StartRest(); // 기술 시전 휴식을 시작합니다.
            }

            if (TryPlayAnimation())
            {
                SpawnCastVFX();
                TriggerPlayAnimationFeedback();
            }
            else
            {
                if (_skillData.ApplyBuffWithoutAnimation) // 애니메이션 재생에 실패하고, 버프를 바로 적용할 경우
                {
                    AddBuffs();
                }
                if (_skillData.StartCooldownWithoutAnimation)
                {
                    StartCooldown();
                }

                if (_skillData.ActivateHitmarkWithoutAnimation) // 애니메이션 재생에 실패하고, 히트마크를 바로 사용할 경우
                {
                    ActivateAttackEntities(); // 공격 엔티티를 활성화합니다.
                }
            }

            int orderVFX = isGrounded ? skillOrder.Current : -1; // 순서 VFX를 설정합니다 (지상일 경우 현재 순서, 공중일 경우 -1).
            SpawnOrderSkillVFX(orderVFX); // 순서 스킬 VFX를 생성합니다.

            NextSkillOrder(skillOrder);// 다음 스킬 순서를 설정합니다.
            StartDropOrder(skillOrder); // 드롭 순서를 시작합니다.
        }

        private void HandleInvalidSkillOrder(bool isGrounded)
        {
            if (TryPlayAnimation())
            {
                SpawnCastVFX();
                TriggerPlayAnimationFeedback();
            }
            else // 애니메이션 재생에 실패했을 때
            {
                if (_skillData.ApplyBuffWithoutAnimation) // 버프를 바로 적용할 경우
                {
                    AddBuffs();
                }
                if (_skillData.StartCooldownWithoutAnimation) // 재사용 대기를 바로 시작할 경우
                {
                    StartCooldown();
                }
                if (_skillData.ActivateHitmarkWithoutAnimation) // 히트마크를 바로 사용할 경우
                {
                    ActivateAttackEntities(); // 공격 엔티티를 활성화합니다.
                }
            }

            UseSkillCount(); // 스킬 카운트를 사용합니다.
            if (_skillData.UseCostOnCast)
            {
                UseVitalResourceOnActivate(); // 자원을 사용합니다.
            }
            if (_skillData.UseCooldownOnCast)
            {
                StartCooldown(); // 쿨다운을 시작합니다.
            }

            StartRest(); // 기술 시전 휴식을 시작합니다.

            int orderVFX = isGrounded ? 0 : -1; // 순서 VFX를 설정합니다 (지상일 경우 0, 공중일 경우 -1).
            SpawnOrderSkillVFX(orderVFX); // 순서 스킬 VFX를 생성합니다.
        }

        private void FinalizeActivation()
        {
            StartForceVelocity();
            TriggerCastFeedback();

            _ = GlobalEvent<SkillNames>.Send(GlobalEventType.PLAYER_CHARACTER_CAST_SKILL, Name);
        }

        private void InternalDeactivate(bool fromInterupt)
        {
            StopForceVelocity();
            StopCastFeedback();

            if (fromInterupt && PreserveAttackEntitiesOnInterrupt)
            {
                DeactivateAttackEntities();
            }

            if (fromInterupt && !_skillData.AddBuffOnSetup)
            {
                RemoveBuffs();
            }
            else if (!fromInterupt)
            {
                RemoveBuffs();
            }

            DespawnCastVFX(Owner.CharacterAnimator.PlayingSkillAnimationName);
        }

        public void Deactivate()
        {
            LogInfo("기술을 비활성화합니다.");
            InternalDeactivate(false);
        }

        public void DeactivateOnInterrupt()
        {
            LogInfo("이전에 사용한 기술을 비활성화합니다.");
            InternalDeactivate(true);
        }

        #region 기술 검사 (Skill Check)

        private bool CheckIfWeaponEquipped()
        {
            if (Owner == null)
            {
                return true;
            }

            if (!Owner.IsPlayer)
            {
                return true;
            }

            if (_skillData.Category == SkillCategories.Dash)
            {
                return true;
            }

            ItemNames equipedWeaponName = ProfileInfo.Inventory.FindEquippedName(EquipmentSlotTypes.Weapons);
            if (equipedWeaponName == ItemNames.None)
            {
                return false;
            }

            return true;
        }

        private bool CheckIfUsingSkillCountReachedZero()
        {
            if (_skillMaxCount >= 1 && _skillCount == 0)
            {
                return true;
            }

            return false;
        }

        #endregion 기술 검사 (Skill Check)

        #region 기술 명령 할당 (Skill Action Assign)

        public void AssignSKill()
        {
            SetHUD();
        }

        public void UnssignSkill()
        {
            ResetHUD();
        }

        #endregion 기술 명령 할당 (Skill Action Assign)

        #region 기술 횟수 (Skill Count)

        private void AddSkillCount()
        {
            if (_skillMaxCount > 0)
            {
                _skillCount = Mathf.Clamp(_skillCount + 1, 0, _skillMaxCount);
                LogProgress("기술 횟수를 추가합니다. {0}/{1}", _skillCount, _skillMaxCount);
                SetCountOfHUD();
            }
        }

        private void UseSkillCount(int useCount = 1)
        {
            if (_skillMaxCount > 0)
            {
                _skillCount = Mathf.Clamp(_skillCount - useCount, 0, _skillMaxCount);
                LogProgress("기술 횟수를 사용합니다. {0}/{1}", _skillCount, _skillMaxCount);
                SetCountOfHUD();
            }
        }

        private void SetSkillCount(int count)
        {
            if (_skillMaxCount > 0)
            {
                _skillCount = Mathf.Clamp(count, 0, _skillMaxCount);
                LogProgress("기술 횟수를 설정합니다. {0}/{1}", _skillCount, _skillMaxCount);
                SetCountOfHUD();
            }
        }

        private void RefillSkillCount()
        {
            if (_skillMaxCount > 0)
            {
                _skillCount = _skillMaxCount;
                LogProgress("기술 횟수를 최대로 채웁니다. {0}/{1}", _skillCount, _skillMaxCount);
                SetCountOfHUD();
            }
        }

        private void RefreshSkillMaxCount()
        {
            _skillMaxCount = _skillData.MaxCount;

            if (Name == SkillNames.FrostArrow)
            {
                if (Owner.Stat.ContainsKey(StatNames.FrostArrowSkillMaxCount))
                {
                    int statValue = Owner.Stat.FindValueOrDefaultToInt(StatNames.FrostArrowSkillMaxCount);
                    _skillMaxCount += statValue;

                    LogProgress("마검사의 핵심 기술 [서리 화살]의 최대 횟수를 {0} 추가합니다.", statValue.ToSelectString());
                }
            }
            else if (_skillData.Category == SkillCategories.Dash)
            {
                if (Owner.Stat.ContainsKey(StatNames.DashSkillCount))
                {
                    int statValue = Owner.Stat.FindValueOrDefaultToInt(StatNames.DashSkillCount);
                    _skillMaxCount += statValue;

                    LogProgress("돌진 기술의 최대 횟수를 {0} 추가합니다.", statValue.ToSelectString());
                }
            }

            LogProgress("기술 최대 횟수를 갱신합니다. {0}/{1}", _skillCount, _skillMaxCount);
        }

        #endregion 기술 횟수 (Skill Count)

        #region 기술 강제 이동 (Force Velocity)

        private void StartForceVelocity()
        {
            if (_skillData.ForceVelocity != FVNames.None)
            {
                CharacterForceVelocity fvAbility = Owner.FindAbility<CharacterForceVelocity>();
                if (fvAbility != null)
                {
                    ForceVelocityAsset forceVelocityAsset = ScriptableDataManager.Instance.FindForceVelocity(_skillData.ForceVelocity);
                    if (forceVelocityAsset != null)
                    {
                        LogProgress("기술의 강제 이동을 활성화합니다. {0}", _skillData.ForceVelocity.ToLogString());
                        fvAbility.StartForceVelocity(forceVelocityAsset, Owner.IsFacingRight, this);
                    }
                }
            }
        }

        private void StopForceVelocity()
        {
            if (_skillData != null)
            {
                if (_skillData.ForceVelocity != FVNames.None)
                {
                    CharacterForceVelocity fvAbility = Owner.FindAbility<CharacterForceVelocity>();
                    if (fvAbility != null)
                    {
                        LogProgress("기술의 강제 이동을 비활성화합니다. {0}", _skillData.ForceVelocity.ToLogString());
                        fvAbility.StopForceVelocity(this);
                    }
                }
            }
        }

        #endregion 기술 강제 이동 (Force Velocity)

        #region 기술 사용 VFX

        private void InitializeOrderSkillVFX()
        {
            if (!OrderSkillVFXs.IsValid())
            {
                return;
            }

            foreach (OrderSkillVFXData item in OrderSkillVFXs)
            {
                item.Initialize(Owner);
            }
        }

        private void SpawnOrderSkillVFX(int currentOrder)
        {
            if (!OrderSkillVFXs.IsValid())
            {
                return;
            }

            ItemNames itemName = ProfileInfo.Inventory.FindEquippedName(EquipmentSlotTypes.Weapons);
            if (itemName == ItemNames.None)
            {
                return;
            }

            ItemData itemData = JsonDataManager.FindItemData(itemName);
            if (!itemData.IsValid() || itemData.IsBlock)
            {
                return;
            }

            OrderSkillVFXData item = OrderSkillVFXs.FirstOrDefault<OrderSkillVFXData>(x => x.Order == currentOrder);
            if (item == null)
            {
                return;
            }

            item.SpawnVFX(itemData.SubCategory, transform, position);
        }

        private void SpawnCastVFX()
        {
            SkillAnimationAsset skillAnimationAsset = GetSkillAnimation();
            if (skillAnimationAsset != null)
            {
                skillAnimationAsset.SpawnCastVFX(transform, position);
            }
        }

        #endregion 기술 사용 VFX
    }
}