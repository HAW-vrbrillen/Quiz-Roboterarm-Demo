using System.Collections;
using UnityEngine;

public class URIKButton : MonoBehaviour
{
    [Header("Robot Joint Transforms (base to end effector)")]
    public Transform joint1_S_Axis1; // Base Rotation
    public Transform joint2_L_Axis2; // Shoulder
    public Transform joint3_U_Axis3; // Elbow
    public Transform joint4_R_Axis4; // Wrist 1
    public Transform joint5_B_Axis5; // Wrist 2
    public Transform joint6_T_Axis6; // Wrist 3 / End Effector

    [Header("Rotation Axes per Joint (y, x, or z)")]
    public string axis1 = "z";  // Base: vertical rotation
    public string axis2 = "z";  // Shoulder: pitch
    public string axis3 = "z";  // Elbow: pitch
    public string axis4 = "y";  // Wrist 1
    public string axis5 = "z";  // Wrist 2
    public string axis6 = "y";  // Wrist 3

    [Header("IK Target (e.g. HAW Logo)")]
    public GameObject ikTarget;

    [Tooltip("Spitze des Werkzeugs (z. B. Stab am Handgelenk). Wenn gesetzt, zielt die IK " +
             "mit dieser Spitze statt mit Achse 6 auf das Ziel.")]
    public Transform werkzeugSpitze;

    /// <summary>Punkt, mit dem der Roboter das Ziel anfährt: Werkzeugspitze oder Achse 6.</summary>
    public Transform Endeffektor => werkzeugSpitze != null ? werkzeugSpitze : joint6_T_Axis6;

    [Header("IK Settings")]
    public float threshold = 0.02f;
    public float learningRate = 5.0f;
    public int stepsPerFrame = 20;

    [Header("Robot Visibility")]
    [Tooltip("Objekte die beim IK-Start eingeblendet werden (z.B. UR5e-Root). Verhindert, dass der Roboter ausgeblendet bleibt wenn startIKOnStart=true.")]
    public GameObject[] roboterObjekte;

    [Header("Partikeleffekt bei Zielankunft")]
    [Tooltip("ParticleSystems die abgespielt werden sobald der Roboter das Ziel erreicht hat.")]
    public ParticleSystem[] particleBeimZiel;

    
[Header("Debug")]
    [Tooltip("IK beim Start automatisch ausführen (zum Testen)")]
    public bool startIKOnStart = false;

[Header("Button Proximity Settings")]
    public float pokeRadius = 0.04f;

    [Header("Challenge IK (schneller)")]
    public float challengeLearningRate = 8.0f;
    public int   challengeStepsPerFrame = 35;

    private bool isRunning = false;
    private bool isFollowMode = false;
    private bool buttonCooldown = false;

    public bool IsRunning    => isRunning;
    public bool IsFollowMode => isFollowMode;
    private IKManager ikManager;
    private Joint[] addedJoints;
    private Transform leftFingertip;
    private Transform rightFingertip;

    private Renderer buttonRenderer;
    private Color normalColor = new Color(0.8f, 0.1f, 0.1f);
    private Color activeColor = new Color(0.1f, 0.8f, 0.1f);

    // Gespeicherte Ausgangsrotationen für Reset
    private Quaternion[] initialRotations;

    void Start()
    {
        buttonRenderer = GetComponent<Renderer>();

        var allTransforms = FindObjectsOfType<Transform>();
        foreach (var t in allTransforms)
        {
            if (t.name == "HandIndexFingertip")
            {
                string path = GetPath(t);
                if (path.Contains("Left"))
                    leftFingertip = t;
                else if (path.Contains("Right"))
                    rightFingertip = t;
            }
        }

                if (leftFingertip != null) Debug.Log("URIKButton: Left fingertip found");
        if (rightFingertip != null) Debug.Log("URIKButton: Right fingertip found");

        // Ausgangsrotationen merken
        StoreInitialRotations();

        if (startIKOnStart)
            StartCoroutine(StartIKNextFrame());
    }

    void StoreInitialRotations()
    {
        Transform[] joints = { joint1_S_Axis1, joint2_L_Axis2, joint3_U_Axis3,
                               joint4_R_Axis4, joint5_B_Axis5, joint6_T_Axis6 };
        initialRotations = new Quaternion[joints.Length];
        for (int i = 0; i < joints.Length; i++)
            if (joints[i] != null) initialRotations[i] = joints[i].localRotation;
    }

    public IEnumerator ResetToInitial(float speed = 60f)
    {
        Transform[] joints = { joint1_S_Axis1, joint2_L_Axis2, joint3_U_Axis3,
                               joint4_R_Axis4, joint5_B_Axis5, joint6_T_Axis6 };
        if (initialRotations == null) StoreInitialRotations();
        if (initialRotations == null) yield break;

        bool done = false;
        while (!done)
        {
            done = true;
            for (int i = 0; i < joints.Length; i++)
            {
                if (joints[i] == null) continue;
                joints[i].localRotation = Quaternion.RotateTowards(
                    joints[i].localRotation, initialRotations[i], speed * Time.deltaTime);
                if (Quaternion.Angle(joints[i].localRotation, initialRotations[i]) > 0.5f)
                    done = false;
            }
            yield return null;
        }
    }

