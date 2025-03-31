using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.IO;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ARZoneDrawer : MonoBehaviour
{
    [Header("AR")]
    public ARRaycastManager raycastManager;

    [Header("Matériaux")]
    public Material fillMaterial;

    private List<Vector3> points = new List<Vector3>();
    private Mesh mesh;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (fillMaterial != null)
        {
            GetComponent<MeshRenderer>().material = fillMaterial;
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        // Support souris dans l’éditeur
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            TryAddPoint(screenPosition);
        }
#else
        // Support tactile sur mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Vector2 screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            TryAddPoint(screenPosition);
        }
#endif
    }

    void TryAddPoint(Vector2 screenPosition)
    {
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;
            points.Add(hitPose.position);
            UpdateMesh();
        }
    }

    void UpdateMesh()
    {
        if (points.Count < 3) return;

        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        mesh.Clear();

        Vector3[] vertices = points.ToArray();
        int[] triangles = new Triangulator(points).Triangulate();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
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

    public void SaveGuardian(string filePath)
    {
        GuardianData data = new GuardianData { positions = points.ToArray() };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("Guardian sauvegardé à : " + filePath);
    }

    public void LoadGuardian(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Aucun Guardian trouvé à : " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        GuardianData data = JsonUtility.FromJson<GuardianData>(json);
        points = new List<Vector3>(data.positions);
        UpdateMesh();
    }

    [System.Serializable]
    public class GuardianData
    {
        public Vector3[] positions;
    }
}
