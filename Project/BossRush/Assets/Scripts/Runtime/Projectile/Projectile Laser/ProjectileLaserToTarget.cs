using Sirenix.OdinInspector;
using System.Collections;
using TeamSuneat.Audio;
using UnityEngine;

namespace TeamSuneat
{
    public class ProjectileLaserToTarget : XProjectileLaser, IAnimatorStateMachine
    {
        [Title("#ProjectileLaserToTarget")]
        public int GroupIndex;

        [FoldoutGroup("#Components")]
        public LineRenderer LaserLine;

        [FoldoutGroup("#Components")]
        public LineRenderer PredictorLine;

        [FoldoutGroup("#Components")]
        public Animator LaserLineAnimator;

        [FoldoutGroup("#Components")]
        public AttackEntity LaserAttackEntity;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 라인 사용")]
        public bool UseLine;

        [FoldoutGroup("#Values")]
        [LabelText("플레이어 캐릭터를 타겟으로 지정한다")]
        public bool UsePlayerTarget;

        [FoldoutGroup("#Values")]
        [LabelText("레이저의 타겟을 따로 지정한다")]
        public bool UseLaserTarget;

        [FoldoutGroup("#Values")]
        [LabelText("레이저가 따라가는 타겟")]
        [ShowIf("UseLaserTarget")]
        public ProjectileLaserTarget LaserTarget;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 지연시간")]
        public float DelayTime;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 지속시간")]
        public float Duration;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 거리")]
        public float Distance;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 회전을 사용한다")]
        public bool UseRotate;

        [FoldoutGroup("#Values")]
        [LabelText("레이저 회전 속도")]
        [ShowIf("UseRotate")]
        [Range(0, 1)]
        public float RotateSpeed;

        [FoldoutGroup("#Values")]
        [LabelText("레이캐스트를 사용한다")]
        public bool UseRaycast;

        [FoldoutGroup("#Values")]
        [LabelText("충돌 레이어")]
        [ShowIf("UseRaycast")]
        public LayerMask CollisionLayer;

        [FoldoutGroup("#Values")]
        [LabelText("무시하는 충돌 태그")]
        [ShowIf("UseRaycast")]
        public GameTags IgnoreCollisionTag;

        [FoldoutGroup("#Values")]
        [LabelText("목표 레이어")]
        [ShowIf("UseRaycast")]
        public LayerMask TargetLayer;

        [FoldoutGroup("#Visual Effect")]
        [LabelText("총구 앞 이펙트 프리펩")]
        public GameObject GunFrontPrefab;

        [FoldoutGroup("#Visual Effect")]
        [LabelText("총구 앞 이펙트 생성 간격 시간")]
        public float IntervalTimeGunFrontFX;

        [FoldoutGroup("#Visual Effect")]
        [LabelText("충돌 이펙트 프리펩")]
        public GameObject HitPrefab;

        [FoldoutGroup("#Visual Effect")]
        [LabelText("충돌 이펙트 생성 간격 시간")]
        public float IntervalTimeHitFX;

        [FoldoutGroup("#Sound Effect")]
        [LabelText("레이저 시작시 효과음")]
        public string StartSFX;

        [FoldoutGroup("#Sound Effect")]
        [LabelText("레이저 진행중 효과음")]
        public string ProcessSFX;

        //
        private Transform _target;

        private Vector3 _direction;

        private Vector3 _hitPosition;

        private bool _isAttacking;

        private Coroutine _laserCoroutine;

        private Coroutine _gunfrontFXCoroutine;

        private Coroutine _hitFXCoroutine;

        private AudioObject _processAudioObject;

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            LaserLine = this.FindComponent<LineRenderer>("Laser LineRenderer");

            PredictorLine = this.FindComponent<LineRenderer>("Predictor LineRenderer");

            LaserLineAnimator = GetComponent<Animator>();

            LaserAttackEntity = GetComponentInChildren<AttackEntity>();
        }

        protected override void OnStart()
        {
            base.OnStart();

            Initialize();
        }

        private void Initialize()
        {
            if (LaserAttackEntity != null)
            {
                LaserAttackEntity.Initialize();
            }

            StopMoveLaserTarget();

            RotateImmediateToTarget(1);
        }

        private void SetTarget()
        {
            if (UsePlayerTarget)
            {
                if (CharacterManager.playerCharacter != null)
                {
                    _target = CharacterManager.playerCharacter.transform;
                }
            }
            else if (UseLaserTarget)
            {
                if (LaserTarget != null)
                {
                    _target = LaserTarget.transform;
                }
            }
        }

