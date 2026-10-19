using UnityEngine;
using System.Collections;

public class managerSdginfo : MonoBehaviour
{
    public GameObject targetObject; // Zielobjekt
    private Vector3 targetWaterWorldscale;
    public AudioSource audioSource; // AudioSource-Komponente
    public AudioClip sdgclip;
    public AudioSource asDolphin;
    
    public float duration = 5f; // Dauer der Bewegung und Rotation in Sekunden

    private Vector3 startPosition; // Startposition
    private Quaternion startRotation; // Startrotation
    private Vector3 startScale;

    public GameObject worldWaterObject;
    public swimpath swimpath1;

    public bool directChallengeFlag = false;

    void Start()
    {
        // Startposition und Startrotation speichern
        startPosition = transform.position;
        startRotation = transform.rotation;
        audioSource.clip = sdgclip;
        

        worldWaterObject.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
        //asDolphin.Stop();


        //StartCoroutine(MoveAndRotateObject());
    }

    public void startImmersionLargeWater()
    {
        StartCoroutine(MoveAndRotateObject());
    }
    public void startImmersionLargeWaterDirect()
    {
        directChallengeFlag = true;
        StartCoroutine(MoveAndRotateObject());
    }

    IEnumerator MoveAndRotateObject()
    {
        if (!directChallengeFlag)
        {
            // Sound abspielen
            audioSource.Play();

            // Warten, bis der Sound fertig ist
            yield return new WaitForSeconds(audioSource.clip.length); 
        }
        audioSource.Stop();
        // Startzeitpunkt der Bewegung und Rotation
        float startTime = Time.time;

        while (Time.time < startTime + duration)
        {
            // Berechnung des Fortschritts
            float t = (Time.time - startTime) / duration;

            // Bewegen des Objekts zur Position des Zielobjekts
             transform.position = Vector3.Lerp(startPosition, targetObject.transform.position, t);

            // Rotieren des Objekts zur Rotation des Zielobjekts
            transform.rotation = Quaternion.Lerp(startRotation, targetObject.transform.rotation, t);

            // Warten auf den nächsten Frame
            yield return null;
        }

        // Stellen Sie sicher, dass das Objekt am Ende genau an der Position und mit der Rotation des Zielobjekts ist
        transform.position = targetObject.transform.position;
        transform.rotation = targetObject.transform.rotation;


        // nun wasserwelt vergrößern
        
        startTime = Time.time;
        startScale = worldWaterObject.transform.localScale;
        targetWaterWorldscale = new Vector3(1f, 1f, 1f);
        while (Time.time < startTime + duration)
        {
            // Berechnung des Fortschritts
            float t = (Time.time - startTime) / duration;

            // Bewegen des Objekts zur Position des Zielobjekts
            worldWaterObject.transform.localScale = Vector3.Lerp(startScale, targetWaterWorldscale, t);


            // Warten auf den nächsten Frame
            yield return null;
        }

        // Stellen Sie sicher, dass das Objekt am Ende genau an der Position und mit der Rotation des Zielobjekts ist
        worldWaterObject.transform.localScale = targetWaterWorldscale;
        asDolphin.Play();
        swimpath1.startMovingDolphin();
    }
    /*
     * very welcome from Prof. Isenberg at HAW Hamburg, Germany in 2024
     * The well known United nations sustainability development goal 14 set the challenges for Life under water.
     * 
     * coastal algal bloom causing Dead zones
     * 
     * Ocean acidification caused by high levels of CO2 in the atmosphere is harmful to various marine organisms
     * that require calcium carbonate for their shells and skeletons, such as corals.
     * 
     * Plastic Polution, Over fishing 
     * 
     * I use unity to give a little immersion and let you help to clean the ocean.
     * You will get a nice thank you from out dolphin Flipper.
     * In Unity we use MixedReality to let the ocean waves role directly in your room.
     * The distance grab utility is a bit tricky but you will make it to place the tins
     * in the backpart of the fisher boat. By the way this introduction is made with lip sync blend shape animation
     * and fullbody human animation of a mixamo character. 
     * I wish you fun and serious thinking for our world. Yours Prof. Isenberg 2024
     */
}
