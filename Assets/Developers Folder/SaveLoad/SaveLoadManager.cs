using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveLoadManager
{
    IRepository _repo;

    public SaveLoadManager(IRepository repo)
    {
        _repo = repo;
    }

    public async Task SaveInRepoAsync(List<EntitySaveData> data)
    {
        await _repo.SaveDataAsync(data); 
    }

    public async Task<List<EntitySaveData>> LoadFromRepo()
    {
        var saveData = await _repo.LoadDataAsync(); 
        return saveData;
    }
}
