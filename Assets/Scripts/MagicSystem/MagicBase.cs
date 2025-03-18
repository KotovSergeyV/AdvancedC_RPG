using System;
using UnityEngine;

public class MagicBase : MonoBehaviour
{
    [field: SerializeField] public Action OnActivateMagic;
    [field: SerializeField] public float CastTime { get; private set; }
    [field: SerializeField] public int manaCost { get; private set; }
}