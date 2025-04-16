using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RepositoryJson :IRepository
{
    private readonly string _savePath = "Assets/Developers Folder/SaveLoad/SaveFiles/JSONSave";
    public void SaveData(List<EntitySaveData> data)
    {
        // Create wrapper object
        var wrapper = new EntitySaveDataWrapper(data);

        // Serialize the wrapper instead of raw list
        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(_savePath, json);
        Debug.Log($"Saved to {_savePath}\nJSON: {json}");
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