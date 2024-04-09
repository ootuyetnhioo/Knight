using UnityEngine;

public abstract class BaseSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                if (FindObjectOfType<T>() != null)
                    _instance = FindObjectOfType<T>();
                else
                {
                    GameObject gObj = new GameObject();
                    gObj.AddComponent<T>();
                    gObj.name = "Singleton_" + typeof(T).ToString();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        CreateInstance();
    }

    protected void CreateInstance()
    {
        if (!_instance)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}