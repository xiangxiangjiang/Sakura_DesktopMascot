using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();
    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return null;
            }
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return _instance;
                    }

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        singleton.hideFlags = HideFlags.DontSave;
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton)_" + typeof(T).ToString();

                        // DontDestroyOnLoad(singleton);
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;
    protected virtual void OnDestroy() { applicationIsQuitting = true; }
}