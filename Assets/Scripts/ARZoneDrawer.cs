using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
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
        Debug.Log("ARZoneDrawer: Awake");
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        if (fillMaterial != null)
        {
            GetComponent<MeshRenderer>().material = fillMaterial;
            Debug.Log("ARZoneDrawer: Fill material appliqué");
        }
        else
        {
            Debug.LogWarning("ARZoneDrawer: Fill material non défini");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        // Support souris dans l’éditeur
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("ARZoneDrawer: Clic souris détecté dans l'éditeur");
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            TryAddPoint(screenPosition);
        }
#else
        // Support tactile sur mobile
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            Debug.Log("ARZoneDrawer: Touche détectée sur mobile");
            Vector2 screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            TryAddPoint(screenPosition);
        }
#endif
    }

    void TryAddPoint(Vector2 screenPosition)
    {
        Debug.Log("ARZoneDrawer: Tentative d'ajout de point à la position écran : " + screenPosition);
        if (raycastManager.Raycast(screenPosition, hits, TrackableType.Planes))
        {
            Pose hitPose = hits[0].pose;
            points.Add(hitPose.position);
            Debug.Log("ARZoneDrawer: Point ajouté à la position : " + hitPose.position);
            UpdateMesh();
        }
        else
        {
            Debug.Log("ARZoneDrawer: Aucun hit détecté lors du raycast.");
        }
    }

    void UpdateMesh()
    {
        if (points.Count < 3)
        {
            Debug.Log("ARZoneDrawer: Pas assez de points pour générer une zone (points count : " + points.Count + ")");
            return;
        }

        if (mesh == null)
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            Debug.Log("ARZoneDrawer: Création d'un nouveau Mesh");
        }

        mesh.Clear();
        Debug.Log("ARZoneDrawer: Mesh effacé pour mise à jour");

        Vector3[] vertices = points.ToArray();
        int[] triangles = new Triangulator(points).Triangulate();
        Debug.Log("ARZoneDrawer: Triangles générés, nombre de triangles : " + (triangles.Length / 3));

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        Debug.Log("ARZoneDrawer: Mesh mis à jour");
    }

    public List<Vector3> GetPoints()
    {
        Debug.Log("ARZoneDrawer: Récupération des points, total : " + points.Count);
        return points;
    }

    public void SetPoints(List<Vector3> newPoints)
    {
        Debug.Log("ARZoneDrawer: Mise à jour des points avec " + newPoints.Count + " points");
        points = newPoints;
        UpdateMesh();
    }

    public void SaveGuardian(string filePath)
    {
        GuardianData data = new GuardianData { positions = points.ToArray() };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
        Debug.Log("ARZoneDrawer: Guardian sauvegardé à : " + filePath);
    }

    public void LoadGuardian(string filePath)
    {
        Debug.Log("ARZoneDrawer: Tentative de chargement du Guardian depuis : " + filePath);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("ARZoneDrawer: Aucun Guardian trouvé à : " + filePath);
            return;
        }

        string json = File.ReadAllText(filePath);
        GuardianData data = JsonUtility.FromJson<GuardianData>(json);
        points = new List<Vector3>(data.positions);
        UpdateMesh();
        Debug.Log("ARZoneDrawer: Guardian chargé, points mis à jour");
    }

    // Méthode à appeler lors du clic sur le bouton "Valider"
    public void ValidateAndLoadMenu()
    {
        Debug.Log("ARZoneDrawer: Validation et sauvegarde du Guardian");
        string filePath = Application.persistentDataPath + "/guardian.json";
        SaveGuardian(filePath);
        Debug.Log("ARZoneDrawer: Chargement de la scène Menu");
        SceneManager.LoadScene("Menu");
    }

    [System.Serializable]
    public class GuardianData
    {
        public Vector3[] positions;
    }
}
