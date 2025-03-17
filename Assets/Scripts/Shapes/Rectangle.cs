using UnityEngine;
using UnityEditor;

public class Rectangle : Shape, IShape
{
    public Vector3 rectangleSize;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        Vector3[] vertices =
        {
            new Vector3(1f, 1f, 0f), // tr
            new Vector3(-1f, 1f, 0f),
            new Vector3(-1f, -1f, 0f),
            new Vector3(1f, -1f, 0f),
        };
        
        Vector3[] frontSquare = new Vector3[4];
        Vector3[] backSquare = new Vector3[4];
        
        vertices[0].x += this.rectangleSize.x * .5f;
        vertices[1].x -= this.rectangleSize.x * .5f;
        vertices[2].x -= this.rectangleSize.x * .5f;
        vertices[3].x += this.rectangleSize.x * .5f;
        
        vertices[0].y += this.rectangleSize.y * .5f;
        vertices[1].y += this.rectangleSize.y * .5f;
        vertices[2].y -= this.rectangleSize.y * .5f;
        vertices[3].y -= this.rectangleSize.y * .5f;
        
        for (int i = 0; i < vertices.Length; i++)
            frontSquare[i] = vertices[i];
        for (int i = 0; i < vertices.Length; i++)
            backSquare[i] = vertices[i];
        
        for (int i = 0; i < backSquare.Length; i++)
        {
            Vector3 vertex = backSquare[i];
            
            vertex.z -= this.rectangleSize.z;
            backSquare[i] = vertex;
        }
        
        // project and translate
        for (int i = 0; i < frontSquare.Length; i++)
            frontSquare[i] = VectorCalculations.Project(VectorCalculations.Translate(frontSquare[i], this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
        
        for (int i = 0; i < backSquare.Length; i++)
            backSquare[i] = VectorCalculations.Project(VectorCalculations.Translate(backSquare[i], this.center, this.rotation), this.innerFocalLength, this.center.z, this.size);
        
        // render
        for (int i = 0; i < frontSquare.Length; i++)
        {
            int j = i + 1;
            if (j >= frontSquare.Length) j = 0;
            
            Vector3 tV1 = frontSquare[i];
            Vector3 tV2 = frontSquare[j];
            
            Vector3 bV1 = backSquare[i];
            Vector3 bV2 = backSquare[j];

            if (gizMode)
            {
                Handles.Label(tV1, $"FV{i}");
                Handles.Label(bV1, $"BV{i}");
            }
            DrawLine(tV1, tV2, gizMode);
            DrawLine(bV1, bV2, gizMode);
        }
        
        // connect
        for (int i = 0; i < frontSquare.Length; i++)
        {
            Vector3 tVertex = frontSquare[i];
            Vector3 bVertex = backSquare[i];
            
            DrawLine(tVertex, bVertex, gizMode);
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
