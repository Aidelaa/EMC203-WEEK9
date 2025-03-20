using UnityEditor;
using UnityEngine;

public class Pyramid : Shape, IShape
{
    private Vector3[] translatedVertices;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        Vector3[] vertices = {
            new Vector3(0f, this.size, 0f), // Top vertex
            new Vector3(-1f, -1f, 1f) * this.size, // Back Left
            new Vector3(1f, -1f, 1f) * this.size, // Back Right
            new Vector3(1f, -1f, -1f) * this.size, // Front Right
            new Vector3(-1f, -1f, -1f) * this.size // Front Left
        };

        this.translatedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = RotateVertex(vertices[i]); // Apply rotation
            this.translatedVertices[i] = VectorCalculations.Project(VectorCalculations.Translate(vertices[i], this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
        }

        DrawBaseEdges(gizMode);
        ConnectBaseToTop(gizMode);
        
        GL.End();
        GL.PopMatrix();
    }

    private Vector3 RotateVertex(Vector3 vertex)
    {
        return this.center + Quaternion.Euler(this.rotation) * (vertex - this.center);
    }
    
    private void DrawBaseEdges(bool gizMode)
    {
        for (int i = 1; i <= 4; i++)
        {
            DrawLine(this.translatedVertices[i], this.translatedVertices[i % 4 + 1], gizMode);
        }
    }

    private void ConnectBaseToTop(bool gizMode)
    {
        for (int i = 1; i <= 4; i++)
        {
            DrawLine(this.translatedVertices[0], this.translatedVertices[i], gizMode);
        }
    }
    
    public void DrawLine(Vector3 v1, Vector3 v2, bool gizMode)
    {
        if (gizMode)
            Gizmos.DrawLine(v1, v2);
        
        GL.Vertex3(v1.x, v1.y, v1.z);
        GL.Vertex3(v2.x, v2.y, v2.z);
    }
    
    public override void OnDrawGizmos() {
        if (!this.enabled) return;
        Render(true);
    }

    public override void OnPostRender()
    {
        Render(false);
    }
}
