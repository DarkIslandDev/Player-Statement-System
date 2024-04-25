using System;
using UnityEngine;

[Serializable]
public class Decoration
{
    [SerializeField] public GameObject prefab;
    [Range(0, 100)] [SerializeField] public float weight;
}