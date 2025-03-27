using UnityEngine;

public class PolygonColliderZone : MonoBehaviour
{
    private Vector3[] trianglePoints;
    private Color myColor;

    public void Init(Vector3[] points, Color color)
    {
        trianglePoints = points;
        myColor = color;
    }

    public bool Contains(Vector3 position)
    {
        return IsPointInTriangle(position, trianglePoints[0], trianglePoints[1], trianglePoints[2]);
    }

    public Color GetColor()
    {
        return myColor;
    }

    // Test barycentrique
    private bool IsPointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float denom = (dot00 * dot11 - dot01 * dot01);
        if (denom == 0f) return false;

        float u = (dot11 * dot02 - dot01 * dot12) / denom;
        float v = (dot00 * dot12 - dot01 * dot02) / denom;

        return (u >= 0) && (v >= 0) && (u + v <= 1);
    }

    public Vector3 GetPoint(int index)
    {
        return trianglePoints[index];
    }

}
