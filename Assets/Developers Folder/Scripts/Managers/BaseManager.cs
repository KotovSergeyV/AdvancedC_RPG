using UnityEngine;

public abstract class BaseManager<T> : MonoBehaviour where T : BaseManager<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = Object.FindFirstObjectByType<T>();
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
            _instance = (T)this;
        else
            Destroy(gameObject);
    }

    public abstract void Initialize();
}
