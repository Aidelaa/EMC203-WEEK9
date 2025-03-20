using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cylinder : Shape, IShape
{
    [Range(5f, 100f)] public float segments;
    public float height;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        List<Vector3> topVertices = GenerateCircleVertices(this.center.y + (this.height * 0.5f));
        List<Vector3> bottomVertices = GenerateCircleVertices(this.center.y - (this.height * 0.5f));

        ApplyRotation(ref topVertices);
        ApplyRotation(ref bottomVertices);

        ProjectVertices(ref topVertices);
        ProjectVertices(ref bottomVertices);

        DrawEdges(topVertices, gizMode, "T");
        DrawEdges(bottomVertices, gizMode, "B");
        ConnectCircles(topVertices, bottomVertices, gizMode);

        GL.End();
        GL.PopMatrix();
    }

    private List<Vector3> GenerateCircleVertices(float yPosition)
    {
        List<Vector3> vertices = new List<Vector3>();
        float angleStep = (360 / this.segments) * Mathf.Deg2Rad;
        
        for (int i = 0; i < this.segments; i++)
        {
            Vector3 vertex = new Vector3(
                Mathf.Cos(angleStep * i) * this.size,
                yPosition,
                Mathf.Sin(angleStep * i) * this.size
            );
            vertices.Add(vertex);
        }
        return vertices;
    }

    private void ApplyRotation(ref List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = Quaternion.Euler(rotation) * (vertices[i] - center) + center;
        }
    }

    private void ProjectVertices(ref List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = VectorCalculations.Project(vertices[i], this.innerFocalLength, this.center.z, this.size);
        }
    }

    private void DrawEdges(List<Vector3> vertices, bool gizMode, string labelPrefix)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextIndex = (i + 1) % vertices.Count;
            DrawLine(vertices[i], vertices[nextIndex], gizMode);

            if (gizMode)
                Handles.Label(vertices[i], $"{labelPrefix}{i}");
        }
    }

    private void ConnectCircles(List<Vector3> topVertices, List<Vector3> bottomVertices, bool gizMode)
    {
        for (int i = 0; i < topVertices.Count; i++)
        {
            DrawLine(topVertices[i], bottomVertices[i], gizMode);
            
            if (gizMode)
                Handles.Label(Vector3.Lerp(topVertices[i], bottomVertices[i], 0.5f), $"TB{i}");
        }
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
