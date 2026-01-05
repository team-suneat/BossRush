using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    public class BaseChainLightning : XBehaviour
    {
        public GameObject LineRendererPrefab;
        public GameObject LightRendererPrefab;

        public int LightningCount = 1;
        public float Duration = 1.0f;
        public float IntervalTime = 0.05f;
        public float SegmentLength = 0.2f;

        public bool UseRandomOffset;

        [EnableIf("UseRandomOffset")]
        public Vector2 RandomOffset;

        public bool IsIncludeMyself = true;

        private List<BaseLightningBolt> _lightningBolts = new();
        private List<Transform> _targetPoints = new();
        private List<Vector3> _offsetPosition = new();
        private NearestTransformComparer _comparer = new NearestTransformComparer();

        private Coroutine _activeCoroutine;

        public void Clear()
        {
            _targetPoints.Clear();
            _offsetPosition.Clear();

            for (int i = 0; i < _lightningBolts.Count; i++)
            {
                _lightningBolts[i].Deactivate();
            }

            _lightningBolts.Clear();

            StopXCoroutine(ref _activeCoroutine);
        }

        public void AddTarget(Transform targetPoint)
        {
            _targetPoints.Add(targetPoint);
            _offsetPosition.Add(RandomEx.GetVector3Value(RandomOffset));

            SpawnLightinigBlot();
        }

        private void SpawnLightinigBlot()
        {
            BaseLightningBolt lightningBolt = new BaseLightningBolt(SegmentLength, _lightningBolts.Count);
            lightningBolt.SpawnLineRenderer(LineRendererPrefab, LightningCount, transform);
            lightningBolt.SpawnLightRenderer(LightRendererPrefab, transform);
            lightningBolt.Init();

            _lightningBolts.Add(lightningBolt);
        }

        public void Activate()
        {
            if (_activeCoroutine == null)
            {
                if (IsIncludeMyself)
                {
                    AddTarget(transform);
                }

                _activeCoroutine = StartXCoroutine(ProcessActive());
            }
        }

        private IEnumerator ProcessActive()
        {
            InfiniteLoopDetector.Reset();

            WaitForSeconds wait = new WaitForSeconds(IntervalTime);
            float elapsedTime = 0;
            while (elapsedTime < Duration)
            {
                InfiniteLoopDetector.Run();

                yield return wait;

                elapsedTime += IntervalTime;
                SortTargetPositions();
                ActivateOfChildren();
                DrawLightning();
            }

            Clear();
        }

        private void SortTargetPositions()
        {
            if (_targetPoints.Count > 1)
            {
                _comparer.Position = position;

                _targetPoints.Sort(_comparer);
            }
        }

        private void ActivateOfChildren()
        {
            for (int i = 0; i < _targetPoints.Count; i++)
            {
                if (_lightningBolts.Count > i)
                {
                    _lightningBolts[i].Activate();
                }
            }
        }

        public void DrawLightning()
        {
            Vector3 targetPosition;
            Vector3 targetPositionPrev;
            Vector3 targetPositionNext;

            for (int i = 0; i < _targetPoints.Count; i++)
            {
                if (_lightningBolts.Count > i)
                {
                    targetPosition = _targetPoints[i].position + _offsetPosition[i];

                    if (IsIncludeMyself)
                    {
                        if (i == 0)
                        {
                            _lightningBolts[i].DrawLightning(position, targetPosition);
                        }
                        else
                        {
                            targetPositionPrev = _targetPoints[i - 1].position + _offsetPosition[i - 1];
                            _lightningBolts[i].DrawLightning(targetPositionPrev, targetPosition);
                        }
                    }
                    else if (_lightningBolts.Count > i + 1)
                    {
                        targetPositionNext = _targetPoints[i + 1].position + _offsetPosition[i + 1];
                        _lightningBolts[i].DrawLightning(targetPosition, targetPositionNext);
                    }
                }
            }
        }
    }
}