using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TeamSuneat.Projectiles;

using UnityEngine;

namespace TeamSuneat
{
    public class DetectSystem : XBehaviour
    {
        [Title("#Detect System")]
        public Character Owner;
        public Projectile OwnerProjectile;
        public DetectTypes DetectType;

        public bool UseCustomDetectMask;
        [EnableIf("UseCustomDetectMask")] public LayerMask DetectMask;

        [FoldoutGroup("Shape")]
        public DetectShapes DetectShape;

        [FoldoutGroup("Shape")]
        [EnableIf("@DetectShape == DetectShapes.Circle || DetectShape == DetectShapes.Arc")]
        public float DetectRadius;

        [FoldoutGroup("Shape")]
        [EnableIf("DetectShape", DetectShapes.Box)]
        public Vector2 DetectBoxSize;

        [FoldoutGroup("Shape")]
        [EnableIf("DetectShape", DetectShapes.Arc)]
        [Range(0, 180)]
        public float DetectArcAngle;

        [SuffixLabel("탐지한 바이탈은 일정 시간 탐지하지 않는다")]
        public bool IsSaveIgnoreVitals;

        [EnableIf("IsSaveIgnoreVitals")]
        [SuffixLabel("등록된 무시 바이탈이 없을 때, 탐지한 첫번째 바이탈은 탐지하지 않는다")]
        public bool IsSaveFirstToIgnoreVitals;

        [EnableIf("IsSaveIgnoreVitals")]
        [SuffixLabel("탐지한 바이탈을 탐지하지 않는 시간을 설정한다")]
        public float IgnoreVitalUnregisterTime = 0.1f;

        [FoldoutGroup("#String")] public string DetectTypeString;
        [FoldoutGroup("#String")] public string DetectShapeString;

        private VitalDetector _vitalDetector;
        private readonly List<Vital> _ignoreVitals = new();
        private Coroutine _detectingCorotuine;

        #region Editor

        public override void AutoGetComponents()
        {
            base.AutoGetComponents();

            Owner = this.FindFirstParentComponent<Character>();
            OwnerProjectile = this.FindFirstParentComponent<Projectile>();
        }

        public override void AutoSetting()
        {
            base.AutoSetting();

            if (DetectType != DetectTypes.None)
            {
                DetectTypeString = DetectType.ToString();
            }

            if (DetectShape != DetectShapes.None)
            {
                DetectShapeString = DetectShape.ToString();
            }
        }

        private void OnValidate()
        {
            _ = EnumEx.ConvertTo(ref DetectType, DetectTypeString);
            _ = EnumEx.ConvertTo(ref DetectShape, DetectShapeString);
        }

        private void OnDrawGizmosSelected()
        {
            if (DetectShape == DetectShapes.Box)
            {
                TSGizmoEx.DrawGizmoRectangle(position, DetectBoxSize, TSColors.Detect);
            }
            else if (DetectShape == DetectShapes.Circle)
            {
                TSGizmoEx.DrawWireSphere(position, DetectRadius, TSColors.Detect);
            }
        }

        public override void AutoNaming()
        {
            if (Owner != null)
            {
                SetGameObjectName("Detect (" + Owner.Name.ToString() + ")");
            }
        }

        #endregion Editor

        protected void Start()
        {
            CreateDetector();
            SetDetectArea();
        }

        public void OnDespawn()
        {
            StopDetecting();
            ClearIgnoreVital();
        }

        public void CreateDetector()
        {
            _vitalDetector = new VitalDetector();
            if (Owner != null)
            {
                if (UseCustomDetectMask)
                {
                    _vitalDetector.Setup(Owner.SID, DetectMask, DetectType);
                }
                else
                {
                    _vitalDetector.Setup(Owner.SID, Owner.TargetMask, DetectType);
                }
            }
            else if (UseCustomDetectMask)
            {
                _vitalDetector.Setup(0, DetectMask, DetectType);
            }
            else
            {
                Log.Error("vital detector를 초기화할 수 없습니다.");
            }
        }

        private void SetDetectArea()
        {
            if (_vitalDetector != null)
            {
                if (DetectShape == DetectShapes.Circle)
                {
                    _vitalDetector.SetDetectArea(DetectShape, DetectRadius);
                }
                if (DetectShape == DetectShapes.Arc)
                {
                    _vitalDetector.SetDetectArea(DetectShape, DetectRadius, DetectArcAngle);
                }
                else if (DetectShape == DetectShapes.Box)
                {
                    _vitalDetector.SetDetectArea(DetectShape, DetectBoxSize);
                }
            }
        }

        #region Detect Timer

