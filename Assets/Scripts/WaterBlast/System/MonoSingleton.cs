using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
