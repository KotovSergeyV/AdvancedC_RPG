using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class RepositoryJson : IRepository
{
    private readonly string _savePath = "Assets/Developers Folder/SaveLoad/SaveFiles/JSONSave";
    public async Task SaveDataAsync(List<EntitySaveData> data)
    {
        Debug.LogWarning("Data count to save: " + data);
        string tempPath = Path.GetTempFileName();
        try
        {
            var wrapper = new EntitySaveDataWrapper(data);
            string json = JsonUtility.ToJson(wrapper, true);

            // True async file writing
            await File.WriteAllTextAsync(tempPath, json);
            if (!File.Exists(_savePath))
            {
                File.Move(tempPath, _savePath);
                Debug.Log($"First save created at {_savePath}");
                return;
            }
            // Atomic file replacement
            File.Replace(tempPath, _savePath, null);

            Debug.Log($"Saved to {_savePath}");
            return;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Save failed: {ex.Message}");
            throw;
        }

    }


    public async Task<List<EntitySaveData>> LoadDataAsync()
    {
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning("No save file found at " + _savePath);
            return new List<EntitySaveData>();
        }

        try
        {
            // Async file read
            string json = await File.ReadAllTextAsync(_savePath);

            // Note: JsonUtility isn't async, but the file operation is
            var wrapper = JsonUtility.FromJson<EntitySaveDataWrapper>(json);

            if (wrapper?.data == null)
            {
                Debug.LogError("Save file corrupted or invalid format");
                return new List<EntitySaveData>();
            }

            return wrapper.data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Load failed: {ex.GetType().Name} - {ex.Message}");
            return new List<EntitySaveData>();
        }
    }

}


[Serializable]
public class EntitySaveDataWrapper
{
    public List<EntitySaveData> data;

    public EntitySaveDataWrapper(List<EntitySaveData> dataList)
    {
        data = dataList;
    }
}