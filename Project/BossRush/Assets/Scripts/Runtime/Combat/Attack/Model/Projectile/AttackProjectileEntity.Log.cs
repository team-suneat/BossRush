

namespace TeamSuneat
{
    public partial class AttackProjectileEntity : AttackEntity
    {
        protected override void LogProgress(string content)
        {
            if (Log.LevelProgress)
            {
                if (Owner != null)
                {
                    Log.Progress(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Progress(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }
        protected override void LogInfo(string content)
        {
            if (Log.LevelInfo)
            {
                if (Owner != null)
                {
                    Log.Info(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Info(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }

        protected override void LogWarning(string content)
        {
            if (Log.LevelWarning)
            {
                if (Owner != null)
                {
                    Log.Warning(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(Owner.Name.ToLogString(), Name.ToLogString(), content));
                }
                else if (OwnerProjectile != null)
                {
                    Log.Warning(LogTags.Attack_Projectile, TSStringGetter.ConcatStringWithComma(OwnerProjectile.Name.ToLogString(), Name.ToLogString(), content));
                }
            }
        }
    }
}