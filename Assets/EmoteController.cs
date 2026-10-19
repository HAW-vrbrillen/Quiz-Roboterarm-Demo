using UnityEngine;
using System.Collections;

public class EmoteController : MonoBehaviour
{
    public SkinnedMeshRenderer characterMeshRenderer;
    public float emotionIntensity = 100f; // Stellen Sie dies nach Bedarf ein
    public int delay = 3;

    public void SetEmotion(string emotionName)
    {
        int blendShapeIndex = characterMeshRenderer.sharedMesh.GetBlendShapeIndex(emotionName);
        if (blendShapeIndex != -1)
        {
            characterMeshRenderer.SetBlendShapeWeight(blendShapeIndex, emotionIntensity);
            StartCoroutine(ResetEmotionAfterDelay(blendShapeIndex, delay)); // 3 Sekunden Verzögerung
        }
    }

    IEnumerator ResetEmotionAfterDelay(int blendShapeIndex, int delay)
    {
        yield return new WaitForSeconds(delay);
        characterMeshRenderer.SetBlendShapeWeight(blendShapeIndex, 0);
    }

    public void ResetEmotions()
    {
        for (int i = 0; i < characterMeshRenderer.sharedMesh.blendShapeCount; i++)
        {
            characterMeshRenderer.SetBlendShapeWeight(i, 0);
        }
    }
}
