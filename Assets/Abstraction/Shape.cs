using System;
using UnityEngine;

public abstract class Shape : MonoBehaviour
{
    public float size;
    public Vector3 center;
    public Vector3 rotation;
    public Material material;
    public Camera camera;
    public float innerFocalLength;

    public abstract void OnDrawGizmos();
    public abstract void OnPostRender();
}
