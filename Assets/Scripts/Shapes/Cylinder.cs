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

        // mb
        List<Vector3> topVertices = new List<Vector3>();
        List<Vector3> bottomVertices = new List<Vector3>();
        // create top circle?
        float angleStep = (360 / this.segments) * Mathf.Deg2Rad;

        Vector3 startingVertex = Vector3.zero;
        Vector3 endingVertex = Vector3.zero;
        for (int i = 0; i < this.segments; i++)
        {
            startingVertex.y = this.center.y + (this.height * .5f);
            endingVertex.y = this.center.y + (this.height * .5f);
            
            startingVertex.x = Mathf.Cos(angleStep * i);
            startingVertex.z = Mathf.Sin(angleStep * i);
            
            endingVertex.x = Mathf.Cos(angleStep * (i + 1));
            endingVertex.z = Mathf.Sin(angleStep * (i + 1));

            startingVertex *= this.size;
            endingVertex *= this.size;
            
            startingVertex = VectorCalculations.Translate(startingVertex, this.center, this.rotation);
            endingVertex = VectorCalculations.Translate(endingVertex, this.center, this.rotation);
            topVertices.Add(startingVertex);
        }
        
        // bottom circle
        startingVertex = Vector3.zero;
        endingVertex = Vector3.zero;
        for (int i = 0; i < this.segments; i++)
        {
            startingVertex.y = this.center.y - (this.height * .5f);
            endingVertex.y = this.center.y - (this.height * .5f);
            
            startingVertex.x = Mathf.Cos(angleStep * i);
            startingVertex.z = Mathf.Sin(angleStep * i);
            
            endingVertex.x = Mathf.Cos(angleStep * (i + 1));
            endingVertex.z = Mathf.Sin(angleStep * (i + 1));

            startingVertex *= this.size;
            endingVertex *= this.size;

            startingVertex = VectorCalculations.Translate(startingVertex, this.center, this.rotation);
            endingVertex = VectorCalculations.Translate(endingVertex, this.center, this.rotation);
            
            bottomVertices.Add(startingVertex);
        }
        
        // project depth
        for (int i = 0; i < topVertices.Count; i++)
        {
            Vector3 tVertex = topVertices[i];
            Vector3 bVertex = bottomVertices[i];
            
            topVertices[i] = VectorCalculations.Project(tVertex, this.innerFocalLength, this.center.z, this.size);
            bottomVertices[i] = VectorCalculations.Project(bVertex, this.innerFocalLength, this.center.z, this.size);
        }
        
        // rendering
        for (int i = 0; i < topVertices.Count; i++)
        {
            int j = i + 1;
            
            if (j >= topVertices.Count) j = 0;
            Vector3 v1 = topVertices[i];
            Vector3 v2 = topVertices[j];
            DrawLine(v1, v2, gizMode);
            
            if (gizMode)
                Handles.Label(v1, $"T{i.ToString()}");
        }

        for (int i = 0; i < bottomVertices.Count; i++)
        {
            int j = i + 1;
            
            if (j >= bottomVertices.Count) j = 0;
            Vector3 v1 = bottomVertices[i];
            Vector3 v2 = bottomVertices[j];
            DrawLine(v1, v2, gizMode);
            
            if (gizMode)
                Handles.Label(v1, $"B{i.ToString()}");
        }
        
        // connect the 2 together
        for (int i = 0; i < topVertices.Count; i++)
        {
            Vector3 v1 = topVertices[i];
            Vector3 v2 = bottomVertices[i];
            
            DrawLine(v1, v2, gizMode);
            
            if (gizMode)
                Handles.Label(Vector3.Lerp(v1, v2, .5f), $"TB{i.ToString()}");
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
