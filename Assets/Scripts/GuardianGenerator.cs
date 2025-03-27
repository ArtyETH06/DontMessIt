// ===============================
// 🧩 1. GuardianZoneGenerator.cs
// ===============================
using System.Collections.Generic;
using UnityEngine;

public class GuardianZoneGenerator : MonoBehaviour
{
    [Header("Références")]
    public ARZoneDrawer zoneDrawer;
    public Material zoneMaterial;
    public Transform zoneParent;
    public Gradient colorGradient;
    public List<ColoredZone> coloredZones = new List<ColoredZone>();

    [Header("Mode Test (Editeur seulement)")]
    public bool useTestGuardianInEditor = true;

    void Start()
    {
#if UNITY_EDITOR
        if (useTestGuardianInEditor && zoneDrawer.GetPoints().Count < 3)
        {
            Debug.Log("🧪 Mode test activé : Guardian fictif.");
            GenerateTestGuardian();
            GenerateColoredZones();
        }
#endif
    }

    public void GenerateColoredZones()
    {
        ClearOldZones();

        List<Vector3> points = zoneDrawer.GetPoints();
        if (points.Count < 3)
        {
            Debug.LogWarning("Pas assez de points pour générer les zones.");
            return;
        }

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

    void GenerateTestGuardian()
    {
        zoneDrawer.SetPoints(new List<Vector3>
        {
            new Vector3(-0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, -0.5f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(-0.5f, 0, 0.5f)
        });
    }
}