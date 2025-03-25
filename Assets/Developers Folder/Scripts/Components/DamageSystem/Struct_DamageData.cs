using System;
using UnityEngine;

[Serializable]
public struct Struct_DamageData
{
    [field: SerializeField] public int DamageAmount;
    [field: SerializeField] public Enum_DamageResponses Responce;
    [field: SerializeField] public Enum_DamageTypes DamageType;

    [field: SerializeField] public bool isInneviåtable;
    [field: SerializeField] public bool isBlockable;

}
