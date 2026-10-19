using UnityEngine;

public class resetProdukt : MonoBehaviour
{
    // Aufruf der Aktivierungsfunktion

    public FraskopfInteraction FraskopfInteraction;
    public void ActivateAllChildren()
    {
        FraskopfInteraction.correctCut = 0;
        FraskopfInteraction.falseCut = 0;
        FraskopfInteraction.countedObjects.Clear();
        FraskopfInteraction.UpdateInfoText();
        foreach (Transform child in transform.GetChild(0))
        {
            child.gameObject.GetComponent<Renderer>().enabled = true;

        }
    }
}
