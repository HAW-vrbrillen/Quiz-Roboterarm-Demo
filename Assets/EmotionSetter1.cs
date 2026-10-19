using UnityEngine;

public class EmotionSetter1 : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMeshRenderer;
    [Header("Tasten 0 reset / 1 Lachen /2 Lächeln ")]
    [Header("3 Verschmitzt Lächeln 4 Erstaunt 5 Traurig")]
    public bool useKeyboardInput = false; // Setzen Sie dies im Inspector oder per Skript auf true, um die Tastatureingabe zu aktivieren.

    private void Start()
    {
        // Hier wird davon ausgegangen, dass der SkinnedMeshRenderer sich auf demselben GameObject befindet wie dieses Skript.
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    // Funktion, die aufgerufen wird, um die Emotion zu setzen.
    public void SetEmotion(int emotion)
    {
        ResetEmotions();

        // Emotionen werden als Zahlen von 1 bis 5 angenommen.
        switch (emotion)
        {
            case 1: // Lachen
                SetBlendShapeWeight("MouthSmile_L", 70);
                SetBlendShapeWeight("MouthSmile_R", 70);
                SetBlendShapeWeight("EyeSquint_L", 20);
                SetBlendShapeWeight("EyeSquint_R", 20);
                SetBlendShapeWeight("CheekSquint_L", 15);
                SetBlendShapeWeight("CheekSquint_R", 15);
                //SetBlendShapeWeight("JawOpen", 30);
                break;
            case 2: // Lächeln
                SetBlendShapeWeight("MouthSmile_L", 40);
                SetBlendShapeWeight("MouthSmile_R", 40);
                SetBlendShapeWeight("EyeSquint_L", 10);
                SetBlendShapeWeight("EyeSquint_R", 10);
                break;
            case 3: // Verschmitzt Lächeln
                SetBlendShapeWeight("MouthSmile_L", 30);
                SetBlendShapeWeight("MouthSmile_R", 20);
                SetBlendShapeWeight("BrowU_C", 10);
                SetBlendShapeWeight("EyeSquint_L", 10);
                SetBlendShapeWeight("EyeSquint_R", 5);
                break;
            case 4: // Erstaunt
                SetBlendShapeWeight("EyesWide", 70);
                SetBlendShapeWeight("BrowsU_C", 30);
                SetBlendShapeWeight("JawOpen", 30);
                //SetBlendShapeWeight("MouthOpen", 20); ist in jawopen enthalten
                break;
            case 5: // Traurig
                SetBlendShapeWeight("MouthFrown_L", 60);
                SetBlendShapeWeight("MouthFrown_R", 60);
                SetBlendShapeWeight("EyesDown", 30);
                SetBlendShapeWeight("BrowsD_L", 25);
                SetBlendShapeWeight("BrowsD_R", 25);
                break;
        }
    }

    // Diese Funktion setzt das Gewicht eines BlendShapes.
    private void SetBlendShapeWeight(string name, float weight)
    {
        int index = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(name);
        if (index != -1)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
        }
        else
        {
            Debug.LogWarning("Blendshape " + name + " not found");
        }
    }

    // Diese Funktion setzt alle BlendShapes zurück auf 0.
    private void ResetEmotions()
    {
        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, 0);
        }
    }

    private void Update()
    {
        if (useKeyboardInput)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                ResetEmotions();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                SetEmotion(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                SetEmotion(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                SetEmotion(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                SetEmotion(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                SetEmotion(5);
            }
        }
    }


}
