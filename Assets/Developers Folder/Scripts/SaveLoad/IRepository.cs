using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Overlays;
using UnityEngine;

public interface IRepository
{
    Task SaveDataAsync(List<EntitySaveData> data);
    Task<List<EntitySaveData>> LoadDataAsync();
}
