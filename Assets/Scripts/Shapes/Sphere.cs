using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Sphere : Shape, IShape
{
    [Range(5f, 50f)] public float segments;
    [Range(5f, 50f)] public float rings;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        List<List<Vector3>> circles = new List<List<Vector3>>();

        // Generate horizontal rings (latitude lines)
        for (int i = 1; i < rings; i++)
        {
            float theta = Mathf.PI * (i / rings);
            float y = Mathf.Cos(theta) * this.size;
            float r = Mathf.Sin(theta) * this.size;

            circles.Add(CreateCircle(r, y, gizMode));
        }

        // Generate vertical segments (longitude lines)
        CreateLongitudeLines(circles, gizMode);

        GL.End();
        GL.PopMatrix();
    }

    private List<Vector3> CreateCircle(float radius, float yLevel, bool gizMode)
    {
        List<Vector3> vertices = new List<Vector3>();
        float angleStep = (Mathf.PI * 2) / segments;
        Vector3 firstPoint = Vector3.zero, prevPoint = Vector3.zero;

        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 currentPoint = new Vector3(Mathf.Cos(angle) * radius, yLevel, Mathf.Sin(angle) * radius);
            currentPoint = ApplyTransformations(currentPoint);

            if (i > 0) DrawLine(prevPoint, currentPoint, gizMode);
            if (i == 0) firstPoint = currentPoint;

            prevPoint = currentPoint;
            vertices.Add(currentPoint);
        }

        DrawLine(prevPoint, firstPoint, gizMode);
        return vertices;
    }

    private void CreateLongitudeLines(List<List<Vector3>> circles, bool gizMode)
    {
        int segmentsCount = circles[0].Count;
        for (int i = 0; i < segmentsCount; i++)
        {
            for (int j = 0; j < circles.Count - 1; j++)
            {
                DrawLine(circles[j][i], circles[j + 1][i], gizMode);
            }
        }
    }

    private Vector3 ApplyTransformations(Vector3 point)
    {
        return VectorCalculations.Project(
            VectorCalculations.Translate(point, this.center, this.rotation), 
            this.innerFocalLength, this.center.z, this.size
        );
    }

    public void DrawLine(Vector3 v1, Vector3 v2, bool gizMode)
    {
        if (gizMode) Gizmos.DrawLine(v1, v2);
        GL.Vertex3(v1.x, v1.y, v1.z);
        GL.Vertex3(v2.x, v2.y, v2.z);
    }

    public override void OnDrawGizmos()
    {
        if (!this.enabled) return;
        Render(true);
    }

    public override void OnPostRender()
    {
        Render(false);
    }
}
