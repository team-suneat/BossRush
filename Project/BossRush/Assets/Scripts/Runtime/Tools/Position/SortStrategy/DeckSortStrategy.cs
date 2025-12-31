using System.Collections.Generic;

using UnityEngine;

namespace TeamSuneat
{
    public class DeckSortStrategy : IPositionGroupSortStrategy
    {
        private Deck<Transform> _childrenDeck;
        private Transform _lastDeckPosition;
        private bool _ignoreLastDeckPosition;

        public DeckSortStrategy(List<Transform> children, bool ignoreLastDeckPosition)
        {
            _childrenDeck = new Deck<Transform>();
            _ignoreLastDeckPosition = ignoreLastDeckPosition;
            SetupDeck(children);
        }

        public void Clear()
        {
            _childrenDeck.Clear();
        }

        public void SetupDeck(List<Transform> children)
        {
            _childrenDeck.Clear();
            _childrenDeck.AddRange(children);
            _childrenDeck.Shuffle();

            if (_ignoreLastDeckPosition && _lastDeckPosition != null && _childrenDeck.Contains(_lastDeckPosition))
            {
                _childrenDeck.Remove(_lastDeckPosition);
                _lastDeckPosition = null;
            }
        }

        public Vector3 GetPosition(Vector3 originPosition, List<Transform> children)
        {
            if (_childrenDeck.Count == 0)
            {
                SetupDeck(children);
            }

            Transform targetTransform = _childrenDeck.DrawTop();
            if (_ignoreLastDeckPosition && _childrenDeck.Count == 0)
            {
                _lastDeckPosition = targetTransform;
            }

            return targetTransform.position;
        }

        public List<Vector3> GetPositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            if (_childrenDeck.Count < positionCount)
            {
                SetupDeck(children);
            }

            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                if (_childrenDeck.Count == 0)
                {
                    SetupDeck(children);
                }

                Transform targetPoint = _childrenDeck.DrawTop();
                result.Add(targetPoint.position);
            }

            return result;
        }

        public List<Vector3> GetShufflePositions(Vector3 originPosition, List<Transform> children, int positionCount)
        {
            if (_childrenDeck.Count < positionCount)
            {
                SetupDeck(children);
            }

            List<Vector3> result = new List<Vector3>();
            for (int i = 0; i < positionCount; i++)
            {
                if (_childrenDeck.Count == 0)
                {
                    SetupDeck(children);
                }

                Transform targetPoint = _childrenDeck.DrawTop();
                result.Add(targetPoint.position);
            }

            return result;
        }
    }
}