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

        // draw multiple circles
        
        List<List<Vector3>> vertices = new List<List<Vector3>>();
        
        for (int i = 0; i < this.segments; i++)
        {
            float theta = Mathf.PI * (i / this.segments);
            float y = Mathf.Cos(theta) * this.size;
            float r = Mathf.Sin(theta) * this.size;

            vertices.Add(DrawCircle(r, y, this.segments, gizMode));
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            int j = i + 1;
            if (j == vertices.Count) j -= 1;

            List<Vector3> bVertex = vertices[i];
            List<Vector3> tNormal = vertices[j];
            
            for (int l = 0; l < bVertex.Count; l++)
            {
                Vector3 v1 = bVertex[l];
                Vector3 v2 = tNormal[l];
                
                DrawLine(v1, v2, gizMode);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    public List<Vector3> DrawCircle(float radi, float yRadi, float quality, bool gizMode)
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 lastPoint = Vector3.zero;
        Vector3 firstPoint = Vector3.zero;
        
        Vector3 startingVertex = Vector3.zero;
        Vector3 endingVertex = Vector3.zero;
        
        for (int i = 0; i < this.segments; i++)
        {
            int a = i;
            if (i >= this.segments) a = 0;
                
            float angle = (Mathf.PI * 2) * a / this.segments;
            float nextAngle = (Mathf.PI * 2) * (a+1) / this.segments;
            
            startingVertex = new Vector3(Mathf.Cos(angle) * radi, yRadi, Mathf.Sin(angle) * radi);
            endingVertex = new Vector3(Mathf.Cos(nextAngle) * radi, yRadi, Mathf.Sin(nextAngle) * radi);
            
            startingVertex = VectorCalculations.Project(VectorCalculations.Translate(startingVertex, this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
            endingVertex = VectorCalculations.Project(VectorCalculations.Translate(endingVertex, this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
            DrawLine(startingVertex, endingVertex, gizMode);
            
            vertices.Add(startingVertex);
            // Handles.Label(startingVertex, $"V:{i}.{startingVertex.y:0.0}");
        }

        return vertices;
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
