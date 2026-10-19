using System.Collections;
using TMPro;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    // speed climb 
    // normal 0.04

    public Transform pathObjectsParent;
    private GameObject[] pathObjects;
    [SerializeField, Header("original speed")]
    private float speed = 0.05f;
    private int currentTargetIndex = 0;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip clip;
    public LayerMask groundLayer;

    public float descentSpeed = 5.0f; // Geschwindigkeit Absenken

    public float checkDistance = 0.005f;
    public float maxStepHeight = 0.15f;
    public float stepForwardDistance = 0.01f;
    public float rayDownDistance = 0.05f;
    public float minGroundClearance = 0.001f;
    public float maxGroundClearance = 0.02f;
    public float targetDistance = 0.02f;
    public Transform watertrans;
    public float waterLevelCorrection;

    private int maxStepAttempts = 10;

    public float rotationSpeed = 40.0f; // Geschwindigkeit der Drehung
    //public TMP_Text debugText;

    private string currentAnimation = ""; // Speichert die aktuell aktive Animation

    private float originalSpeed; // Zum Speichern der urspr�nglichen Geschwindigkeit
    public float reducedSpeedFactor = 0.25f; // Faktor f�r die reduzierte Geschwindigkeit

    private string currentGoalAnimation = ""; // Speichert die Animation des aktuellen Ziels


    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        InitializePathObjects();
        if (speed < 0.001f)
        {
            Debug.LogWarning("Achtung: Speed ist zu klein: " + speed);
            speed = 0.05f;

        }
        //speed = 0.05f;
        originalSpeed = speed; // Speichere die urspr�ngliche Geschwindigkeit  
        Debug.Log("speed:alles ist" + speed);
        //UpdateGoalAnimation(); // Initialisiere die erste Zielanimation ff

        //debugText = GameObject.Find("debugtext").GetComponent<TMP_Text>();
    }

    private void InitializePathObjects()
    {
        if (pathObjectsParent  != null)
        {
            pathObjects = new GameObject[pathObjectsParent.childCount];
            for (int i = 0; i < pathObjectsParent.childCount; i++)
            {
                pathObjects[i] = pathObjectsParent.GetChild(i).gameObject;
            }
        }
    }

    public void OnFootstep()
    {
        audioSource.PlayOneShot(clip);
    }

    void Update() // AdjustHeight MoveTowardsTargetOnXZPlan UpdateAnimation
    {
        if (pathObjects == null || pathObjects.Length == 0 || currentTargetIndex >= pathObjects.Length) return;

        AdjustHeight();
        MoveTowardsTargetOnXZPlane();
        //UpdateAnimation();
    }

    private void AdjustHeight()
    {
        if (currentAnimation == "Swimming")
        {
            float waterLevel = watertrans.position.y;
            // Setze Höhe auf Wasserhöhe mit Korrekturwert
            float targetHeight = waterLevel + waterLevelCorrection;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

            // Update Textelement mit Wasserhöhe in Millimeter
            //debugText.text = $"Höhe im Wasser: {(targetHeight * 1000):F0}mm";
        }
        else
        {
            RaycastHit hit;
            Vector3 rayStart = transform.position + Vector3.up * rayDownDistance;
            if (Physics.Raycast(rayStart, -Vector3.up, out hit, rayDownDistance + maxGroundClearance, groundLayer))
            {
                float distanceToGround = hit.distance - rayDownDistance;
                float targetHeight = transform.position.y - distanceToGround + minGroundClearance;

                transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);

                // Update Textelement mit Raycast-Information in Millimeter
                //debugText.text = $"Abstand nach unten: {(distanceToGround * 1000):F0}mm";
            }
            MeasureAndDisplayForwardDistance();
        }
    }


    private void MeasureAndDisplayForwardDistance()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + transform.forward * stepForwardDistance;
        if (Physics.Raycast(rayStart, transform.forward, out hit, maxStepHeight, groundLayer))
        {
            float distanceForward = hit.distance;
            //debugText.text += $"Abstand nach vorne: {(distanceForward * 1000):F0}mm";
        }
        else
        {
            //debugText.text += "Abstand nach vorne: Kein Hindernis";
        }
    }

    private void MoveTowardsTargetOnXZPlane()
    {
        // ung�nstig das dies in update ist also jeden zyklus l�uft
        // besser dann wenn neue animation gesetzt wird
        // AdjustSpeedBasedOnAnimation(); // Pass die Geschwindigkeit basierend auf der Animation an

        Vector3 targetPosition = pathObjects[currentTargetIndex].transform.position;
        targetPosition.y = transform.position.y; // Ignoriere die H�he (Y) f�r die Bewegung

        // Ausrichtung des Charakters zum Ziel
        Vector3 directionToTarget = targetPosition - transform.position;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // �berpr�fe die Distanz zum Ziel, bevor die Bewegung aktualisiert wird
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        if (distanceToTarget > checkDistance)
        {
            // Bewegung zum Ziel
            Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            transform.position = newPosition;
        }

        // �berpr�fe, ob das Ziel erreicht ist
        if (Vector3.Distance(transform.position, targetPosition) < targetDistance)
        {
            currentTargetIndex = (currentTargetIndex + 1) % pathObjects.Length;
            //UpdateGoalAnimation(); // Aktualisiere die Animation basierend auf dem neuen Ziel
            SetAnimationBasedOnTarget();
        }
    }

    private void SetAnimationBasedOnTarget()
    {
        string nextTargetName = pathObjects[currentTargetIndex].name;
        string newAnimation = DetermineAnimationBasedOnTargetName(nextTargetName);

        // Aktualisiere die Animation nur, wenn sie sich �ndert aber bitte 
        if (newAnimation != currentAnimation)
        {
            if (!string.IsNullOrEmpty(currentAnimation))
            {
                animator.SetBool(currentAnimation, false);
            }

            animator.SetBool(newAnimation, true);
            currentAnimation = newAnimation;

            // Anpassen der Geschwindigkeit - !!! es gibt extra Funktion !! alles
            if (newAnimation == "Swimming")
            {
                speed = originalSpeed * reducedSpeedFactor;
            }
            else if (newAnimation == "Climbing")
            {
                speed = originalSpeed * reducedSpeedFactor / 4;
            }
            else
            {
                speed = originalSpeed;
            }

        }
    }

    private string DetermineAnimationBasedOnTargetName(string targetName)
    {
        if (targetName.Contains("Swimming"))
        {
            return "Swimming";
        }
        else if (targetName.Contains("Climbing"))
        {
            return "Climbing";
        }
        else
        {
            return "Walking";
        }
    }
}
/* UpdateAnimation schon in SetAnimationBasedOnTarget
    private void UpdateAnimation()
    {
        bool isMoving = Vector3.Distance(transform.position, pathObjects[currentTargetIndex].transform.position) > targetDistance;

        string currentTargetName = pathObjects[currentTargetIndex].name;
        string nextAnimation = DetermineAnimationBasedOnTargetName(currentTargetName);

        // Pr�fe, ob die aktuelle Animation ge�ndert werden muss
        if (currentAnimation != nextAnimation)
        {
            if (!string.IsNullOrEmpty(currentAnimation))
            {
                animator.SetBool(currentAnimation, false);
            }

            animator.SetBool(nextAnimation, true);
            currentAnimation = nextAnimation;

            // Anpassen der Geschwindigkeit siehe extra Funktion
            speed = (nextAnimation == "Swimming") ? originalSpeed * reducedSpeedFactor : originalSpeed;

            speed = (nextAnimation == "Climbing") ? originalSpeed * reducedSpeedFactor / 4 : originalSpeed;
        }
        else if (isMoving && currentAnimation == "Walking")
        {
            // Setze die Walk-Animation nur, wenn sich der Charakter bewegt und die aktuelle Animation Walk ist
            animator.SetBool("Walking", true);
        }
    }
*/
/*UpdateGoalAnimation notused
    private void UpdateGoalAnimation()
    {
        string nextTargetName = pathObjects[currentTargetIndex].name;
        string nextAnimation = "";

        if (nextTargetName.Contains("Swimming"))
        {
            nextAnimation = "Swimming";
        }
        else if (nextTargetName.Contains("Climbing"))
        {
            nextAnimation = "Climbing";
        }
        else
        {
            nextAnimation = "Walking";
        }

        // �ndere die Animation nur, wenn sie sich vom aktuellen Ziel unterscheidet
        if (currentGoalAnimation != nextAnimation)
        {
            //animator.SetBool(currentGoalAnimation, false);
            animator.SetBool(nextAnimation, true);
            currentGoalAnimation = nextAnimation;

            // Setze die Geschwindigkeit entsprechend (Walking Speed wird automatisch als Original gesetzt!!)
            speed = (nextAnimation == "Swimming" ) ? originalSpeed * reducedSpeedFactor : originalSpeed;
            speed = (nextAnimation == "Climbing") ? originalSpeed * reducedSpeedFactor /4 : originalSpeed;
        }
    }

*/
/* UpdateAnimationAndSpeed notused
    private void UpdateAnimationAndSpeed()
    {
        // �berpr�fe den Namen des n�chsten Ziel-GameObjects
        string nextTargetName = pathObjects[currentTargetIndex].name;
        bool isSwimmingOrClimbing = nextTargetName.Contains("Swimming") || nextTargetName.Contains("Climbing");

        if (nextTargetName.Contains("Swimming"))
        {
            speed = originalSpeed;
            animator.SetBool("Swimming", true);
        }
        if (nextTargetName.Contains("Walking"))
        {
            speed = originalSpeed * reducedSpeedFactor;
            animator.SetBool("Walking", true);
        }
        if (nextTargetName.Contains("Climbing"))
        {
            speed = originalSpeed * reducedSpeedFactor/4;
            animator.SetBool("Climbing", true);
        }
    }
*/
/*AdjustSpeedBasedOnAnimation notused
    private void AdjustSpeedBasedOnAnimation()
    {
        // �berpr�fe den Status der spezifischen Animationen
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Swimming"))
        {
            speed = originalSpeed * reducedSpeedFactor; // Setze die Geschwindigkeit auf 25%
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Climbing"))
        {
            speed = originalSpeed * reducedSpeedFactor / 4; // Setze die Geschwindigkeit 25%/4 ca 6%
        }
        else
        {
            speed = originalSpeed; // Setze die Geschwindigkeit auf den urspr�nglichen Wert zur�ck
        }
    }
*/




