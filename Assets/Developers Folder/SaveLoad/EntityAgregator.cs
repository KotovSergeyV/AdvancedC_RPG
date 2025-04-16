using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

public class EntityAgregator
{
    private static (GameObject, Enum_EntityType)[] Entities;

    public void AddEntity(GameObject entity, Enum_EntityType type) 
    {
        Entities.Append((entity, type));
    }
     
    public List<EntitySaveData> GenerateSaveData()
    {
        List<EntitySaveData> saveData = new List<EntitySaveData>();
        

        foreach (var entity in Entities)
        {
            var data = new EntitySaveData
            {
                Position = entity.Item1.transform.position,
                Rotation = entity.Item1.transform.rotation,
                EntityType = entity.Item2,
                CoreData = new CoreData()
            };

            data.CoreData.Initialise(entity.Item1.GetComponent<EntityCoreSystem>());
            saveData.Add(data);
        }

        return saveData;
    }

} 

