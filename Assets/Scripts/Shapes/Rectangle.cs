using UnityEngine;
using UnityEditor;

public class Rectangle : Shape, IShape
{
    public Vector3 rectangleSize;
    private Vector3[] frontSquare;
    private Vector3[] backSquare;

    public void Render(bool gizMode)
    {
        if (!this.material) return;
        
        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);
        
        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        GenerateVertices();
        ApplyTransformations();
        RenderEdges(gizMode);
        
        GL.End();
        GL.PopMatrix();
    }

    private void GenerateVertices()
    {
        frontSquare = new Vector3[4];
        backSquare = new Vector3[4];
        
        Vector3 halfSize = rectangleSize * 0.5f;
        
        Vector3[] baseVertices =
        {
            new Vector3(halfSize.x, halfSize.y, 0f),
            new Vector3(-halfSize.x, halfSize.y, 0f),
            new Vector3(-halfSize.x, -halfSize.y, 0f),
            new Vector3(halfSize.x, -halfSize.y, 0f),
        };
        
        for (int i = 0; i < baseVertices.Length; i++)
        {
            frontSquare[i] = baseVertices[i];
            backSquare[i] = baseVertices[i] - new Vector3(0, 0, rectangleSize.z);
        }
    }

    private void ApplyTransformations()
    {
        Quaternion rotationQuat = Quaternion.Euler(this.rotation);
        
        for (int i = 0; i < frontSquare.Length; i++)
        {
            frontSquare[i] = rotationQuat * frontSquare[i] + this.center;
            backSquare[i] = rotationQuat * backSquare[i] + this.center;
            
            frontSquare[i] = VectorCalculations.Project(frontSquare[i], this.innerFocalLength, this.center.z, this.size);
            backSquare[i] = VectorCalculations.Project(backSquare[i], this.innerFocalLength, this.center.z, this.size);
        }
    }
    
    private void RenderEdges(bool gizMode)
    {
        for (int i = 0; i < frontSquare.Length; i++)
        {
            int next = (i + 1) % frontSquare.Length;
            
            DrawLine(frontSquare[i], frontSquare[next], gizMode);
            DrawLine(backSquare[i], backSquare[next], gizMode);
            DrawLine(frontSquare[i], backSquare[i], gizMode);

            if (gizMode)
            {
                Handles.Label(frontSquare[i], $"FV{i}");
                Handles.Label(backSquare[i], $"BV{i}");
            }
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
