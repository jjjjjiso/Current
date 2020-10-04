using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

using UnityEngine;

struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        Debug.Log("Converted to time");
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        Debug.Log("Converted to JDT");
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

public class JsonHelper
{
    static private readonly string key = "waterBlast_key";

    /// <summary> 로드 </summary>
    static public string Decrypt(string textToDecrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);
        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        return Encoding.UTF8.GetString(plainText);
    }

    /// <summary> 저장 </summary>
    static public string Encrypt(string textToEncrypt, string key)
    {
        RijndaelManaged rijndaelCipher = new RijndaelManaged();
        rijndaelCipher.Mode = CipherMode.CBC;
        rijndaelCipher.Padding = PaddingMode.PKCS7;
        rijndaelCipher.KeySize = 128;
        rijndaelCipher.BlockSize = 128;

        byte[] pwdBytes = Encoding.UTF8.GetBytes(key);
        byte[] keyBytes = new byte[16];
        int len = pwdBytes.Length;
        if (len > keyBytes.Length)
        {
            len = keyBytes.Length;
        }

        Array.Copy(pwdBytes, keyBytes, len);
        rijndaelCipher.Key = keyBytes;
        rijndaelCipher.IV = keyBytes;

        ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
        byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
        return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
    }

    static public T LoadJsonFile<T>(string path) where T : class
    {
        var file = new StreamReader(path);
        var fileContents = Decrypt(file.ReadToEnd(), key); //file.ReadToEnd();
        object obj = FromJson<T>(fileContents);
        file.Close();

        return obj as T;
    }

    static public void SaveJsonFile<T>(string path, T data) where T : class
    {
        var file = new StreamWriter(path);
        string toJson = Encrypt(ToJson(data, true), key); //ToJson(data, true);
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

    static public string ToJsonDateTime(object obj)
    {
        return JsonUtility.ToJson((JsonDateTime)obj);
    }

    static public string ToJsonDateTime(object obj, bool prettyPrint)
    {
        return JsonUtility.ToJson((JsonDateTime)obj, prettyPrint);
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