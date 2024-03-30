namespace HoangHH.DesignPattern
{
    /// <summary>
    /// Singleton pattern for non-MonoBehaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HNonMonoSingleton<T> where T : class, new()
    {
        private static T _instance;

        public static T Ins
        {
            get
            {
                if (_instance is not null) return _instance;
                _instance = new T();
                return _instance;
            }
        }
    }
}
