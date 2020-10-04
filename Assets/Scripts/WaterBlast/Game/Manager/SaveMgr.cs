using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using UnityEngine;

namespace WaterBlast.Game.Manager
{
    public class SaveMgr
    {
        static private readonly string SAVEDATA_PATH_FORMAT = "{0}/{1}";

        static public T Load<T>(string fileName) where T : class
        {
            string dataPath = string.Empty;

#if UNITY_EDITOR
            dataPath = Application.dataPath;
#elif UNITY_ANDROID
            dataPath = Application.persistentDataPath;
#endif

            string path = string.Format(SAVEDATA_PATH_FORMAT, dataPath, fileName);

            T temp = null;
            if (File.Exists(path))
            {
                temp = JsonHelper.LoadJsonFile<T>(path);
            }

            return temp as T;
        }

        static public void Save<T>(T data, string fileName) where T : class
        {
            string dataPath = string.Empty;

#if UNITY_EDITOR
            dataPath = Application.dataPath;
#elif UNITY_ANDROID
            dataPath = Application.persistentDataPath;
#endif
            string path = string.Format(SAVEDATA_PATH_FORMAT, dataPath, fileName);
            JsonHelper.SaveJsonFile<T>(path, data);
        }
    }
}