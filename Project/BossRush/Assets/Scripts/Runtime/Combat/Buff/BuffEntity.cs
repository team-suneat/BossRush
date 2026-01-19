using Lean.Pool;
using TeamSuneat.Data;
using UnityEngine;

namespace TeamSuneat
{
    public class BuffEntity : MonoBehaviour, IPoolable
    {
        public BuffAssetData Data { get; private set; }
        public Character Owner { get; private set; }
        public Character Caster { get; private set; }

        public int Level { get; private set; } = 1;
        public int Stack { get; private set; } = 1;

        private float _elapsed;
        private float _intervalElapsed;

        public void Setup(BuffAssetData data, Character owner, Character caster, int level)
        {
            Data = data;
            Owner = owner;
            Caster = caster;
            Level = level;
            Stack = 1;
            _elapsed = 0f;
            _intervalElapsed = 0f;

            if (Owner != null)
            {
                Log.Info(LogTags.Buff, "{0}에게 버프 적용: {1} (레벨: {2}, 지속시간: {3}초)", Owner.Name.ToLogString(), Data.Name.ToLogString(), level, Data.Duration);
            }

            ApplyOnStart();
        }

        public void LogicUpdate()
        {
            if (Data == null || Owner == null)
            {
                return;
            }

            if (Data.Duration <= 0)
            {
                return;
            }

            float dt = Time.deltaTime;
            _elapsed += dt;

            if (Data.Interval > 0f && Data.Type == BuffType.DamageOverTime)
            {
                _intervalElapsed += dt;
                if (_intervalElapsed >= Data.Interval)
                {
                    _intervalElapsed -= Data.Interval;
                    ApplyInterval();
                }
            }

            if (Data.Duration > 0 && _elapsed >= Data.Duration)
            {
                OnExpire();
            }
        }

        public void AddStack(int add)
        {
            Stack = Mathf.Max(1, Stack + add);
            if (Data.Type == BuffType.StatBuff)
            {
                ApplyStat(+1);
            }

            if (Owner != null)
            {
                Log.Info(LogTags.Buff, "{0}의 버프 스택 증가: {1} (스택: {2})", Owner.Name.ToLogString(), Data.Name.ToLogString(), Stack);
            }
        }

        public void Despawn()
        {
            ResourcesManager.Despawn(gameObject);
        }

        public void OnSpawn()
        {
        }

        public void OnDespawn()
        {
            if (Data != null && Owner != null)
            {
                switch (Data.Type)
                {
                    case BuffType.StatBuff:
                        ApplyStat(-1);
                        break;

                    case BuffType.Stun:
                        ApplyStun(false);
                        break;
                }
            }

            Data = null;
            Owner = null;
            Caster = null;
            Level = 1;
            Stack = 1;
            _elapsed = 0f;
            _intervalElapsed = 0f;
        }

        private void ApplyOnStart()
        {
            switch (Data.Type)
            {
                case BuffType.StatBuff:
                    ApplyStat(+1);
                    break;

                case BuffType.DamageOverTime:
                    break;

                case BuffType.Stun:
                    ApplyStun(true);
                    break;
            }
        }

        private void ApplyInterval()
        {
            if (Data.Type != BuffType.DamageOverTime)
            {
                return;
            }

            ApplyDamageTick();
        }

        private void OnExpire()
        {
            switch (Data.Type)
            {
                case BuffType.StatBuff:
                    ApplyStat(-1);
                    break;

                case BuffType.Stun:
                    ApplyStun(false);
                    break;
            }

            if (Owner != null)
            {
                Log.Info(LogTags.Buff, "{0}의 버프 만료: {1}", Owner.Name.ToLogString(), Data.Name.ToLogString());
            }

            if (Owner.Buff != null)
            {
                Owner.Buff.Remove(Data.Name);
            }
        }

        private void ApplyStat(int sign)
        {
            if (Data.Stat == StatNames.None)
            {
                return;
            }

            float statValue = Data.Value * sign * Stack;
            BuffEntity source = this;
            string sourceName = Data.Name.ToString();
            string sourceType = "Buff";
            Owner.Stat.AddWithSourceInfo(Data.Stat, statValue, source, sourceName, sourceType);

            if (Owner != null)
            {
                string operation = sign > 0 ? "증가" : "감소";
                Log.Info(LogTags.Buff, "{0}의 능력치 {1}: {2} {3} (버프: {4}, 스택: {5})",
                    Owner.Name.ToLogString(), Data.Stat.ToLogString(), operation,
                    Mathf.Abs(statValue), Data.Name.ToLogString(), Stack);
            }
        }

        private void ApplyDamageTick()
        {
            if (Owner.MyVital == null)
            {
                return;
            }

            DamageResult dmg = new()
            {
                DamageValue = Data.Value * Stack,
                Attacker = Caster,
                TargetVital = Owner.MyVital,
            };
            if (!Owner.MyVital.CheckDamageImmunity(dmg))
            {
                _ = Owner.MyVital.TakeDamage(dmg);

                if (Owner != null)
                {
                    Log.Info(LogTags.Buff, "{0}에게 DoT 피해 적용: {1} (버프: {2}, 스택: {3})", Owner.Name.ToLogString(), dmg.DamageValue, Data.Name.ToLogString(), Stack);
                }
            }
        }

        private void ApplyStun(bool on)
        {
            if (Data.State != StateEffects.Stun)
            {
                return;
            }

            if (on)
            {
                Owner.ApplyStun(Data.Duration);
                Owner.MyVital.Life.SetTemporarilyInvulnerable(this);

                if (Owner != null)
                {
                    Log.Info(LogTags.Buff, "{0}에게 기절 적용: {1} (지속시간: {2}초)", Owner.Name.ToLogString(), Data.Name.ToLogString(), Data.Duration);
                }
            }
            else
            {
                Owner.ExitCrowdControlToState();
                Owner.MyVital.Life.ResetTemporarilyInvulnerable(this);

                if (Owner != null)
                {
                    Log.Info(LogTags.Buff, "{0}의 기절 해제: {1}", Owner.Name.ToLogString(), Data.Name.ToLogString());
                }
            }
        }
    }
}