        private void StartMoveLaserTarget()
        {
            if (LaserTarget != null)
            {
                LaserTarget.StartMove();
            }
        }

        private void StopMoveLaserTarget()
        {
            if (LaserTarget != null)
            {
                LaserTarget.StopMove();
            }
        }

        public override void StartLaser()
        {
            _isAttacking = false;

            SetTarget();

            StartMoveLaserTarget();

            AnimationSpawn();

            SpawnStartSFX();

            SpawnProcessSFX();

            if (UseRotate)
            {
                _laserCoroutine = StartXCoroutine(ProcessRaycast());
            }
            else
            {
                _laserCoroutine = StartXCoroutine(ProcessFollow());
            }

            _hitFXCoroutine = StartXCoroutine(ProcessSpawnHitFX());

            _gunfrontFXCoroutine = StartXCoroutine(ProcessSpawnGunFrontFX());
        }

        public override void StopLaser()
        {
            StopXCoroutine(ref _laserCoroutine);
            StopXCoroutine(ref _gunfrontFXCoroutine);
            StopXCoroutine(ref _hitFXCoroutine);

            AnimationDespawn();

            StopMoveLaserTarget();

            RotateImmediateToTarget(1);

            SetLinePositions();

            DespawnProcessSFX();
        }

        private IEnumerator ProcessRaycast()
        {
            float elapsedTime = 0f;

            WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();

            if (DelayTime > 0f)
            {
                RotateToTarget();

                while (elapsedTime < DelayTime)
                {
                    yield return waitFixedUpdate;

                    elapsedTime += Time.fixedDeltaTime;

                    if (_target != null)
                    {
                        RaycastToTarget();

                        TryAttackTarget();

                        SetLinePositions();
                    }
                }

                _isAttacking = true;
            }

            elapsedTime = 0f;

            while (elapsedTime < Duration)
            {
                yield return waitFixedUpdate;

                elapsedTime += Time.fixedDeltaTime;

                if (_target != null)
                {
                    RotateToTarget();

                    RaycastToTarget();

                    TryAttackTarget();

                    SetLinePositions();
                }
            }

            _isAttacking = false;

            AnimationDespawn();

            _laserCoroutine = null;
        }

        private IEnumerator ProcessFollow()
        {
            float elapsedTime = 0f;

            WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();

            if (DelayTime > 0f)
            {
                FollowToTarget();

                while (elapsedTime < DelayTime)
                {
                    yield return waitFixedUpdate;

                    elapsedTime += Time.fixedDeltaTime;

                    if (_target != null)
                    {
                        RaycastToTarget();

                        TryAttackTarget();

                        SetLinePositions();
                    }
                }

                _isAttacking = true;
            }

            elapsedTime = 0f;

            while (elapsedTime < Duration)
            {
                yield return waitFixedUpdate;

                elapsedTime += Time.fixedDeltaTime;

                if (_target != null)
                {
                    FollowToTarget();

                    RaycastToTarget();

                    TryAttackTarget();

                    SetLinePositions();
                }
            }

            _isAttacking = false;

            AnimationDespawn();

            _laserCoroutine = null;
        }

        private IEnumerator ProcessSpawnGunFrontFX()
        {
            if (DelayTime > 0f)
            {
                yield return new WaitForSeconds(DelayTime);
            }
            else
            {
                yield return null;
            }

            float elapsedTime = 0f;

            WaitForSeconds waitForInterval = new WaitForSeconds(IntervalTimeGunFrontFX);

            while (elapsedTime < Duration)
            {
                yield return waitForInterval;

                elapsedTime += IntervalTimeGunFrontFX;

                SpawnGunFrontFX();
            }

            DespawnProcessSFX();

            _gunfrontFXCoroutine = null;
        }

        private IEnumerator ProcessSpawnHitFX()
        {
            if (DelayTime > 0f)
            {
                yield return new WaitForSeconds(DelayTime);
            }
            else
            {
                yield return null;
            }

            float elapsedTime = 0f;

            float duration = Duration - 1f;

            WaitForSeconds waitForInterval = new WaitForSeconds(IntervalTimeHitFX);

            while (elapsedTime < duration)
            {
                yield return waitForInterval;

                elapsedTime += IntervalTimeHitFX;

                SpawnHitFX();
            }

            _hitFXCoroutine = null;
        }

        private void RotateImmediateToTarget(float rotateSpeed)
        {
            if (transform != null)
            {
                if (_target != null && _target.transform != null)
                {
                    transform.RotateForFixedUpdate(_target.transform, rotateSpeed);
                }

                _direction = AngleEx.ToVector3(transform.rotation);
            }
        }

