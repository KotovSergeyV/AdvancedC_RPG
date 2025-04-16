using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public interface IRepository
{
    public void SaveData(List<EntitySaveData> data) { }

    public List<EntitySaveData> LoadData() { return new List<EntitySaveData>(); }
}
