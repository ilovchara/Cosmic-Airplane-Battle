using System.IO;
using UnityEngine;

public static class SaveSystem
{
    // 保存数据到指定文件
    public static void Save(string saveFileName, object data)
    {
        // 将数据转为JSON格式
        var json = JsonUtility.ToJson(data);
        // 获取保存路径
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            // 将JSON数据写入文件
            File.WriteAllText(path, json);

            #if UNITY_EDITOR
            // 在编辑器中打印成功保存的日志
            Debug.Log($"Successfully saved data to {path}.");
            #endif
        }
        catch (System.Exception exception)
        {
            #if UNITY_EDITOR
            // 在编辑器中打印保存失败的错误日志
            Debug.LogError($"Failed to save data to {path}. \n{exception}");
            #endif
        }
    }

    // 从指定文件加载数据
    public static T Load<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            // 读取文件中的JSON数据
            var json = File.ReadAllText(path);
            // 将JSON数据转为指定类型的数据
            var data = JsonUtility.FromJson<T>(json);

            return data;
        }
        catch (System.Exception exception)
        {
            #if UNITY_EDITOR
            // 在编辑器中打印加载失败的错误日志
            Debug.LogError($"Failed to load data from {path}. \n{exception}");
            #endif

            return default; // 返回默认值
        }
    }

    // 删除指定的保存文件
    public static void DeleteSaveFile(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            // 删除文件
            File.Delete(path);
        }
        catch (System.Exception exception)
        {
            #if UNITY_EDITOR
            // 在编辑器中打印删除失败的错误日志
            Debug.LogError($"Failed to delete {path}. \n{exception}");
            #endif
        }
    }

    // 判断指定的保存文件是否存在
    public static bool SaveFileExists(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        // 判断文件是否存在
        return File.Exists(path);
    }
}
