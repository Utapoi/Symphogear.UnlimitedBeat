namespace Symphogear.Common
{
    public class Singleton<T> : SymphogearBehaviour where T : SymphogearBehaviour
    {
        public static T Instance { get; private set; }

        public override bool Prepare()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this as T;
                DontDestroyOnLoad(Instance);

            }

            return base.Prepare();
        }
    }
}
