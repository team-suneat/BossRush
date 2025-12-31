using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

namespace TeamSuneat
{
    public partial class BuffEntity : XBehaviour, IPoolable
    {
        private Coroutine _spawnStateVFXCoroutine;
        private List<VFXObject> _visualEffectOfState = new();

        // Spawn & Despawn

        public void SpawnBuffStateVFX()
        {
            switch (AssetData.StateEffect)
            {
                case StateEffects.None:
                    return;

                case StateEffects.Chilled: // 오한
                    {
                        StartSpawnStateVFX();
                    }
                    break;

                case StateEffects.Stun: // 기절
                case StateEffects.Dazed: // 멍함
                case StateEffects.Paralysis: // 마비
                case StateEffects.Burning: // 연소
                case StateEffects.Freeze: // 빙결
                case StateEffects.Jolted: // 충격
                case StateEffects.ElectricShock: // 감전
                    {
                        SpawnStateVFX();
                    }
                    break;

                default:
                    LogProgress("버프의 상태({0}) 이펙트를 생성하지 않습니다.", AssetData.StateEffect.ToLogString());
                    break;
            }
        }

        private void DespawnStateVFX()
        {
            if (_visualEffectOfState.IsValid())
            {
                for (int i = 0; i < _visualEffectOfState.Count; i++)
                {
                    _visualEffectOfState[i].ForceDespawn();

                    LogProgress("버프의 이펙트를 삭제합니다. 이펙트: {0}", _visualEffectOfState[i].GetHierarchyName());
                }

                _visualEffectOfState.Clear();
            }

            StopSpawnStateVFX();
        }

        //
        private VFXObject SpawnStateVFX()
        {
            string vfxName = GetStateVFXName();
            if (string.IsNullOrEmpty(vfxName))
            {
                return null;
            }

            VFXObject visualEffect = VFXManager.Spawn(vfxName, Owner, transform);
            if (visualEffect != null)
            {
                SetupVisualEffect(visualEffect);
                _visualEffectOfState.Add(visualEffect);
                LogProgress("버프의 이펙트를 생성합니다. 이펙트: {0}", visualEffect.GetHierarchyName());
            }

            return visualEffect;
        }

        private void SetupVisualEffect(VFXObject visualEffect)
        {
            visualEffect.SetSortingName(Owner.CharacterRenderer.SortingLayerName);
            visualEffect.SetSortingOrder(Owner.CharacterRenderer.SortingLayerMaxOrder + 1);
            visualEffect.SetDirection(true);

            if (Owner.UseCustomBuffVFXPosition)
            {
                visualEffect.position = position;
            }
        }

        private string GetStateVFXName()
        {
            switch (AssetData.StateEffect)
            {
                case StateEffects.None:
                case StateEffects.Jolted: // 버프 독립체의 피드백에서 생성합니다.
                    {
                        return string.Empty;
                    }

                case StateEffects.ElectricShock:
                    {
                        if (Owner.IsPlayer)
                        {
                            return string.Format("fx_state_{0}_player", AssetData.StateEffect.ToLowerString());
                        }
                        else
                        {
                            return string.Format("fx_state_{0}", AssetData.StateEffect.ToLowerString());
                        }
                    }
                default:
                    {
                        return string.Format("fx_state_{0}", AssetData.StateEffect.ToLowerString());
                    }
            }
        }

        private void StartSpawnStateVFX()
        {
            if (_spawnStateVFXCoroutine == null)
            {
                _spawnStateVFXCoroutine = StartXCoroutine(ProcessSpawnStateVFX());
            }
        }

        private void StopSpawnStateVFX()
        {
            StopXCoroutine(ref _spawnStateVFXCoroutine);
        }

        private IEnumerator ProcessSpawnStateVFX()
        {
            InfiniteLoopDetector.Reset();

            while (true)
            {
                VFXObject spawnedVFX = SpawnStateVFX();
                if (spawnedVFX != null)
                {
                    spawnedVFX.position = GetStateVFXPosition();
                    yield return new WaitForSeconds(spawnedVFX.SpawnIntervalOfStateEffect);
                }
                else
                {
                    break;
                }

                InfiniteLoopDetector.Run();
            }
        }

        private Vector3 GetStateVFXPosition()
        {
            if (!Owner.BuffSpawnArea.IsZero())
            {
                Vector3 offset = RandomEx.GetVector3Value(Owner.BuffSpawnArea);

                return position + offset;
            }

            return position;
        }
    }
}