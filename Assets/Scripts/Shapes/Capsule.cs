using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Capsule : Shape, IShape
{
    [Range(5f, 50f)] public float segments;
    [Range(5, 100f)] public float distance;

    public void Render(bool gizMode)
    {
        if (!this.material) return;

        GL.PushMatrix();
        GL.Begin(GL.LINES);
        this.material.SetPass(0);

        GL.Color(Color.green);

        if (this.camera != null)
            GL.LoadProjectionMatrix(this.camera.projectionMatrix);

        // Lists to store semi-circle vertices
        List<List<Vector3>> semiCircleTop = new List<List<Vector3>>();
        List<List<Vector3>> semiCircleBottom = new List<List<Vector3>>();

        // Generate the top semi-circle
        for (int i = 0; i < this.segments; i++)
        {
            float theta = Mathf.PI * (i / this.segments); // Angle from top to middle
            float y = Mathf.Cos(theta) * this.size;
            float r = Mathf.Sin(theta) * this.size;
            semiCircleTop.Add(DrawSemiCircle(false, r, y, gizMode));
        }

        // Generate the bottom semi-circle
        for (int i = 0; i < this.segments; i++)
        {
            float theta = Mathf.PI * (i / this.segments);
            float y = Mathf.Cos(theta) * this.size;
            float r = Mathf.Sin(theta) * this.size;
            semiCircleBottom.Add(DrawSemiCircle(true, r, y, gizMode));
        }

        // Connect corresponding points between both semi-circles to form the capsule body
        for (int i = 0; i < semiCircleTop.Count; i++)
        {
            List<Vector3> topVertices = semiCircleTop[i];
            List<Vector3> bottomVertices = semiCircleBottom[i];

            for (int j = 0; j < topVertices.Count; j++)
            {
                DrawLine(topVertices[j], bottomVertices[j], gizMode);
            }
        }

        GL.End();
        GL.PopMatrix();
    }

    public List<Vector3> DrawSemiCircle(bool inverted, float radius, float yOffset, bool gizMode)
    {
        List<Vector3> vertices = new List<Vector3>();
        float halfDistance = this.distance * 0.5f;
        if (inverted) halfDistance = -halfDistance;

        float angleStep = Mathf.PI / (this.segments - 1); // Step size for semi-circle

        Vector3 firstPoint = Vector3.zero;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i < this.segments; i++)
        {
            float angle = i * angleStep;
            Vector3 currentPoint = new Vector3(Mathf.Cos(angle) * radius, yOffset, Mathf.Sin(angle) * radius);

            // Offset along Z-axis to form the capsule shape
            currentPoint += new Vector3(0f, 0f, halfDistance);

            currentPoint = VectorCalculations.Project(
                VectorCalculations.Translate(currentPoint, this.center, this.rotation),
                this.innerFocalLength, this.center.z, this.size
            );

            if (i > 0) DrawLine(prevPoint, currentPoint, gizMode);

            if (i == 0) firstPoint = currentPoint; 
            prevPoint = currentPoint;
            vertices.Add(currentPoint);
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
