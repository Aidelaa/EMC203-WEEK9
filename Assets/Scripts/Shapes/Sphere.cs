using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Sphere : Shape, IShape
{
    [Range(5f, 50f)] public float segments;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        // List of circles forming the sphere
        List<List<Vector3>> circles = new List<List<Vector3>>();

        for (int i = 0; i < this.segments; i++)
        {
            float theta = Mathf.PI * (i / this.segments); // Vertical angle
            float y = Mathf.Cos(theta) * this.size;
            float r = Mathf.Sin(theta) * this.size;

            circles.Add(DrawCircle(r, y, gizMode));
        }

        // Connect circles to create vertical lines
        for (int i = 0; i < circles.Count - 1; i++) 
        {
            List<Vector3> bottomCircle = circles[i];
            List<Vector3> topCircle = circles[i + 1];

            for (int j = 0; j < bottomCircle.Count; j++)
            {
                DrawLine(bottomCircle[j], topCircle[j], gizMode);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    public List<Vector3> DrawCircle(float radius, float yLevel, bool gizMode)
    {
        List<Vector3> vertices = new List<Vector3>();
        float angleStep = (Mathf.PI * 2) / this.segments;
        
        Vector3 firstPoint = Vector3.zero;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i < this.segments; i++)
        {
            float angle = i * angleStep;
            Vector3 currentPoint = new Vector3(Mathf.Cos(angle) * radius, yLevel, Mathf.Sin(angle) * radius);

            // Apply transformations
            currentPoint = VectorCalculations.Project(
                VectorCalculations.Translate(currentPoint, this.center, this.rotation), 
                this.innerFocalLength, this.center.z, this.size
            );

            if (i > 0) DrawLine(prevPoint, currentPoint, gizMode);
            
            if (i == 0) firstPoint = currentPoint; // Store first vertex
            prevPoint = currentPoint;
            vertices.Add(currentPoint);
        }

        // Close the circle by connecting the last point to the first
        DrawLine(prevPoint, firstPoint, gizMode);

        return vertices;
    }

    public void DrawLine(Vector3 v1, Vector3 v2, bool gizMode)
    {
        if (gizMode)
            Gizmos.DrawLine(v1, v2);
        
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
