using System.Collections;
using System.Collections.Generic;
using TeamSuneat.Data;
using TeamSuneat.Data.Game;
using UnityEngine;

namespace TeamSuneat
{
    public partial class SkillSystem : XBehaviour
    {
        public Character Owner;

        private List<SkillEntity> _entityList = new();
        private readonly Dictionary<SkillNames, SkillEntity> _entities = new();
        private readonly List<SkillNames> _allowedSkills = new();
        private VCharacterSkill _characterSkillInfo;

        /// <summary> 재시전 대기 여부 </summary>
        private SkillNames _isWaitingRecastSkill;

        public VCharacterSkill CharacterSkillInfo
        {
            get
            {
                if (_characterSkillInfo == null)
                {
                    VCharacter characterInfo = GameApp.GetSelectedCharacter();
                    if (characterInfo != null)
                    {
                        _characterSkillInfo = characterInfo.Skill;
                    }
                }

                return _characterSkillInfo;
            }
        }

        public SkillNames LastSkillName { get; set; }

        protected void Awake()
        {
            InitializeEntityList();
        }

        protected override void OnStart()
        {
            base.OnStart();
            RegisterEntities();
            InitializeBufferSystem();
        }

        private void InitializeEntityList()
        {
            _entityList.Clear();
            _entityList.AddRange(GetComponentsInChildren<SkillEntity>());
        }

