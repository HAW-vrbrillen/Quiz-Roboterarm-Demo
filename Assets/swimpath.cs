using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public class swimpath : MonoBehaviour
{
    public Transform[] waypoints; // Wegpunkte, die im Inspector festgelegt werden
    public float speed = 5f; // Geschwindigkeit der Bewegung
    public float turnSpeed = 90f; // Drehgeschwindigkeit in Grad pro Sekunde
    private Animator animator;
    AudioSource audioSource;
    public AudioClip dolphinShouts;
    public AudioClip oceanSound;

    private int currentWaypointIndex = 0; // Der Index des aktuellen Wegpunktes
    public BoatController boatController;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = oceanSound;
        //audioSource.Play();

        
    }
    public void startMovingDolphin()
    {
        if (waypoints.Length > 0)
        {
            // Starte die Bewegung zum ersten Wegpunkt
            StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex]));
        }
    }

    IEnumerator MoveToWaypoint(Transform targetWaypoint)
    {
        animator.SetFloat(0, 0.4f);
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
        {
            // Ermittle die Richtung zum Zielwegpunkt
            Vector3 targetDirection = targetWaypoint.position - transform.position;
            targetDirection.y = 0; // Ignoriere Höhenunterschiede

            // Ermittle die Zielrotation basierend auf der Zielrichtung
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            // Drehe das Objekt schrittweise in Richtung der Zielrotation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

            // Bewege das Objekt zum Wegpunkt, wenn es annähernd ausgerichtet ist
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

            yield return null;
        }

        // Hier könntest du eine Animation auslösen oder andere Aktionen ausführen
        if (currentWaypointIndex == 5 && boatController.objectsInBoat>2)
        {
            
            animator.SetTrigger("Bite");
            audioSource.PlayOneShot(dolphinShouts);
            
            
            yield return new WaitForSeconds(2f);
        }
        

        // Fortfahren zum nächsten Wegpunkt
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        StartCoroutine(MoveToWaypoint(waypoints[currentWaypointIndex]));
    }
    
}
