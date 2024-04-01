using System;
using UnityEngine;

[Serializable]
public class Decoration
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public Vector3 positionOffset;
    [SerializeField] public Vector3 rotationOffset;
    [SerializeField] [Range(0, 360)] public float randomRotation = 0;
}