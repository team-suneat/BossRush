using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
    [System.Serializable]
    public class VisualEffectSpawnData
    {
        public GameObject[] Prefabs;
        public EffectSpawnTypes SpawnType;
        public EffectFacingTypes FacingType = EffectFacingTypes.Owner;

        [SuffixLabel("캐릭터와는 별개로 위치를 지정할 수 있습니다.")]
        [FoldoutGroup("#Toggle")] public bool IgnoreCharacterPosition;

        [FoldoutGroup("#Position")] public Vector2 SpawnArea;
        [FoldoutGroup("#Position")] public Vector3 SpawnOffset;
        [FoldoutGroup("#Position")] public Transform SpawnPoint;

        //─────────────────────────────────────────────────────────────────────────────────

        public bool UseParent { get; set; }

        public bool UseOnlyOne { get; set; }

        //─────────────────────────────────────────────────────────────────────────────────

        private Character _ownerCharacter;
        private List<VFXObject> _visualEffects = new List<VFXObject>();
        private int _currentIndex;

        private Transform _transform;
        private Vector3 _spawnPosition;
        private Vector3 _spawnOffset;

        //─────────────────────────────────────────────────────────────────────────────────

        public void LoadToggleValues()
        {
            if (Prefabs.IsValidArray())
            {
                VFXObject visualEffect = Prefabs[0].GetComponent<VFXObject>();

                UseParent = visualEffect.HaveParent;
                UseOnlyOne = visualEffect.OnlyOne;
            }
        }

        public void SetOwner(Character ownerCharacter)
        {
            _ownerCharacter = ownerCharacter;
        }

        public void SetParent(Transform followTransform)
        {
            _transform = followTransform;
        }

        public void SetPosition(Vector3 position)
        {
            if (UseParent) return;

            if (IgnoreCharacterPosition)
            {
                _spawnPosition = GetNotCharacterPosition(position);
            }
            else if (_ownerCharacter != null)
            {
                _spawnPosition = _ownerCharacter.position;
            }
        }

        private void SetOffset()
        {
            _spawnOffset = Vector3.zero;
            _spawnOffset = GetRandomAreaPosition(_spawnOffset);
            _spawnOffset = GetOffsetPosition(_spawnOffset);
        }

        private Vector3 GetNotCharacterPosition(Vector3 position)
        {
            if (SpawnPoint != null)
            {
                return SpawnPoint.position;
            }
            else
            {
                return position;
            }
        }

        private Vector3 GetRandomAreaPosition(Vector3 position)
        {
            if (!SpawnArea.IsZero())
            {
                position.x += RandomEx.Range(-SpawnArea.x, SpawnArea.x);
                position.y += RandomEx.Range(-SpawnArea.y, SpawnArea.y);
            }

            return position;
        }

        private Vector3 GetOffsetPosition(Vector3 position)
        {
            if (!SpawnOffset.IsZero())
            {
                if (GetFacingRight())
                {
                    position += SpawnOffset;
                }
                else
                {
                    position += SpawnOffset.FlipX();
                }
            }

            return position;
        }

        //─────────────────────────────────────────────────────────────────────────────────────────────────────────

        public bool TrySpawnVisualEffect()
        {
            if (Prefabs == null || Prefabs.Length == 0)
            {
                return false;
            }

            if (UseOnlyOne)
            {
                _visualEffects.RemoveNull();
                if (_visualEffects != null && _visualEffects.Count > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void SpawnVisualEffects(Transform parent)
        {
            for (int i = 0; i < Prefabs.Length; i++)
            {
                SpawnVisualEffect(parent, i);
            }
        }

        public void SpawnVisualEffect(Transform parent, int index)
        {
            GameObject prefab = GetPrefab(index);
            bool isFacingRight = GetFacingRight();

            SetParent(parent);
            SetPosition(parent.position);
            SetOffset();

            VFXObject visualEffect = null;
            if (UseParent)
            {
                var parentCharacter = parent.FindFirstParentComponent<Character>();

                if (parentCharacter != null)
                {
                    visualEffect = VFXManager.Spawn(prefab, parentCharacter, _transform);
                }
                else if (_ownerCharacter != null)
                {
                    visualEffect = VFXManager.Spawn(prefab, _ownerCharacter, _transform);
                }
                else
                {
                    visualEffect = VFXManager.Spawn(prefab, _transform, isFacingRight);
                }
            }
            else
            {
                visualEffect = VFXManager.Spawn(prefab, _spawnPosition, isFacingRight);
            }

            OnSpawnVisualEffect(visualEffect);
        }

        public void SpawnVisualEffect(Vector3 spawnPosition, int index)
        {
            GameObject prefab = GetPrefab(index);
            bool isFacingRight = GetFacingRight();

            SetParent(null);
            SetPosition(spawnPosition);
            SetOffset();

            VFXObject visualEffect = null;
            if (UseParent)
            {
                if (_ownerCharacter != null)
                {
                    visualEffect = VFXManager.Spawn(prefab, _ownerCharacter, _transform);
                    visualEffect.position += _spawnOffset;
                }
                else
                {
                    visualEffect = VFXManager.Spawn(prefab, _transform, isFacingRight);
                    visualEffect.position += _spawnOffset;
                }
            }
            else
            {
                visualEffect = VFXManager.Spawn(prefab, _spawnPosition, isFacingRight);
            }

            OnSpawnVisualEffect(visualEffect);
        }

        private void OnSpawnVisualEffect(VFXObject visualEffect)
        {
            if (visualEffect != null)
            {
                visualEffect.SetOwner(_ownerCharacter);
                visualEffect.position = visualEffect.position + _spawnOffset;

                SetupOnlyOne(visualEffect);

                if (visualEffect.Mover != null)
                {
                    visualEffect.Mover.SetOriginPosition(visualEffect.position);
                    visualEffect.Mover.SetTargetPosition(visualEffect.position - SpawnOffset);
                    visualEffect.Mover.StartMove();
                }

                visualEffect.RegisterDespawnEvent(OnDespawnVisualEffect);

                _visualEffects.Add(visualEffect);
            }
        }

        private void OnDespawnVisualEffect(VFXObject visualEffect)
        {
            if (_visualEffects.Contains(visualEffect))
            {
                _visualEffects.Remove(visualEffect);
            }
        }

        public void DespawnVisualEffect()
        {
            if (_visualEffects != null && _visualEffects.Count > 0)
            {
                _visualEffects.RemoveNull();

                for (int i = 0; i < _visualEffects.Count; i++)
                {
                    Log.Info(LogTags.Effect, "강제로 모든 VisualEffect를 소멸시킵니다. {0}", _visualEffects[i].GetHierarchyName());

                    _visualEffects[i].ForceDespawn();
                }
            }
        }

        //

        private GameObject GetPrefab(int index)
        {
            if (SpawnType == EffectSpawnTypes.Sequentially)
            {
                if (Prefabs.Length > _currentIndex)
                {
                    return Prefabs[_currentIndex++];
                }
                else
                {
                    _currentIndex = 0;

                    return Prefabs[_currentIndex++];
                }
            }
            else if (SpawnType == EffectSpawnTypes.Randomly)
            {
                int randomIndex = RandomEx.Range(0, Prefabs.Length);

                return Prefabs[randomIndex];
            }
            else if (SpawnType == EffectSpawnTypes.Designated)
            {
                if (index >= 0 && Prefabs.Length > index)
                {
                    return Prefabs[index];
                }
            }

            return null;
        }

        private bool GetFacingRight()
        {
            if (UseParent)
            {
                return true;
            }
            else if (FacingType == EffectFacingTypes.Random)
            {
                return RandomEx.GetBoolValue();
            }

            return true;
        }

        private void SetupOnlyOne(VFXObject visualEffect)
        {
            if (UseOnlyOne || visualEffect.OnlyOne)
            {
                visualEffect.RegisterDespawnEvent(OnDespawnOnlyOneVFX);
            }
        }

        private void OnDespawnOnlyOneVFX()
        {
            UnregisterAll();
        }

        private void UnregisterAll()
        {
            _visualEffects.Clear();
        }
    }
}