        private void RotateToTarget()
        {
            if (false == UseRotate)
            {
                return;
            }

            if (transform == null)
            {
                return;
            }

            if (_target != null && _target.transform != null)
            {
                transform.RotateForFixedUpdate(_target.transform, RotateSpeed);
            }

            _direction = AngleEx.ToVector3(transform.rotation);
        }

        private void RaycastToTarget()
        {
            if (false == UseRaycast)
            {
                return;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(position, _direction, Distance, CollisionLayer);

            if (hits == null)
            {
                return;
            }

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == null)
                {
                    continue;
                }

                if (hits[i].collider.CompareTag(IgnoreCollisionTag))
                {
                    continue;
                }

                _hitPosition = hits[i].point;

                DebugEx.DrawLine(position, _hitPosition, Color.green, 1f);
            }
        }

        private void FollowToTarget()
        {
            if (LaserTarget == null)
            {
                return;
            }

            _direction = _target.position - position;

            _direction.Normalize();
        }

        private void TryAttackTarget()
        {
            if (false == _isAttacking)
            {
                return;
            }

            if (LaserAttackEntity == null)
            {
                return;
            }

            if (LaserAttackEntity.IsCooldown)
            {
                return;
            }

            RaycastHit2D[] hits = Physics2D.RaycastAll(position, _direction, Distance, TargetLayer);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider == null)
                {
                    continue;
                }

                Vital targetVital = VitalManager.Instance.Find(hits[i].collider);

                if (targetVital != null)
                {
                    LaserAttackEntity.AddTarget(targetVital);

                    LaserAttackEntity.Activate();

                    break;
                }
            }
        }

        private void SetLinePositions()
        {
            if (false == UseLine)
            {
                return;
            }

            Vector3[] positions = new Vector3[] { position, _hitPosition };
            Vector3[] defaultPositions = new Vector3[] { Vector3.zero, Vector3.zero };

            if (_isAttacking)
            {
                SetLaserLinePosition(positions);
                SetPredictorLinePosition(defaultPositions);
            }
            else
            {
                SetLaserLinePosition(defaultPositions);
                SetPredictorLinePosition(positions);
            }
        }

        private void SetLaserLinePosition(Vector3[] positions)
        {
            if (LaserLine != null)
            {
                LaserLine.SetPositions(positions);
            }
        }

        private void SetPredictorLinePosition(Vector3[] positions)
        {
            if (PredictorLine != null)
            {
                PredictorLine.SetPositions(positions);
            }
        }

        #region Animation

        public void OnAnimatorStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public void OnAnimatorStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName("Spawn"))
            {
                if (DelayTime <= 0f)
                {
                    _isAttacking = true;
                }
            }
        }

        private void AnimationSpawn()
        {
            if (LaserLineAnimator != null)
            {
                LaserLineAnimator.UpdateAnimatorTriggerIfExists("Spawn");
            }
        }

        private void AnimationDespawn()
        {
            if (LaserLineAnimator != null)
            {
                LaserLineAnimator.UpdateAnimatorTriggerIfExists("Despawn");
            }
        }

        #endregion Animation

        #region Visual Effect

        private void SpawnGunFrontFX()
        {
            if (GunFrontPrefab != null)
            {
                Vector3 spawnPosition = position + GetRandomVFXOffset();

                ResourcesManager.Instantiate<AutoDespawn>(GunFrontPrefab, spawnPosition);
            }
        }

        private void SpawnHitFX()
        {
            if (HitPrefab != null)
            {
                Vector3 spawnPosition = _hitPosition + GetRandomVFXOffset();

                ResourcesManager.Instantiate<AutoDespawn>(HitPrefab, spawnPosition);
            }
        }

        private Vector3 GetRandomVFXOffset()
        {
            float offsetX = RandomEx.Range(-0.1f, 0.1f);

            float offsetY = RandomEx.Range(-0.1f, 0.1f);

            return new Vector3(offsetX, offsetY);
        }

        #endregion Visual Effect

        #region Sound Effect

        protected void SpawnStartSFX()
        {
            AudioClip clip = ResourcesManager.LoadClip(StartSFX);

            AudioManager.PlaySFXOneShot(clip);
        }

        protected void SpawnProcessSFX()
        {
            AudioClip clip = ResourcesManager.LoadClip(ProcessSFX);

            _processAudioObject = AudioManager.PlaySFXLoop(clip);
        }

        protected void DespawnProcessSFX()
        {
            if (_processAudioObject != null)
            {
                AudioManager.StopSFX(_processAudioObject);

                _processAudioObject = null;
            }
        }

        #endregion Sound Effect
    }
}