using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ARZoneDrawer : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    public Material fillMaterial; // Matériau à assigner dans l'inspecteur

    private List<Vector3> points = new List<Vector3>();
    private Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = fillMaterial;
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Vector3 hitPoint = hits[0].pose.position;

                    if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], hitPoint) > 0.05f)
                    {
                        points.Add(hitPoint);
                        UpdateMesh();
                    }
                }
            }
        }
    }
    void UpdateMesh()
    {
        if (points.Count < 3) return;

        if (mesh == null)
        {
            mesh = new Mesh();
            var mf = GetComponent<MeshFilter>();
            if (mf != null) mf.mesh = mesh;
            else Debug.LogWarning("❌ Pas de MeshFilter trouvé sur ARZoneDrawer.");
        }

        mesh.Clear();

        Vector3[] verts = points.ToArray();
        int[] indices = new Triangulator(points).Triangulate();

        mesh.vertices = verts;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }


    public void ValidateZone()
    {
        SaveGuardian();
        SceneManager.LoadScene("Menu"); // Remplace par le nom réel de ta scène menu
    }

    public void SaveGuardian()
    {
        GuardianData data = new GuardianData();
        foreach (var point in points)
        {
            data.points.Add(new Vector3Serializable(point));
        }

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "guardian.json");
        File.WriteAllText(path, json);
        Debug.Log("Guardian sauvegardé à : " + path);
    }

    public void LoadGuardian()
    {
        string path = Path.Combine(Application.persistentDataPath, "guardian.json");
        if (!File.Exists(path))
        {
            Debug.LogWarning("Aucun Guardian sauvegardé !");
            return;
        }

        string json = File.ReadAllText(path);
        GuardianData data = JsonUtility.FromJson<GuardianData>(json);

        points.Clear();
        foreach (var p in data.points)
            points.Add(p.ToVector3());

        UpdateMesh();
    }

    public List<Vector3> GetPoints()
    {
        return points;
    }


    public void SetPoints(List<Vector3> newPoints)
    {
        points = newPoints;
        UpdateMesh();
    }

}
