using UnityEngine;

/// <summary>
/// Erzeugt eine nach innen gekruemmte Panorama-Wand aus einem Bild.
/// Platziere mehrere davon im Halbkreis fuer ein immersives Ambiente.
/// </summary>
[ExecuteInEditMode]
public class CurvedBackdrop : MonoBehaviour
{
    [Header("Panorama Einstellungen")]
    [Tooltip("Breite der Wand in Metern")]
    public float breite = 30f;

    [Tooltip("Hoehe der Wand in Metern")]
    public float hoehe = 15f;

    [Tooltip("Kruemmungsradius (groesser = flacher)")]
    public float radius = 20f;

    [Tooltip("Anzahl Segmente (mehr = glattere Kruemmung)")]
    [Range(8, 64)]
    public int segmente = 24;

    [Header("Material")]
    public Material bildMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private bool meshGenerated = false;

    void OnEnable()
    {
        if (!meshGenerated)
            GenerateMesh();
    }

    public void GenerateMesh()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Material nur setzen wenn bildMaterial zugewiesen ist,
        // sonst behalte das bereits am Renderer zugewiesene Material
        if (bildMaterial != null)
            meshRenderer.sharedMaterial = bildMaterial;

        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        Mesh mesh = new Mesh();
        mesh.name = "CurvedBackdropMesh";

        int vertCount = (segmente + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[segmente * 6];

        float totalAngle = breite / radius;
        float startAngle = -totalAngle / 2f;

        for (int i = 0; i <= segmente; i++)
        {
            float t = (float)i / segmente;
            float angle = startAngle + t * totalAngle;

            float x = Mathf.Sin(angle) * radius;
            float z = radius - Mathf.Cos(angle) * radius;

            vertices[i * 2] = new Vector3(x, 0, z);
            vertices[i * 2 + 1] = new Vector3(x, hoehe, z);

            uvs[i * 2] = new Vector2(t, 0);
            uvs[i * 2 + 1] = new Vector2(t, 1);
        }

        for (int i = 0; i < segmente; i++)
        {
            int vi = i * 2;
            int ti = i * 6;

            triangles[ti] = vi;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = vi + 2;

            triangles[ti + 3] = vi + 1;
            triangles[ti + 4] = vi + 3;
            triangles[ti + 5] = vi + 2;
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        meshGenerated = true;
    }

    void OnValidate()
    {
        if (gameObject.activeInHierarchy)
            GenerateMesh();
    }
}
