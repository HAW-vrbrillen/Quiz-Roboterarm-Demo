using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Editor-Script: Erzeugt 3 Logo-Puzzle-Cubes aus den echten Logo-JPGs:
///   1. "Hamburg"                  → Logo_Hamburg.jpg (Wappen)
///   2. "HAW Hamburg"              → Logo_HAW_Marke_CMYK_300dpi.jpg
///   3. "Rathaus-Passage-Hamburg"  → Logo_rathauspassagehamburg.jpg
///
/// Struktur pro Cube:
///   Parent: Cube (MeshFilter+MeshRenderer+Material) + BoxCollider(isTrigger) + Rigidbody(isKinematic)
///   Child:  [BuildingBlock] HandGrab (Grabbable + HandGrabInteractable)
///
/// Menü: GameObject > Logo Puzzle erstellen (3 Logos)
/// </summary>
public class CreateLogoPuzzlePieces : Editor
{
    static string texturePath = "Assets/LogoPuzzle/Textures";
    static string materialPath = "Assets/LogoPuzzle/Materials";

    // Puzzle-Definition: Name → Textur-Dateiname
    static readonly (string cubeName, string textureFile, string displayName)[] pieces = new[]
    {
        ("LP_Hamburg",                "Logo_Hamburg.jpg",                  "Hamburg"),
        ("LP_HAW_Hamburg",            "Logo_HAW_Marke_CMYK_300dpi.jpg",   "HAW Hamburg"),
        ("LP_Rathaus_Passage",        "Logo_rathauspassagehamburg.jpg",   "Rathaus-Passage-Hamburg"),
    };

    [MenuItem("GameObject/Logo Puzzle erstellen (3 Logos)", false, 10)]
    static void CreatePuzzle()
    {
        EnsureFolder(materialPath);

        // Prüfe ob alle Texturen vorhanden sind
        foreach (var piece in pieces)
        {
            string texPath = texturePath + "/" + piece.textureFile;
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (tex == null)
            {
                EditorUtility.DisplayDialog("Textur fehlt",
                    $"Textur nicht gefunden: {texPath}\nBitte Logo-JPGs in {texturePath}/ ablegen.",
                    "OK");
                return;
            }
        }

        // Parent-Container
        GameObject container = new GameObject("LogoPuzzle_Logos");
        Undo.RegisterCreatedObjectUndo(container, "Create Logo Puzzle (3 Logos)");

        float cubeSize = 0.08f;
        float spacing = 0.12f;

        for (int i = 0; i < pieces.Length; i++)
        {
            var piece = pieces[i];
            string texAssetPath = texturePath + "/" + piece.textureFile;
            Vector3 localPos = new Vector3(i * spacing, 0, 0);

            CreatePuzzleCube(
                piece.cubeName,
                container.transform,
                texAssetPath,
                localPos,
                cubeSize
            );
        }

        // Ziel-Container für Snap-Positionen
        GameObject targets = new GameObject("PuzzleTargets_Logos");
        Undo.RegisterCreatedObjectUndo(targets, "Create Puzzle Targets");
        targets.transform.SetParent(container.transform, false);
        targets.transform.localPosition = new Vector3(0, 0.3f, 0); // Targets oberhalb

        // Gemeinsames Glas-Material für alle Targets
        Material glassMat = CreateGlassMaterial();

        for (int i = 0; i < pieces.Length; i++)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "Target_" + pieces[i].cubeName;
            target.transform.SetParent(targets.transform, false);
            target.transform.localPosition = new Vector3(i * spacing, 0, 0);
            // Gleiche Größe wie die Puzzle-Cubes
            target.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

            // Glas-Material zuweisen
            target.GetComponent<MeshRenderer>().material = glassMat;

            // Collider entfernen — Target ist nur visuell
            Object.DestroyImmediate(target.GetComponent<BoxCollider>());
        }

        Selection.activeGameObject = container;

