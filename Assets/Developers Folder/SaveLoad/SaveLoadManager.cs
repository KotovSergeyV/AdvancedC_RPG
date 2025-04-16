using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager
{
    IRepository _repo;

    public SaveLoadManager(IRepository repo)
    {
        _repo = repo;
    }

    public void SaveInRepo()
    {
        var data = EntityAgregator.GenerateSaveData();
        Debug.Log(data);
        _repo.SaveData(data);
    }

    public List<EntitySaveData> LoadFromRepo()
    {
        return _repo.LoadData();
    }
}
