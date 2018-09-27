using UnityEngine;

public class MonoDontDestroySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static protected T instance = null;

    static public T Get()
    {
        return (T)instance;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this as T;

        DontDestroyOnLoad(transform.root.gameObject);

        OnAwake();
    }

    protected virtual void OnAwake() { }
}