        private void RegisterEntities()
        {
            _entities.Clear();
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i] != null)
                {
                    _entityList[i].Initialize();
                    if (!_entities.ContainsKey(_entityList[i].Name))
                    {
                        LogProgress("미리 생성된 기술을 등록합니다. {0} ({1})", _entityList[i].Name, _entityList[i].Name.ToLogString());
                        _entities.Add(_entityList[i].Name, _entityList[i]);
                    }
                }
            }
        }

        #region Global

        protected override void RegisterGlobalEvent()
        {
            base.RegisterGlobalEvent();

            if (Owner.IsPlayer)
            {
                GlobalEvent.Register(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
                GlobalEvent<SkillNames, int>.Register(GlobalEventType.GAME_DATA_ADD_CHARACTER_SKILL, OnPlayerSkillAdd);
                GlobalEvent<SkillNames>.Register(GlobalEventType.GAME_DATA_REMOVE_CHARACTER_SKILL, OnPlayerSkillRemove);
                GlobalEvent<SkillNames, int>.Register(GlobalEventType.GAME_DATA_CHARACTER_SKILL_LEVEL_REFRESH, OnPlayerSkillLevelChange);
                GlobalEvent<ActionNames, SkillNames>.Register(GlobalEventType.GAME_DATA_ASSIGN_CHARACTER_SKILL, OnPlayerSkillAssign);
                GlobalEvent<SkillNames>.Register(GlobalEventType.GAME_DATA_UNASSIGN_CHARACTER_SKILL, OnPlayerSkillUnassign);
            }
        }

        protected override void UnregisterGlobalEvent()
        {
            base.UnregisterGlobalEvent();

            if (Owner.IsPlayer)
            {
                GlobalEvent.Unregister(GlobalEventType.PLAYER_CHARACTER_BATTLE_READY, OnPlayerCharacterBattleReady);
                GlobalEvent<SkillNames, int>.Unregister(GlobalEventType.GAME_DATA_ADD_CHARACTER_SKILL, OnPlayerSkillAdd);
                GlobalEvent<SkillNames>.Unregister(GlobalEventType.GAME_DATA_REMOVE_CHARACTER_SKILL, OnPlayerSkillRemove);
                GlobalEvent<SkillNames, int>.Unregister(GlobalEventType.GAME_DATA_CHARACTER_SKILL_LEVEL_REFRESH, OnPlayerSkillLevelChange);
                GlobalEvent<ActionNames, SkillNames>.Unregister(GlobalEventType.GAME_DATA_ASSIGN_CHARACTER_SKILL, OnPlayerSkillAssign);
                GlobalEvent<SkillNames>.Unregister(GlobalEventType.GAME_DATA_UNASSIGN_CHARACTER_SKILL, OnPlayerSkillUnassign);
            }
        }

        private void OnPlayerCharacterBattleReady()
        {
            SkillNames skillName;
            VSkill[] mySkills = CharacterSkillInfo.GetMySkills();

            if (mySkills.IsValid())
            {
                for (int i = 0; i < mySkills.Length; i++)
                {
                    skillName = mySkills[i].Name;
                    if (_entities.ContainsKey(skillName))
                    {
                        _entities[skillName].SetHUD();
                    }
                }
            }
        }

        private void OnPlayerSkillAdd(SkillNames skillName, int skillLevel)
        {
            LogInfo("새롭게 추가된 기술에 대응합니다. {0}, Lv:{1}", skillName.ToLogString(), skillLevel);

            OnPlayerSkillLevelChange(skillName, skillLevel);
        }

        private void OnPlayerSkillLevelChange(SkillNames skillName, int skillLevel)
        {
            LogInfo("레벨 변경된 기술에 대응합니다. {0}, Lv:{1}", skillName.ToLogString(), skillLevel);

            if (skillLevel == 0)
            {
                OnPlayerSkillRemove(skillName);
            }
            else
            {
                if (!_entities.ContainsKey(skillName))
                {
                    SpawnAndRegister(skillName, skillLevel);
                }
                else
                {
                    SetLevel(skillName, skillLevel);
                }
            }
        }

        private void OnPlayerSkillRemove(SkillNames skillName)
        {
            LogInfo("기술을 제거합니다. {0}", skillName);

            if (_entities.ContainsKey(skillName))
            {
                _entities[skillName].ResetLevel();
                _entities[skillName].Deactivate();
                _entities[skillName].RemovePassives();

                if (!TryPlayerSkill(skillName))
                {
                    _entities[skillName].Despawn();
                    _ = _entities.Remove(skillName);
                }
            }
        }

        private bool TryPlayerSkill(SkillNames skillName)
        {
            SkillAssetData characterSkill = ScriptableDataManager.Instance.FindSkillClone(skillName);

            if (characterSkill.IsValid())
            {
                if (_entities.ContainsKey(skillName))
                {
                    if (characterSkill.Owners.IsValidArray())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnPlayerSkillAssign(ActionNames actionName, SkillNames skillName)
        {
            if (CharacterSkillInfo == null) { return; }
            if (!Owner.IsAlive) { return; }

            LogProgress("할당된 기술에 대응합니다. {0}", skillName);

            if (_entities.ContainsKey(skillName))
            {
                int totalLevel = CharacterSkillInfo.FindTotalLevel(skillName);
                SetLevel(skillName, totalLevel);
                _entities[skillName].AssignSKill();
            }
        }

        private void OnPlayerSkillUnassign(SkillNames skillName)
        {
            if (CharacterSkillInfo == null) { return; }
            if (!Owner.IsAlive) { return; }

            LogProgress("할당 해제된 기술에 대응합니다. {0}", skillName);

            if (_entities.ContainsKey(skillName))
            {
                int totalLevel = CharacterSkillInfo.FindTotalLevel(skillName);
                SetLevel(skillName, totalLevel);
                _entities[skillName].UnssignSkill();
            }
        }

        #endregion Global

        public void OnBattleReady()
        {
            AddMySkills();
            AddEditorSkills();
            SetupAnimations();
        }

        private void AddMySkills()
        {
            VSkill[] mySkills = CharacterSkillInfo.GetMySkills();
            if (mySkills.IsValid())
            {
                foreach (VSkill skillInfo in mySkills)
                {
                    int totalLevel = skillInfo.GetTotalLevel();
                    if (_entities.ContainsKey(skillInfo.Name))
                    {
                        SetLevel(skillInfo.Name, totalLevel);
                    }
                    else
                    {
                        SpawnAndRegister(skillInfo.Name, totalLevel);
                    }
                }
            }
        }

        private void AddEditorSkills()
        {
            for (int i = 0; i < _entityList.Count; i++)
            {
                if (_entityList[i].AutoAddOnBattle)
                {
                    Add(_entityList[i].Name);
                }
            }
        }

        private void SetupAnimations()
        {
            foreach (KeyValuePair<SkillNames, SkillEntity> entity in _entities)
            {
                entity.Value.SetupAnimationsWithCondition();
            }
        }

        //
        public bool ContainsKey(SkillNames skillName)
        {
            if (_entities.IsValid())
            {
                if (_entities.ContainsKey(skillName))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckCastable(SkillNames skillName)
        {
            if (_entities.IsValid())
            {
                if (_entities.ContainsKey(skillName))
                {
                    if (_entities[skillName].Level > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void Add(SkillNames skillName)
        {
            if (!_entities.ContainsKey(skillName))
            {
                SpawnAndRegister(skillName, Owner.Level);
            }
            else
            {
                SetLevel(skillName, Owner.Level);
            }
        }

        private void SpawnAndRegister(SkillNames skillName, int skillLevel)
        {
            if (skillName == SkillNames.None)
            {
                return;
            }

            SkillEntity skillEntity = SpawnSkillEntity(skillName, skillLevel);
            if (skillEntity != null)
            {
                RegisterEntity(skillEntity);
            }
        }

        private SkillEntity SpawnSkillEntity(SkillNames skillName, int skillLevel)
        {
            SkillEntity skillEntity = ResourcesManager.SpawnSkillEntity(transform);
            if (skillEntity != null)
            {
                skillEntity.Setup(skillName, skillLevel, Owner);
                skillEntity.SetHUD();
                skillEntity.SpawnAttackEntity();
            }

            return skillEntity;
        }

        private void RegisterEntity(SkillEntity entity)
        {
            if (!_entityList.Contains(entity))
            {
                _entityList.Add(entity);
            }

            _entities.Add(entity.Name, entity);
        }

        public void RefreshLevelOfAllSkills()
        {
            VSkill[] mySkills = CharacterSkillInfo.GetMyCharacterSkills(Owner.Name);
            if (mySkills.IsValid())
            {
                LogProgress("모든 캐릭터 기술의 레벨을 갱신합니다.");

                for (int i = 0; i < mySkills.Length; i++)
                {
                    if (!ContainsKey(mySkills[i].Name))
                    {
                        // 등록되지 않은 기술 독립체의 레벨을 갱신하지 않습니다.
                        continue;
                    }

                    SetLevel(mySkills[i].Name, mySkills[i].GetTotalLevel());
                }
            }
        }

        public void RefreshLevelOfAssignedSkills()
        {
            VSkill[] mySkills = CharacterSkillInfo.GetMyCharacterSkills(Owner.Name);
            if (mySkills.IsValid())
            {
                LogProgress("할당된 모든 캐릭터 기술의 레벨을 갱신합니다.");

                for (int i = 0; i < mySkills.Length; i++)
                {
                    if (!ContainsKey(mySkills[i].Name))
                    {
                        // 등록되지 않은 기술 독립체의 레벨을 갱신하지 않습니다.
                        continue;
                    }

                    if (mySkills[i].ActionName == ActionNames.None)
                    {
                        // 할당하지 않은 기술은 제외합니다.
                        continue;
                    }

                    SetLevel(mySkills[i].Name, mySkills[i].GetTotalLevel());
                }
            }
        }

        public void RefreshLevelOfAwardedSkills()
        {
            VSkill[] mySkills = CharacterSkillInfo.GetMyCharacterSkills(Owner.Name);
            if (mySkills.IsValid())
            {
                LogProgress("포인트를 수여한 모든 캐릭터 기술의 레벨을 갱신합니다.");

                for (int i = 0; i < mySkills.Length; i++)
                {
                    if (!ContainsKey(mySkills[i].Name))
                    {
                        // 등록되지 않은 기술 독립체의 레벨을 갱신하지 않습니다.
                        continue;
                    }

                    if (mySkills[i].Level == 0)
                    {
                        // 포인트를 수여하지 않은 기술은 제외합니다.
                        continue;
                    }

                    SetLevel(mySkills[i].Name, mySkills[i].GetTotalLevel());
                }
            }
        }

        private void SetLevel(SkillNames skillName, int skillLevel)
        {
            if (_entities[skillName] != null)
            {
                _entities[skillName].Setup(skillName, skillLevel, Owner);
            }
        }

        #region 재사용 대기 (Cooldown)

        public bool CheckCooldowning(SkillNames skillName)
        {
            if (_entities.ContainsKey(skillName))
            {
                if (_entities[skillName].CheckCooldowning())
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckCooldowning(SkillCategories skillCategory)
        {
            if (skillCategory != SkillCategories.None)
            {
                for (int i = 0; i < _entityList.Count; i++)
                {
                    if (_entityList[i].Category == skillCategory)
                    {
                        if (_entityList[i].CheckCooldowning())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        #endregion 재사용 대기 (Cooldown)

        #region 기술 시전 (Cast Skill)

        /// <summary> 기술을 시전할 수 있는지 확인합니다. </summary>
        public bool TryCast(SkillNames skillName)
        {
            // 버퍼 시스템이 활성화되어 있으면 버퍼 통합 로직 사용
            if (IsBufferSystemEnabled())
            {
                return TryCastWithBufferSystem(skillName);
            }

            // 기존 로직 (버퍼 시스템 비활성화 시)
            return TryCastOriginal(skillName);
        }

        /// <summary> 기존 TryCast 로직 (버퍼 시스템 없이) </summary>
        private bool TryCastOriginal(SkillNames skillName)
        {
            if (Owner.IsCrowdControl)
            {
                LogWarning("{0}, 군중 제어 상태인 캐릭터는 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            VSkill skillInfo = CharacterSkillInfo.Find(skillName);
            if (skillInfo == null)
            {
                LogWarning("{0}, 할당되지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }
            if (!_entities.ContainsKey(skillInfo.Name))
            {
                LogWarning("{0}, 기술 독립체가 생성되어있지 않습니다.", skillInfo.Name.ToLogString());
                return false;
            }

            if (!CharacterSkillInfo.Contains(skillName))
            {
                LogWarning("{0}, 습득하지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }
            if (_allowedSkills.Count > 0 && !_allowedSkills.Contains(skillName))
            {
                LogWarning("{0}, 허용하지 않은 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            if (!CheckAnimationConditionsForCast(skillName))
            {
                return false;
            }

            return _entities[skillName].TryActivate();
        }

        /// <summary> 애니메이션 조건을 확인합니다. </summary>
        private bool CheckAnimationConditionsForCast(SkillNames skillName)
        {
            if (_isWaitingRecastSkill != SkillNames.None)
            {
                if (_isWaitingRecastSkill == skillName)
                {
                    LogWarning("{0}, 재시전 대기 중인 기술을 시전할 수 없습니다.", skillName.ToLogString());
                    return false;
                }
            }

            if (Owner.CharacterAnimator.IsDamaging)
            {
                LogWarning("{0}, 피격 중 기술을 시전할 수 없습니다.", skillName.ToLogString());
                return false;
            }

            SkillAnimationAsset skillAnimation = Owner.CharacterAnimator.GetPlayingAnimation();
            if (skillAnimation != null)
            {
                // 시전중인 기술을 재시전한다면
                if (IsSameSkillName(skillAnimation, skillName) || IsReplacedSkillBeingPlayed(skillAnimation, skillName))
                {
                    if (TryStopByRecast(skillName, skillAnimation))
                    {
                        // 재시전으로 기술을 정지할 수 있다면 기술을 시전할 수 없습니다.
                        return false;
                    }
                    else if (IsDefaultAttack(skillAnimation, skillName))
                    {
                        // 재시전하는 기술이 기본 공격이라면 기술을 시전할 수 없습니다.
                        return false;
                    }
                }
                else
                {
                    LogProgressSkillMismatch(skillAnimation, skillName);
                }
            }

            return true;
        }

        private bool IsSameSkillName(SkillAnimationAsset skillAnimation, SkillNames skillName)
        {
            return skillAnimation.SkillName == skillName;
        }

        private bool IsReplacedSkillBeingPlayed(SkillAnimationAsset skillAnimation, SkillNames skillName)
        {
            SkillAssetData skillData = ScriptableDataManager.Instance.FindSkillClone(skillAnimation.SkillName);
            return skillName == skillData.ReplacedSkill;
        }

        private bool IsDefaultAttack(SkillAnimationAsset skillAnimation, SkillNames skillName)
        {
            return skillAnimation.SkillName != SkillNames.None && skillName.IsDefaultAttack();
        }

        private void LogProgressSkillMismatch(SkillAnimationAsset skillAnimation, SkillNames skillName)
        {
            if (Log.LevelProgress)
            {
                LogProgress("재생 중인 애니메이션의 기술 이름({0})과 명령에 할당된 기술 이름({1})이 다릅니다. 애니메이션 조건을 확인할 수 없어, 재시전으로 기술을 종료할 수 없습니다.",
                    skillAnimation.SkillName.ToLogString(), skillName.ToLogString());
            }
        }

        /// <summary> 재시전 조건을 확인합니다. </summary>
        private bool TryStopByRecast(SkillNames skillName, SkillAnimationAsset skillAnimation)
        {
            if (skillAnimation.UseStopByRecast)
            {
                if (_isWaitingRecastSkill == SkillNames.None)
                {
                    // 기술 엔티티가 존재하는지 확인
                    if (!_entities.ContainsKey(skillName))
                    {
                        LogWarning("재시전 취소 시도 중 기술 엔티티를 찾을 수 없음: {0}", skillName.ToLogString());
                        return false;
                    }

                    float lastTime = Time.time - _entities[skillName].CastTime;
                    if (lastTime > skillAnimation.RecastStopAnimationTime)
                    {
                        LogProgress("기술 재시전으로 재생 중인 기술({0})의 애니메이션({1})을 정지합니다.", skillAnimation.SkillName.ToLogString(), skillAnimation.AnimationName);
                        Owner.CharacterAnimator.StopSequenceSkillAnimation(skillAnimation);
                        _ = StartXCoroutine(ProgressRecastResumeAnimation(skillAnimation));
                        return true;
                    }
                    else
                    {
                        LogProgress("재시전 취소 시간 조건 미충족: {0} (경과시간: {1:F2}s, 필요시간: {2:F2}s)",
                            skillName.ToLogString(), lastTime, skillAnimation.RecastStopAnimationTime);
                    }
                }
                else
                {
                    LogProgress("재시전 대기 중이므로 재시전 취소 불가: {0}", skillName.ToLogString());
                }
            }
            else
            {
                LogProgress("재시전 취소 기능이 비활성화된 기술: {0}", skillName.ToLogString());
            }

            return false;
        }

        private IEnumerator ProgressRecastResumeAnimation(SkillAnimationAsset skillAnimation)
        {
            _isWaitingRecastSkill = skillAnimation.SkillName;

            LogProgress("{0}, 지금으로 부터 {1}초가 지나면 재시전으로 정지한 기술을 재생할 수 있습니다.", skillAnimation.SkillName.ToLogString(), skillAnimation.RecastResumeAnimationDelayTime);

            yield return new WaitForSeconds(skillAnimation.RecastResumeAnimationDelayTime);

            _isWaitingRecastSkill = SkillNames.None;

            LogProgress("{0}, {1}초가 지나 재시전으로 정지한 기술을 재생할 수 있습니다.", skillAnimation.SkillName.ToLogString(), skillAnimation.RecastResumeAnimationDelayTime);
        }

        public void DoCast(SkillNames skillName)
        {
            LogInfo("기술을 시전합니다. {0}", skillName.ToLogString());

            if (LastSkillName != skillName)
            {
                DeactivateLastSkill();
                LastSkillName = skillName;
            }

            if (_entities.ContainsKey(skillName))
            {
                _entities[skillName].Activate();
            }
            else
            {
                Log.Error("기술({0})을 찾을 수 없어, 시전할 수 없습니다.", skillName.ToLogString());
            }
        }

        #endregion 기술 시전 (Cast Skill)

        #region 최근 기술 (Last Skill)

        private void DeactivateLastSkill()
        {
            if (!_entities.ContainsKey(LastSkillName))
            {
                return;
            }

            SkillEntity skillEntity = _entities[LastSkillName];
            if (skillEntity == null)
            {
                return;
            }

            if (skillEntity.CheckMaxOrderOfGround())
            {
                skillEntity.LogProgressRestOrderGroundLastSkill();
                skillEntity.SetFirstSkillOrderGround();
                skillEntity.StopDropOrderByGround();
            }

            if (skillEntity.Application == SkillApplications.Hitmark)
            {
                LogProgress("이전 기술의 히트마크를 비활성화합니다. {0}({1})", LastSkillName, LastSkillName.ToLogString());

                skillEntity.SetForceDespawnVisualEffectFeedback();
                skillEntity.DeactivateOnInterrupt();
            }
        }

        public void ResetLastSkill()
        {
            LogProgress("이전 기술을 초기화합니다. {0}", LastSkillName.ToLogString());

            LastSkillName = SkillNames.None;
        }

        #endregion 최근 기술 (Last Skill)

        #region 찾기 (Find)

        public SkillEntity Find(SkillNames skillName)
        {
            if (_entities.ContainsKey(skillName))
            {
                return _entities[skillName];
            }

            return null;
        }

        public SkillEntity[] FindAll(SkillAnimationAsset skillAnimationAsset)
        {
            List<SkillEntity> entities = new();

            if (skillAnimationAsset.ReplacedSkillName != SkillNames.None)
            {
                if (_entities.ContainsKey(skillAnimationAsset.ReplacedSkillName))
                {
                    entities.Add(_entities[skillAnimationAsset.ReplacedSkillName]);
                }
            }

            if (_entities.ContainsKey(skillAnimationAsset.SkillName))
            {
                entities.Add(_entities[skillAnimationAsset.SkillName]);
            }

            return entities.ToArray();
        }

        public List<SkillEntity> FindAll(SkillCategories skillCategory)
        {
            List<SkillEntity> result = new();

            List<SkillNames> skillNames = JsonDataManager.GetSkillNames(Owner.Name, skillCategory);
            for (int i = 0; i < skillNames.Count; i++)
            {
                SkillEntity skillEntity = Find(skillNames[i]);
                if (skillEntity != null)
                {
                    result.Add(skillEntity);
                }
            }

            return result;
        }

        /// <summary>
        /// 재사용 대기중인 기술 독립체를 찾습니다.
        /// </summary>
        public SkillEntity[] FindUsingCooldownSkills()
        {
            List<SkillEntity> result = new();

            List<SkillNames> skillNames = JsonDataManager.GetSkillNames(Owner.Name);
            for (int i = 0; i < skillNames.Count; i++)
            {
                SkillEntity skillEntity = Find(skillNames[i]);
                if (skillEntity != null)
                {
                    if (skillEntity.IsCooldown)
                    {
                        if (skillEntity.Category == SkillCategories.Dash)
                        {
                            continue;
                        }

                        result.Add(skillEntity);
                    }
                }
            }

            return result.ToArray();
        }

        public int FindLevel(SkillNames skillName)
        {
            if (_entities.ContainsKey(skillName))
            {
                return _entities[skillName].Level;
            }

            return 0;
        }

        #endregion 찾기 (Find)

        #region 기술 허용 (Allow Skill)

        public void AddAllowedSkills(SkillNames[] allowedSkills)
        {
            if (allowedSkills.IsValidArray())
            {
                for (int i = 0; i < allowedSkills.Length; i++)
                {
                    if (!_allowedSkills.Contains(allowedSkills[i]))
                    {
                        _allowedSkills.Add(allowedSkills[i]);

                        LogProgress("{0}, 허용하는 기술을 추가합니다. 허용하는 기술 이외의 기술을 사용할 수 없습니다. 허용하는 기술의 수: {1}", allowedSkills[i].ToLogString(), _allowedSkills.Count);
                    }
                }

                GlobalEvent.Send(GlobalEventType.GAME_DATA_REGISTER_ALLOWED_SKILL);
            }
        }

        public void RemoveAllowedSkills(SkillNames[] allowedSkills)
        {
            if (allowedSkills.IsValidArray())
            {
                for (int i = 0; i < allowedSkills.Length; i++)
                {
                    if (_allowedSkills.Contains(allowedSkills[i]))
                    {
                        _ = _allowedSkills.Remove(allowedSkills[i]);

                        LogProgress("{0}, 허용하는 기술을 삭제합니다. 허용하는 기술 이외의 기술을 사용할 수 없습니다. 허용하는 기술의 수: {1}", allowedSkills[i].ToLogString(), _allowedSkills.Count);
                    }
                }

                GlobalEvent.Send(GlobalEventType.GAME_DATA_UNREGISTER_ALLOWED_SKILL);
            }
        }

        #endregion 기술 허용 (Allow Skill)

        #region 능력치 (Stat)

        public int CalculateSkillLevel(SkillAssetData skillData, int skillLevel)
        {
            if (skillData.IsValid())
            {
                if (skillData.UseAddLevel)
                {
                    skillLevel += LoadAddAllSkillLevel(skillData.Name, skillLevel);
                    skillLevel += LoadAddAssignedSkillLevel(skillData.Name, skillLevel);
                    skillLevel += LoadAddAwardedSkillLevel(skillData.Name, skillLevel);
                    skillLevel += LoadAddCategorySkillLevel(skillData, skillLevel);

                    int maxLevel = LoadMaxCategorySkillLevel(skillData, skillLevel);
                    if (maxLevel > 0)
                    {
                        skillLevel = Mathf.Min(skillLevel, maxLevel);
                    }
                }
            }

            return skillLevel;
        }

        private int LoadAddAllSkillLevel(SkillNames skillName, int skillLevel)
        {
            int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAllSkillLevel);
            if (additionalLevel > 0)
            {
                LogProgress("기술({0})의 레벨({1})이 '모든 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                return additionalLevel;
            }

            return 0;
        }

        private int LoadAddAwardedSkillLevel(SkillNames skillName, int skillLevel)
        {
            VSkill skillInfo = CharacterSkillInfo.Find(skillName);
            if (skillInfo != null)
            {
                if (skillInfo.Level > 0)
                {
                    // 포인트를 수여한 기술의 레벨을 능력치에 따라 오릅니다.
                    int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAwardedSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '수여한 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());

                        return additionalLevel;
                    }
                }
            }

            return 0;
        }

        private int LoadAddAssignedSkillLevel(SkillNames skillName, int skillLevel)
        {
            VSkill skillInfo = CharacterSkillInfo.Find(skillName);
            if (skillInfo != null)
            {
                if (skillInfo.ActionName != ActionNames.None)
                {
                    // 할당된 기술의 레벨을 능력치에 따라 오릅니다.
                    int additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAssignSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '할당된 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillName.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                }
            }

            return 0;
        }

        private int LoadAddCategorySkillLevel(SkillAssetData skillData, int skillLevel)
        {
            int additionalLevel;
            switch (skillData.Category)
            {
                case SkillCategories.Basic:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddBasicSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '기본 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Core:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddCoreSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '핵심 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Assistant:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddAssistantSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '보조 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Power:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddPowerSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '숙련 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;

                case SkillCategories.Ultimate:
                    additionalLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.AddUltimateSkillLevel);
                    if (additionalLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '궁극 기술 레벨 증가' 능력치 효과(+{2})로 인해 증가합니다.", skillData.Name.ToLogString(), skillLevel, additionalLevel.ToSelectString());
                        return additionalLevel;
                    }
                    break;
            }

            return 0;
        }

        private int LoadMaxCategorySkillLevel(SkillAssetData skillData, int skillLevel)
        {
            int skillMaxLevel;
            switch (skillData.Category)
            {
                case SkillCategories.Basic:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxBasicSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '기본 기술 최대 레벨' 능력치 효과({2})로 인해 레벨이 설정됩니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Core:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxCoreSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '핵심 기술 최대 레벨' 능력치 효과({2})로 인해 레벨이 설정됩니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Assistant:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxAssistantSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '보조 기술 최대 레벨' 능력치 효과({2})로 인해 레벨이 설정됩니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Power:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxPowerSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '숙련 기술 최대 레벨' 능력치 효과({2})로 인해 레벨이 설정됩니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;

                case SkillCategories.Ultimate:
                    skillMaxLevel = Owner.Stat.FindValueOrDefaultToInt(StatNames.MaxUltimateSkillLevel);
                    if (skillMaxLevel > 0)
                    {
                        LogProgress("기술({0})의 레벨({1})이 '궁극 기술 최대 레벨' 능력치 효과({2})로 인해 레벨이 설정됩니다.", skillData.Name.ToLogString(), skillLevel, skillMaxLevel.ToSelectString());
                        return skillMaxLevel;
                    }
                    break;
            }

            return 0;
        }

        #endregion 능력치 (Stat)
    }
}