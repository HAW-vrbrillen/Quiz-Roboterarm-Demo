using UnityEngine;

/// <summary>
/// Erzeugt eine unterteilte Plane und animiert die Vertices als wehende Fahne.
/// An ein leeres GO hängen — Mesh + Renderer werden automatisch erstellt.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FlagWave : MonoBehaviour
{
    [Header("Fahne")]
    public float width  = 1.2f;
    public float height = 0.8f;
    public int   segmentsX = 20;
    public int   segmentsY = 12;

    [Header("Wind")]
    public float waveAmplitude = 0.08f;
    public float waveFrequency = 3.0f;
    public float waveSpeed     = 2.5f;
    public float secondaryWave = 0.04f;

    private Vector3[] baseVertices;
    private Mesh mesh;

    void Awake()
    {
        mesh = BuildPlane();
        GetComponent<MeshFilter>().mesh = mesh;
        baseVertices = mesh.vertices.Clone() as Vector3[];
    }

    void Update()
    {
        var verts = mesh.vertices;
        float t = Time.time * waveSpeed;

        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 v = baseVertices[i];

            // Wie weit vom Mast entfernt (0 = Mast, 1 = Fahnenende)
            float xNorm = v.x / width;

            // Hauptwelle — stärker je weiter vom Mast
            float wave1 = Mathf.Sin(v.x * waveFrequency + t) * waveAmplitude * xNorm;
            // Sekundärwelle für natürlicheres Flattern
            float wave2 = Mathf.Sin(v.x * waveFrequency * 2.3f + v.y * 1.5f + t * 1.7f)
                          * secondaryWave * xNorm;

            verts[i] = v + new Vector3(0, 0, wave1 + wave2);
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    Mesh BuildPlane()
    {
        var m = new Mesh { name = "FlagMesh" };

        int vCountX = segmentsX + 1;
        int vCountY = segmentsY + 1;
        var vertices = new Vector3[vCountX * vCountY];
        var uvs      = new Vector2[vCountX * vCountY];

        for (int iy = 0; iy < vCountY; iy++)
        {
            for (int ix = 0; ix < vCountX; ix++)
            {
                int idx = iy * vCountX + ix;
                float xf = (float)ix / segmentsX;
                float yf = (float)iy / segmentsY;
                vertices[idx] = new Vector3(xf * width, yf * height, 0);
                uvs[idx]      = new Vector2(xf, yf);
            }
        }

        int[] tris = new int[segmentsX * segmentsY * 6];
        int ti = 0;
        for (int iy = 0; iy < segmentsY; iy++)
        {
            for (int ix = 0; ix < segmentsX; ix++)
            {
                int bl = iy * vCountX + ix;
                int br = bl + 1;
                int tl = bl + vCountX;
                int tr = tl + 1;

                tris[ti++] = bl; tris[ti++] = tl; tris[ti++] = br;
                tris[ti++] = br; tris[ti++] = tl; tris[ti++] = tr;
            }
        }

        m.vertices  = vertices;
        m.uv        = uvs;
        m.triangles = tris;
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }
}
