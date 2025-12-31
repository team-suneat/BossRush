namespace TeamSuneat
{
    /// <summary>
    /// 씬 이동시 할당 해제되는 전역 클래스
    /// </summary>
    public class XStaticBehaviour<T> : XBehaviour where T : XBehaviour
    {
        public static T Instance;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            if (Instance != null)
            {
                if (Instance.Equals(this))
                {
                    Instance = default;
                }
            }
        }
    }
}