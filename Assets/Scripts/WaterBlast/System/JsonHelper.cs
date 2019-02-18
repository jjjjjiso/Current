using System;
using System.IO;

using UnityEngine;

public class JsonHelper
{
    static public T LoadJsonFile<T>(string path) where T : class
    {
        var file = new StreamReader(path);
        var fileContents = file.ReadToEnd();
        object obj = FromJson<T>(fileContents);
        file.Close();

        return obj as T;
    }

    static public void SaveJsonFile<T>(string path, T data) where T : class
    {
        var file = new StreamWriter(path);
        string toJson = ToJson(data, true);
        file.WriteLine(toJson);
        file.Close();
    }

    static public string ToJson(object obj)
    {
        return JsonUtility.ToJson(obj);
    }

    static public string ToJson(object obj, bool prettyPrint)
    {
        return JsonUtility.ToJson(obj, prettyPrint);
    }

    static public T FromJson<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    static public T[] ArrayFromJson<T>(string json)
    {
        json = "{ \"array\": " + json + "}";
        string newJson = json;
        JsonArrayWrapper<T> wrapper = FromJson<JsonArrayWrapper<T>>(newJson);
        return (T[])wrapper.array;
    }

    [Serializable]
    public class JsonArrayWrapper<T>
    {
        public T[] array;
    }
}