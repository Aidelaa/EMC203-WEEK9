using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class VectorCalculations
{
    public static Vector2 RotateBy(float angle, float axis1, float axis2)
    {
        float firstAxis = axis1 * Mathf.Cos(angle) - axis2 * Mathf.Sin(angle);
        float secondAxis = axis2 * Mathf.Cos(angle) + axis1 * Mathf.Sin(angle);
        
        return new Vector2(firstAxis, secondAxis);
    }
    
    public static Vector3 Translate(Vector3 point, Vector3 center, Vector3 rotation)
    {
        float radius = rotation.z * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radius);
        float sin = Mathf.Sin(radius);
        
        float newX = cos * point.x - sin * point.y;
        float newY = sin * point.x + cos * point.y;
        
        return new Vector3(newX + center.x, newY + center.y, point.z + center.z);
    }

    public static Vector3 Project(Vector3 point, float focalLength, float centerZ, float size)
    {
        float z = point.z + focalLength;
        float scale = focalLength / ((centerZ - size * .5f) + focalLength);
        
        return new Vector3(point.x / scale, point.y / scale, z);
    }
}
