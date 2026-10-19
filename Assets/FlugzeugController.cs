//20240219-2329-Mo nun Neigung

using System.Collections;
using UnityEngine;

public class FlugzeugController : MonoBehaviour
{
    public float geschwindigkeit = 5.0f;
    public float drehgeschwindigkeit = 2.0f; // Anpassen für sanftere Drehung
    public float steiggeschwindigkeit = 2.0f; // Geschwindigkeit für Steigen und Sinken

    private Rigidbody rb;
    public bool StartStopAirbus = false;

    private float zielDrehungY; // Zielrotation um die Y-Achse
    private float zielSteigung = 0f; // Ziel für Steigung/Sinken
    private float zielNeigung = 0f; // Ziel für die Neigung (Roll)

    public SoundClipScheduler soundClipScheduler;

    private Vector3 startposition; // Zum Speichern der Startposition
    private Quaternion startrotation; // Zum Speichern der Startrotation

    public GameObject cabin;
    public GameObject quiz;

    public RingFlightController RingFlightController;

    //public AudioClip aclip1;
    //public AudioClip aclip2;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        zielDrehungY = transform.eulerAngles.y;
        zielSteigung = transform.eulerAngles.x;
        startposition = transform.position;
        startrotation = transform.rotation;
    }

    public void scaleDown(GameObject go)
    {
        go.transform.localScale = Vector3.one/10000;
    }

    public void starteStoppeAirbus()
    {
        StartStopAirbus = !StartStopAirbus;
        if(StartStopAirbus){
            soundClipScheduler.PlayAirbusSound();
            scaleDown(quiz);
            scaleDown(cabin);
            
            
        }
        else {
            soundClipScheduler.audioSource1.Stop();
            soundClipScheduler.audioSource2.Stop();
        }

    }
    public void BackToBase()
    {
        // Stoppe jegliche Bewegung
        StartStopAirbus = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Setze Position und Rotation zurück auf die Startwerte
        transform.position = startposition;
        transform.rotation = startrotation;

        // Setze Zielrotationen zurück auf die Startwerte
        zielDrehungY = startrotation.eulerAngles.y;
        zielSteigung = startrotation.eulerAngles.x;
        zielNeigung = startrotation.eulerAngles.z;

        // Optional: Stoppe auch alle AudioSources, wenn nötig
        AudioSource[] allAudioSources = GetComponents<AudioSource>();
        foreach (var audioSource in allAudioSources)
        {
            audioSource.Stop();
        }
        RingFlightController.iniRingFlight();

    }




    void Update()
    {
        if (StartStopAirbus)
        {
            // Vorwärtsbewegung
            transform.position += transform.forward * geschwindigkeit * Time.deltaTime;

            // Glätte die Drehung um die Y-Achse für Kurven
            float winkelY = Mathf.LerpAngle(transform.eulerAngles.y, zielDrehungY, Time.deltaTime * drehgeschwindigkeit);
            
            // Glätte die Neigung für Steigen und Sinken
            float winkelX = Mathf.LerpAngle(transform.eulerAngles.x, zielSteigung, Time.deltaTime * steiggeschwindigkeit);
            
            // Glätte die Neigung (Roll) für Linkskurve/Rechtskurve
            float winkelZ = Mathf.LerpAngle(transform.eulerAngles.z, zielNeigung, Time.deltaTime * drehgeschwindigkeit);

            transform.rotation = Quaternion.Euler(winkelX, winkelY, winkelZ);
        }
    }

    public void FlyLeft()
    {
        zielDrehungY -= 45f; // Zielwinkel für eine kontinuierliche Linkskurve
        zielNeigung = 15f; // Leichte Neigung nach rechts für Linkskurve
    }

    public void FlyRight()
    {
        zielDrehungY += 45f; // Zielwinkel für eine kontinuierliche Rechtskurve
        zielNeigung = -15f; // Leichte Neigung nach links für Rechtskurve
    }

    public void FlyStraight()
    {
        zielDrehungY = transform.eulerAngles.y; // Zielwinkel für Geradeausflug
        zielNeigung = 0f; // Keine Neigung, zurück in Neutralposition
    }

    public void FlyUp()
    {
        zielSteigung -= 10f; // Neigung nach oben anpassen
    }

    public void FlyDown()
    {
        zielSteigung += 10f; // Neigung nach unten anpassen
    }
}



//20240219-2328-Mo gut aber keine neigung bei kurve
/* using System.Collections;
using UnityEngine;

public class FlugzeugController : MonoBehaviour
{
    public float geschwindigkeit = 0.2f;
    public float drehgeschwindigkeit = 0.5f; // Anpassen für sanftere Drehung
    public float steiggeschwindigkeit = 2.0f; // Geschwindigkeit für Steigen und Sinken

    private Rigidbody rb;
    public bool StartStopAirbus = false;

    private float zielDrehungY; // Zielrotation um die Y-Achse
    private float zielSteigung = 0f; // Ziel für Steigung/Sinken

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        zielDrehungY = transform.eulerAngles.y;
    }

    public void starteStoppeAirbus()
    {
        StartStopAirbus = !StartStopAirbus;
    }

    void Update()
    {
        if (StartStopAirbus)
        {
            // Vorwärtsbewegung
            transform.position += transform.forward * geschwindigkeit * Time.deltaTime;

            // Glätte die Drehung um die Y-Achse für Kurven
            float winkelY = Mathf.LerpAngle(transform.eulerAngles.y, zielDrehungY, Time.deltaTime * drehgeschwindigkeit);
            
            // Glätte die Neigung für Steigen und Sinken
            float winkelX = Mathf.LerpAngle(transform.eulerAngles.x, zielSteigung, Time.deltaTime * steiggeschwindigkeit);
            
            transform.rotation = Quaternion.Euler(winkelX, winkelY, 0);
        }
    }

    // Funktionen für Steuerung
    public void FlyLeft()
    {
        zielDrehungY -= 45f; // Zielwinkel für eine kontinuierliche Linkskurve
    }

    public void FlyRight()
    {
        zielDrehungY += 45f; // Zielwinkel für eine kontinuierliche Rechtskurve
    }

    public void FlyStraight()
    {
        zielDrehungY = transform.eulerAngles.y; // Zielwinkel für Geradeausflug
    }

    public void FlyUp()
    {
        zielSteigung -= 10f; // Neigung nach oben anpassen
    }

    public void FlyDown()
    {
        zielSteigung += 10f; // Neigung nach unten anpassen
    }
}
 */