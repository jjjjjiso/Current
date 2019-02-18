using UnityEngine;

public class MonoDontDestroySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static protected T instance = null;

    static public T G
    {
        get
        {
            return (T)instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this as T;

            DontDestroyOnLoad(this);

            OnAwake();
        }
    }

    protected virtual void OnAwake() { }
}
