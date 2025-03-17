using UnityEditor;
using UnityEngine;

public class Pyramid : Shape, IShape
{
    private Vector3[] translatedVertices;

    public void Render(bool gizMode)
    {
        if (!material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        material.SetPass(0);

        GL.Color(Color.green);
        if (camera != null)
            GL.LoadProjectionMatrix(camera.projectionMatrix);

        Vector3[] vertices = GenerateVertices();
        translatedVertices = new Vector3[vertices.Length];
        
        for (int i = 0; i < vertices.Length; i++)
        {
            translatedVertices[i] = VectorCalculations.Project(
                VectorCalculations.Translate(vertices[i], center, rotation),
                innerFocalLength, center.z, size
            );
        }
        
        DrawEdges(gizMode);
        LabelVertices(gizMode);

        GL.End();
        GL.PopMatrix();
    }

    private Vector3[] GenerateVertices()
    {
        return new Vector3[]
        {
            new Vector3(0f, size, 0f), // Top vertex
            new Vector3(-1f, -1f, 1f) * size, // Back Left
            new Vector3(1f, -1f, 1f) * size,  // Back Right
            new Vector3(1f, -1f, -1f) * size, // Front Right
            new Vector3(-1f, -1f, -1f) * size // Front Left
        };
    }

    private void DrawEdges(bool gizMode)
    {
        // Connect top vertex to base vertices
        for (int i = 1; i <= 4; i++)
        {
            DrawLine(translatedVertices[0], translatedVertices[i], gizMode);
        }
        
        // Connect base edges
        DrawLine(translatedVertices[1], translatedVertices[2], gizMode);
        DrawLine(translatedVertices[2], translatedVertices[3], gizMode);
        DrawLine(translatedVertices[3], translatedVertices[4], gizMode);
        DrawLine(translatedVertices[4], translatedVertices[1], gizMode);
    }

    private void LabelVertices(bool gizMode)
    {
        if (!gizMode) return;
        
        Handles.Label(translatedVertices[0], "Top");    
        Handles.Label(translatedVertices[1], "Back Left"); 
        Handles.Label(translatedVertices[2], "Back Right"); 
        Handles.Label(translatedVertices[3], "Front Right");
        Handles.Label(translatedVertices[4], "Front Left");
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
