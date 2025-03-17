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
            new Vector3(0f, this.size, 0f),

            // back faces
            new Vector3(-1f, -1f, 1f) * this.size,
            new Vector3(1f, -1f, 1f) * this.size,

            // front faces
            new Vector3(1f, -1f, -1f) * this.size,
            new Vector3(-1f, -1f, -1f) * this.size,
        };

        this.translatedVertices = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            this.translatedVertices[i] = VectorCalculations.Project(VectorCalculations.Translate(vertices[i], this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
        }
        // ts pmo
        DrawLine(this.translatedVertices[0], this.translatedVertices[1], gizMode); 
        DrawLine(this.translatedVertices[0], this.translatedVertices[2], gizMode);
        DrawLine(this.translatedVertices[0], this.translatedVertices[3], gizMode);
        DrawLine(this.translatedVertices[0], this.translatedVertices[4], gizMode);
        DrawLine(this.translatedVertices[1], this.translatedVertices[2], gizMode);
        DrawLine(this.translatedVertices[2], this.translatedVertices[3], gizMode);
        DrawLine(this.translatedVertices[3], this.translatedVertices[4], gizMode);
        DrawLine(this.translatedVertices[4], this.translatedVertices[1], gizMode);

        if (gizMode)
        {    
            Handles.Label(this.translatedVertices[0], "Top");    
            Handles.Label(this.translatedVertices[1], "Back Left"); 
            Handles.Label(this.translatedVertices[2], "Back Right"); 
            Handles.Label(this.translatedVertices[3], "Front Left");
            Handles.Label(this.translatedVertices[4], "Front Right");   
        }
        
        GL.End();
        GL.PopMatrix();
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
