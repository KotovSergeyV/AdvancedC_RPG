using System;
using UnityEngine;


[Serializable]
public class EntitySaveData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Enum_EntityType EntityType;
    public CoreData CoreData;
}


public enum Enum_EntityType
{
    Player, 
    Melee, 
    Range,
    Civilian
}