        Debug.Log($"Logo Puzzle erstellt! {pieces.Length} Cubes mit echten Logos:");
        for (int i = 0; i < pieces.Length; i++)
            Debug.Log($"  {pieces[i].cubeName} → {pieces[i].textureFile} (\"{pieces[i].displayName}\")");
        Debug.Log("WICHTIG: Positioniere die Cubes und Targets in deiner Szene.");
        Debug.Log("WICHTIG: Weise puzzlePieces und targetPositions im LogoPuzzleSnap zu.");
    }

    static void CreatePuzzleCube(string name, Transform parent, string textureAssetPath, Vector3 localPos, float size)
    {
        // --- PARENT: Cube mit Material ---
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPos;
        cube.transform.localRotation = Quaternion.identity;
        cube.transform.localScale = new Vector3(size, size, size);

        // Material erstellen mit Logo-Textur
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));

        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(textureAssetPath);
        if (tex != null)
        {
            mat.mainTexture = tex;
            // Smoothness runter für bessere Lesbarkeit in VR
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", 0.0f);
        }
        else
        {
            Debug.LogWarning("Textur nicht gefunden: " + textureAssetPath);
        }

        string matAssetPath = materialPath + "/" + name + ".mat";
        AssetDatabase.CreateAsset(mat, matAssetPath);

        MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
        renderer.material = mat;

        // BoxCollider: isTrigger = true
        BoxCollider boxCol = cube.GetComponent<BoxCollider>();
        boxCol.isTrigger = true;

        // Rigidbody: isKinematic, kein Gravity
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // --- CHILD: [BuildingBlock] HandGrab ---
        GameObject handGrabChild = new GameObject("[BuildingBlock] HandGrab");
        handGrabChild.transform.SetParent(cube.transform, false);
        handGrabChild.transform.localPosition = Vector3.zero;
        handGrabChild.transform.localRotation = Quaternion.identity;
        handGrabChild.transform.localScale = Vector3.one;

        // Grabbable (Oculus.Interaction)
        AddComponentByGUID(handGrabChild, "5a1bc992571301d4a9602ac95ef4c71a");
        // HandGrabInteractable (Oculus.Interaction.HandGrab)
        AddComponentByGUID(handGrabChild, "e9a7676b01585ce43908639a27765dfc");
        // PointableElement / GrabbableMultiAction
        AddComponentByGUID(handGrabChild, "43f86b14a27b52f4f9298c33015b5c26");

        Debug.Log($"Cube '{name}' erstellt. HandGrab-Child muss manuell Rigidbody-Referenz bekommen!");
    }

    static Material CreateGlassMaterial()
    {
        string matPath = materialPath + "/TargetGlass.mat";

        // Falls Material schon existiert, wiederverwenden
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(matPath);
        }

        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        if (mat.shader.name == "Hidden/InternalErrorShader")
            mat = new Material(Shader.Find("Standard"));

        // Transparent machen
        if (mat.HasProperty("_Surface"))
        {
            // URP Lit: Surface Type = Transparent
            mat.SetFloat("_Surface", 1f);
            mat.SetFloat("_Blend", 0f); // Alpha blend
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
        else
        {
            // Standard Shader Fallback
            mat.SetFloat("_Mode", 3f); // Transparent
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        // Hellblau-transparente Glasfarbe
        mat.SetColor("_BaseColor", new Color(0.5f, 0.8f, 1f, 0.2f));
        if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", new Color(0.5f, 0.8f, 1f, 0.2f));

        // Glatt/glänzend wie Glas
        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", 0.9f);
        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", 0.0f);

        AssetDatabase.CreateAsset(mat, matPath);
        return mat;
    }

    static void AddComponentByGUID(GameObject obj, string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning($"Script mit GUID {guid} nicht gefunden! Bitte manuell hinzufügen via BuildingBlock.");
            return;
        }

        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
        if (script == null)
        {
            Debug.LogWarning($"MonoScript konnte nicht geladen werden: {path}");
            return;
        }

        System.Type type = script.GetClass();
        if (type == null)
        {
            Debug.LogWarning($"Klasse nicht gefunden in Script: {path}");
            return;
        }

        obj.AddComponent(type);
    }

    static void EnsureFolder(string assetPath)
    {
        string[] parts = assetPath.Split('/');
        string current = parts[0]; // "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}
