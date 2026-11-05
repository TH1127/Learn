using System;
using System.IO;
using UnityEngine;

public static class SaveHelper
{
    // 原子写入：先写临时文件，再替换原文件，避免写一半崩溃损坏存档
    public static void WriteAllTextAtomic(string path, string content)
    {
        string tmp = path + ".tmp";
        File.WriteAllText(tmp, content);
        if (File.Exists(path)) File.Delete(path);
        File.Move(tmp, path);
    }

    // 读取：文件不存在或解析失败时返回默认值，不抛异常
    public static T LoadJson<T>(string path) where T : new()
    {
        try
        {
            if (!File.Exists(path)) return new T();
            return JsonUtility.FromJson<T>(File.ReadAllText(path)) ?? new T();
        }
        catch (Exception)
        {
            return new T();
        }
    }
}