        public void StartDetecting()
        {
            if (_vitalDetector == null) { return; }
            if (_detectingCorotuine != null) { return; }

            _detectingCorotuine = StartXCoroutine(ProcessDetecting());
        }

        public void StopDetecting()
        {
            StopXCoroutine(ref _detectingCorotuine);
        }

        private IEnumerator ProcessDetecting()
        {
            bool isDetected = false;
            while (!isDetected)
            {
                DoDetect();

                if (Owner != null && Owner.CheckTargetCharacterAlive())
                {
                    isDetected = true;
                }
                else
                {
                    isDetected = false;
                    yield return new WaitForSeconds(1f);
                }
            }

            _detectingCorotuine = null;
        }

        #endregion Detect Timer

        public void DoDetect()
        {
            List<Vital> detectedVitalList = DoDetectVitalList();
            if (detectedVitalList.IsValid())
            {
                Vital detectedVital = detectedVitalList[0];
                if (detectedVital != null)
                {
                    if (Owner != null)
                    {
                        Owner.SetTarget(detectedVital);
                    }

                    if (OwnerProjectile != null)
                    {
                        OwnerProjectile.SetTarget(detectedVital);
                    }

                    if (IsSaveIgnoreVitals)
                    {
                        RegisterIgnoreVital(detectedVital);
                        _ = CoroutineNextTimer(IgnoreVitalUnregisterTime, () =>
                        {
                            UnregisterIgnoreVital(detectedVital);
                        });
                    }
                }
            }
        }

        public List<Vital> DoDetectVitalList()
        {
            Log.Progress(LogTags.Detect, "근처 바이탈 목록을 탐지합니다: {0}", this.GetHierarchyPath());
            if (_vitalDetector != null)
            {
                _vitalDetector.SetComparerPosition(position);
                List<Vital> detectedVitalList = null;
                if (OwnerProjectile != null)
                {
                    // 부모 발사체가 있다면 발사체의 방향에 따라 바이탈을 탐지합니다.
                    detectedVitalList = _vitalDetector.DoDetectVitalList(position, OwnerProjectile.IsDirectionRight);
                }
                else if (Owner != null)
                {
                    detectedVitalList = _vitalDetector.DoDetectVitalList(position, Owner.IsFacingRight);
                }

                if (detectedVitalList.IsValid())
                {
                    ApplyIgnoreVitals(ref detectedVitalList);
                }

                return detectedVitalList;
            }
            else
            {
                Log.Warning("바이탈 탐지기 클래스가 설정되지 않았습니다. 탐지를 진행할 수 없습니다: {0}", this.GetHierarchyPath());
            }

            return null;
        }

        // Ignore Vital

        private void RegisterIgnoreVital(Vital vital)
        {
            if (!_ignoreVitals.Contains(vital))
            {
                _ignoreVitals.Add(vital);

                if (vital.Owner != null)
                {
                    Log.Info(LogTags.Detect, "무시하는 바이탈({0})을 등록합니다. {1}", vital.Owner.GetHierarchyName(), _ignoreVitals.Count);
                }
                else if (vital != null)
                {
                    Log.Info(LogTags.Detect, "무시하는 바이탈({0})을 등록합니다. {1}", vital.GetHierarchyPath(), _ignoreVitals.Count);
                }
            }
        }

        private void UnregisterIgnoreVital(Vital vital)
        {
            if (_ignoreVitals.Contains(vital))
            {
                _ = _ignoreVitals.Remove(vital);

                if (vital.Owner != null)
                {
                    Log.Info(LogTags.Detect, "무시하는 바이탈({0})을 등록 해제합니다. {1}", vital.Owner.GetHierarchyName(), _ignoreVitals.Count);
                }
                else if (vital != null)
                {
                    Log.Info(LogTags.Detect, "무시하는 바이탈({0})을 등록 해제합니다. {1}", vital.GetHierarchyPath(), _ignoreVitals.Count);
                }
            }
        }

        public void ClearIgnoreVital()
        {
            _ignoreVitals.Clear();
        }

        private void ApplyIgnoreVitals(ref List<Vital> detectedVitalList)
        {
            if (IsSaveIgnoreVitals)
            {
                if (_ignoreVitals.IsValid())
                {
                    foreach (Vital ignoreVital in _ignoreVitals)
                    {
                        if (detectedVitalList.Contains(ignoreVital))
                        {
                            _ = detectedVitalList.Remove(ignoreVital);
                        }
                    }
                }
                else if (IsSaveFirstToIgnoreVitals)
                {
                    detectedVitalList.RemoveAt(0);
                }
            }
        }
    }
}