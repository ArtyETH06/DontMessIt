using System.Collections.Generic;
using UnityEngine;

public class GuardianZoneGenerator : MonoBehaviour
{
    [Header("Références")]
    public ARZoneDrawer zoneDrawer; // Référence vers le composant qui a les points
    public Material zoneMaterial;   // Matériau de base à colorer

    [Header("Options")]
    public Transform zoneParent;    // Pour regrouper les zones générées
    public Gradient colorGradient;  // Pour générer de jolies couleurs
    public List<ColoredZone> coloredZones = new List<ColoredZone>();

    public void GenerateColoredZones()
    {
        ClearOldZones();

        List<Vector3> points = zoneDrawer.GetPoints(); // Méthode à ajouter
        if (points.Count < 3) return;

        Triangulator triangulator = new Triangulator(points);
        int[] triangles = triangulator.Triangulate();
        Vector3[] vertices = points.ToArray();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            GameObject triangleObj = new GameObject("Zone_" + (i / 3));
            triangleObj.transform.parent = zoneParent;

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] { p1, p2, p3 };
            mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter mf = triangleObj.AddComponent<MeshFilter>();
            mf.mesh = mesh;

            MeshRenderer mr = triangleObj.AddComponent<MeshRenderer>();
            Material newMat = new Material(zoneMaterial);

            float colorKey = (float)i / (float)triangles.Length;
            Color color = colorGradient.Evaluate(colorKey);
            newMat.color = color;
            mr.material = newMat;

            PolygonColliderZone pc = triangleObj.AddComponent<PolygonColliderZone>();
            pc.Init(new Vector3[] { p1, p2, p3 }, color);

            coloredZones.Add(new ColoredZone
            {
                gameObject = triangleObj,
                color = color,
                collider = pc
            });
        }
    }

    void ClearOldZones()
    {
        foreach (Transform child in zoneParent)
        {
            Destroy(child.gameObject);
        }
        coloredZones.Clear();
    }
}
