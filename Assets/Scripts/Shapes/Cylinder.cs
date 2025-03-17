using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Cylinder : Shape, IShape
{
    [Range(5f, 100f)] public float segments;
    public float height;

    public void Render(bool gizMode)
    {
        if (!material) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        GL.Color(Color.green);
        if (camera != null)
            GL.LoadProjectionMatrix(camera.projectionMatrix);

        List<Vector3> topVertices = GenerateCircleVertices(center.y + (height * 0.5f));
        List<Vector3> bottomVertices = GenerateCircleVertices(center.y - (height * 0.5f));

        ProjectVertices(topVertices);
        ProjectVertices(bottomVertices);

        // Render edges
        DrawEdges(topVertices, gizMode, "T");
        DrawEdges(bottomVertices, gizMode, "B");
        ConnectCircles(topVertices, bottomVertices, gizMode);

        GL.End();
        GL.PopMatrix();
    }

    private List<Vector3> GenerateCircleVertices(float yPosition)
    {
        List<Vector3> vertices = new List<Vector3>();
        float angleStep = (360f / segments) * Mathf.Deg2Rad;
        
        for (int i = 0; i < segments; i++)
        {
            Vector3 vertex = new Vector3(
                Mathf.Cos(angleStep * i) * size,
                yPosition,
                Mathf.Sin(angleStep * i) * size
            );

            vertex = VectorCalculations.Translate(vertex, center, rotation);
            vertices.Add(vertex);
        }
        
        return vertices;
    }

    private void ProjectVertices(List<Vector3> vertices)
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = VectorCalculations.Project(vertices[i], innerFocalLength, center.z, size);
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
        if (!enabled) return;
        Render(true);
    }

    public override void OnPostRender()
    {
        Render(false);
    }
}