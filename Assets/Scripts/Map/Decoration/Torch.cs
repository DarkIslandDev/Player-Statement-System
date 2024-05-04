using System;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Transform checkPoint;
    [SerializeField] private ParticleSystem fire;
    public BoxCollider BoxCollider { get => boxCollider; private set => boxCollider = value; }
    public ParticleSystem ParticleSystem { get => fire; private set => fire = value; }

    public void CheckIfColliderNull()
    {
        Collider collider = checkPoint.GetComponent<Collider>();
        
        if (collider == null)
        {
            DestroyImmediate(gameObject);
        }
    }
}