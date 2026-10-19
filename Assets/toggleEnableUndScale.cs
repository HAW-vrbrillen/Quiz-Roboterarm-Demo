using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class toggleEnableUndScale : MonoBehaviour
{
    public GameObject go1;
    public AudioSource as1;
    public AudioSource as2;
    public bool toogleScaleNotEnable = true;

    // Start is called before the first frame update
    void Start()
    {
        if (toogleScaleNotEnable)
        {
            ToggleScale(go1);
        }
        else { toggleEnableObject(); }
    }
    public void toggleEnableObject()
    {
        if (go1.activeSelf == false)
        {
            go1.SetActive(true);
        }
        else
        { go1.SetActive(false); }
    }
    public void AnsagenAus()
    {

        as1.Stop();
        as2.Stop();

    }
    public void ToggleScale(GameObject obj)
    {
        float scaleThreshold = 1f / 500f; // Definiere eine Schwelle für die Verkleinerung
        Vector3 currentScale = obj.transform.localScale;
        Debug.Log("current scale= " + obj.transform.localScale.ToString());

        // Prüfe, ob die Skala des Objekts kleiner als die Schwelle ist
        if (currentScale.x < scaleThreshold && currentScale.y < scaleThreshold && currentScale.z < scaleThreshold)
        {
            // Wenn das Objekt verkleinert ist, vergrößere es
            obj.transform.localScale = currentScale * 1000f;
        }
        else
        {
            
            // Wenn das Objekt nicht verkleinert ist, verkleinere es
            obj.transform.localScale = currentScale / 1000f;
        }
    }

}
