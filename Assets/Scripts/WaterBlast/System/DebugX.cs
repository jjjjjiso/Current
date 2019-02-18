using System;
using System.Diagnostics;

namespace WaterBlast.System
{
    public class DebugX
    {
        ///*
        [Conditional("UNITY_EDITOR")]
        static public void Log(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogWarning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogError(object msg, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(msg, context);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogError(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(context, format, args);
        }

        [Conditional("UNITY_EDITOR")]
        static public void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
        //*/


        /*
        [Conditional("UNITY_ANDROID")]
        static public void Log(string msg)
        {
            UnityEngine.Debug.Log(msg);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogFormat(format, args);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogWarning(string msg)
        {
            UnityEngine.Debug.LogWarning(msg);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogWarningFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(format, args);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogError(object msg, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(msg, context);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogError(string msg)
        {
            UnityEngine.Debug.LogError(msg);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogErrorFormat(string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(format, args);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(context, format, args);
        }

        [Conditional("UNITY_ANDROID")]
        static public void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
        */
    }
}