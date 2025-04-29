using System.Collections.Generic;
using System.Linq;
using UnityEditor.Overlays;
using UnityEngine;

public class EntityAgregator
{

    private static List<(GameObject, Enum_EntityType)> Entities = new List<(GameObject, Enum_EntityType)>();

    public static void AddEntity(GameObject entity, Enum_EntityType type) 
    {
        Entities.Add((entity, type));
    }
    public static void Clear() { Entities.Clear(); }

    public static List<EntitySaveData> GenerateSaveData()
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

