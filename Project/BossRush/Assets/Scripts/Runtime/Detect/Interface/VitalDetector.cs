using System.Collections.Generic;

using UnityEngine;

namespace TeamSuneat
{
    public class VitalDetector
    {
        private SID _ownerSID;
        private LayerMask _targetMask;
        private DetectShapes _detectShape;
        private Vector2 _detectBoxSize;
        private float _detectRadius;
        private float _detectArcAngle;

        public VitalComparer Comparer { get; set; }
        private DetectTypes _detectType;

        public void Setup(SID owerSID, LayerMask targetMask, DetectTypes detectType)
        {
            _ownerSID = owerSID;
            _targetMask = targetMask;
            _detectType = detectType;

            switch (_detectType)
            {
                case DetectTypes.Nearest: { Comparer = new NearestVitalComparer(); } break;
                case DetectTypes.Farthest: { Comparer = new FarthestVitalComparer(); } break;
                case DetectTypes.LowestLife: { Comparer = new LowestLifeVitalComparer(); } break;
                case DetectTypes.HighestLife: { Comparer = new HighestLifeVitalComparer(); } break;
            }
        }

        public void SetComparerPosition(Vector3 originPosition)
        {
            if (Comparer != null)
            {
                Comparer.Position = originPosition;
            }
        }

        public void SetDetectArea(DetectShapes detectShape, float radius, float arcAngle)
        {
            _detectShape = detectShape;
            _detectRadius = radius;
            _detectArcAngle = arcAngle;
        }

        public void SetDetectArea(DetectShapes detectShape, float radius)
        {
            _detectShape = detectShape;
            _detectRadius = radius;
        }

        public void SetDetectArea(DetectShapes detectShape, Vector2 size)
        {
            _detectShape = detectShape;
            _detectBoxSize = size;
        }

        public List<Vital> DoDetectVitalList(Vector3 detectPosition, bool isFacingRight)
        {
            List<Vital> targetVitals = FindInArea(detectPosition, isFacingRight);
            return DoDetectVitalList(targetVitals, detectPosition);
        }

        private List<Vital> DoDetectVitalList(List<Vital> candidates, Vector3 detectPosition)
        {
            if (candidates.IsValid())
            {
                if (candidates.Count > 1)
                {
                    if (_detectType == DetectTypes.Random)
                    {
                        Deck<Vital> deck = new Deck<Vital>();
                        deck.AddRange(candidates);
                        deck.Shuffle();
                        return deck.ToList();
                    }
                    else
                    {
                        SetComparerPosition(detectPosition);
                        candidates.Sort(Comparer);
                    }
                }

                return candidates;
            }

            return null;
        }

        private List<Vital> FindInArea(Vector3 detectPosition, bool isFacingRight)
        {
            List<Vital> targetVitals = null;

            switch (_detectShape)
            {
                case DetectShapes.Arc:
                    {
                        targetVitals = VitalManager.Instance.FindInArc(detectPosition, _detectRadius, _detectArcAngle, isFacingRight, _targetMask);
                    }
                    break;

                case DetectShapes.Circle:
                    {
                        targetVitals = VitalManager.Instance.FindInCircle(detectPosition, _detectRadius, _targetMask);
                    }
                    break;

                case DetectShapes.Box:
                    {
                        targetVitals = VitalManager.Instance.FindInBox(detectPosition, _detectBoxSize, _targetMask);
                    }
                    break;

                default:
                    Log.Error("탐지의 형태가 설정되지 않았습니다. 탐지할 수 없습니다.");
                    return null;
            }

            if (targetVitals.IsValid())
            {
                targetVitals.RemoveAll(x => x.SID == _ownerSID);
            }

            return targetVitals;
        }
    }
}