using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
            OnAwake();
        }
    }

    protected virtual void OnAwake() { }

    private void OnDestroy()
    {
        instance = null;
    }
}
