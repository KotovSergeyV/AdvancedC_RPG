using System.Collections.Generic;
using UnityEngine;

public class SpawnConfig
{
    public GameObject Prefab;
    public List<GameObject> VariantsWeapons;
    [Range(0, 1)] public float SpawnWeight = 1f;
    public string WeaponSocketName = "WeaponSocket";
}