    /// <summary>IK mit Challenge-Geschwindigkeit starten (schneller aber noch sichtbar).</summary>
    public void StartChallengeIK()
    {
        if (isRunning) return;
        learningRate   = challengeLearningRate;
        stepsPerFrame  = challengeStepsPerFrame;
        StartIK();
    }

    private IEnumerator StartIKNextFrame()
    {
        // Einen Frame warten, damit LogoPuzzleSnap.Start() den Roboter nicht
        // danach wieder ausblendet
        yield return null;
        StartIK();
    }

    
string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    void Update()
    {
        if (buttonCooldown) return;

        // Toggle: poke again to stop
        bool poked = false;

        if (leftFingertip != null &&
            Vector3.Distance(transform.position, leftFingertip.position) < pokeRadius)
            poked = true;

        if (!poked && rightFingertip != null &&
            Vector3.Distance(transform.position, rightFingertip.position) < pokeRadius)
            poked = true;

        if (poked)
        {
            if (isRunning)
            {
                StopIK();
            }
            else
            {
                StartIK();
            }
            StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        buttonCooldown = true;
        yield return new WaitForSeconds(1.0f);
        buttonCooldown = false;
    }

    public void StartIK()
    {
        if (isRunning) return;

        // Roboter einblenden, falls er ausgeblendet ist (z.B. vor Puzzle-Completion)
        if (roboterObjekte != null)
        {
            foreach (var obj in roboterObjekte)
            {
                if (obj != null) obj.SetActive(true);
            }
        }

        if (ikTarget == null)
        {
            Debug.LogError("URIKButton: No IK target assigned!");
            return;
        }

        StartCoroutine(RunIK());
    }

    void TriggerPartikel()
    {
        if (particleBeimZiel == null) return;
        foreach (var ps in particleBeimZiel)
        {
            if (ps == null) continue;
            ps.gameObject.SetActive(true);
            ps.Play();
        }
    }

    public void StopIK()
    {
        if (!isRunning) return;

        if (ikManager != null)
            ikManager.enabled = false;

        isRunning = false;
        isFollowMode = false;

        if (buttonRenderer != null)
            buttonRenderer.material.color = normalColor;

        Debug.Log("URIKButton: IK stopped.");
    }

    private IEnumerator RunIK()
    {
        isRunning = true;

        if (buttonRenderer != null)
            buttonRenderer.material.color = activeColor;

        Debug.Log("URIKButton: Starting IK towards " + ikTarget.name);

        Transform[] jointTransforms = new Transform[]
        {
            joint1_S_Axis1, joint2_L_Axis2, joint3_U_Axis3,
            joint4_R_Axis4, joint5_B_Axis5, joint6_T_Axis6
        };
        string[] axisConfig = new string[] { axis1, axis2, axis3, axis4, axis5, axis6 };

        // Werkzeugspitze als letztes Kettenglied ohne eigene Drehachse ("") anhängen:
        // Die IK misst dann den Abstand der Spitze statt der Achse 6 zum Ziel.
        if (werkzeugSpitze != null)
        {
            jointTransforms = new Transform[]
            {
                joint1_S_Axis1, joint2_L_Axis2, joint3_U_Axis3,
                joint4_R_Axis4, joint5_B_Axis5, joint6_T_Axis6, werkzeugSpitze
            };
            axisConfig = new string[] { axis1, axis2, axis3, axis4, axis5, axis6, "" };
        }

        // Setup Joint components
        addedJoints = new Joint[jointTransforms.Length];
        for (int i = 0; i < jointTransforms.Length; i++)
        {
            if (jointTransforms[i] == null)
            {
                Debug.LogError("URIKButton: Joint transform " + i + " is not assigned!");
                isRunning = false;
                yield break;
            }

            Joint existing = jointTransforms[i].GetComponent<Joint>();
            if (existing != null)
                addedJoints[i] = existing;
            else
                addedJoints[i] = jointTransforms[i].gameObject.AddComponent<Joint>();

            addedJoints[i].m_jointtype = axisConfig[i];
        }

        // Chain joints
        for (int i = 0; i < addedJoints.Length - 1; i++)
            addedJoints[i].m_child = addedJoints[i + 1];
        addedJoints[addedJoints.Length - 1].m_child = null;

        // Setup IKManager
        ikManager = gameObject.GetComponent<IKManager>();
        if (ikManager == null)
            ikManager = gameObject.AddComponent<IKManager>();

        ikManager.m_root = addedJoints[0];
        ikManager.m_end = addedJoints[addedJoints.Length - 1];
        ikManager.m_target = ikTarget;
        ikManager.m_threshold = threshold;
        ikManager.m_rate = learningRate;
        ikManager.m_steps = stepsPerFrame;
        ikManager.enabled = true;

        // Zum Ziel fahren — auto-stop wenn nah genug oder Timeout
        Debug.Log("URIKButton: Fahre zum Ziel...");
        float elapsed = 0f;
        while (isRunning && elapsed < 15f)
        {
            float dist = Vector3.Distance(Endeffektor.position, ikTarget.transform.position);
            if (dist < threshold)
            {
                Debug.Log("URIKButton: Ziel erreicht — Partikel + Auto-Stop.");
                TriggerPartikel();
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Auto-Stop: IKManager deaktivieren, Roboter ist wieder frei beweglich
        StopIK();
    }
}
