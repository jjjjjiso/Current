using System;

using UnityEngine;

public static class MonoExtension
{
    public static void Invoke(this MonoBehaviour m, Action method, float time)
    {
        m.Invoke(method.Method.Name, time);
    }

    public static void InvokeRepeating(this MonoBehaviour m, Action method, float time, float repeatRate)
    {
        m.InvokeRepeating(method.Method.Name, time, repeatRate);
    }

    public static void CancelInvoke(this MonoBehaviour m, Action method)
    {
        m.CancelInvoke(method.Method.Name);
    }

    public static bool Invoke(this MonoBehaviour m, Action method)
    {
        return m.IsInvoking(method.Method.Name);
    }
}
