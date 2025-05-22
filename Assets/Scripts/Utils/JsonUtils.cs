using System.IO;
using UnityEngine;

public static class JsonUtils
{
    public static void WriteJsonFile(string absolutePath, string json)
    {
        File.WriteAllText(absolutePath, json); // Write all the data to the file.
    }

    public static string GetJsonFile(string absolutePath)
    {
        if (!File.Exists(absolutePath))
        {
            Debug.Log("File not found at " + absolutePath);
            return "";
        }
        return File.ReadAllText(absolutePath); // Write all the data to the file.
    }
    public static T LoadJsonData<T>(string absoluteName)
    {
        string json = GetJsonFile(absoluteName);
        return JsonUtility.FromJson<T>(json);
    }
    public static void WriteJsonData<T>(string absolutePath, T obj)
    {
        string json = JsonUtility.ToJson(obj, true);
        WriteJsonFile(absolutePath, json);
    }
}
