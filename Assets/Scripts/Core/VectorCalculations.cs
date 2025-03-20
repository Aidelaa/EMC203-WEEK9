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
        // Convert degrees to radians
        float xRad = rotation.x * Mathf.Deg2Rad;
        float yRad = rotation.y * Mathf.Deg2Rad;
        float zRad = rotation.z * Mathf.Deg2Rad;

        // Rotate around X-axis
        float cosX = Mathf.Cos(xRad);
        float sinX = Mathf.Sin(xRad);
        float newY = cosX * point.y - sinX * point.z;
        float newZ = sinX * point.y + cosX * point.z;

        // Rotate around Y-axis
        float cosY = Mathf.Cos(yRad);
        float sinY = Mathf.Sin(yRad);
        float newX = cosY * point.x + sinY * newZ;
        newZ = cosY * newZ - sinY * point.x;

        // Rotate around Z-axis
        float cosZ = Mathf.Cos(zRad);
        float sinZ = Mathf.Sin(zRad);
        float finalX = cosZ * newX - sinZ * newY;
        float finalY = sinZ * newX + cosZ * newY;

        return new Vector3(finalX + center.x, finalY + center.y, newZ + center.z);
    }

    public static Vector3 Project(Vector3 point, float focalLength, float centerZ, float size)
    {
        float z = point.z + focalLength;
        float scale = focalLength / ((centerZ - size * 0.5f) + focalLength);

        return new Vector3(point.x / scale, point.y / scale, z);
    }
}
