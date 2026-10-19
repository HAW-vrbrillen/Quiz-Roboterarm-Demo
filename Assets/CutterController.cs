using UnityEngine;
using TMPro;
using System.Collections;
public class CutterController : MonoBehaviour
{
    public GameObject MainCabin; // Das zu steuernde Objekt
    public GameObject Worktable;
    public GameObject CutterHeadNearFar;
    public GameObject product;
    public GameObject product2;
    public Light lamp; // Referenz zur Lampe
    public TMP_Text cabinValue;
    public TMP_Text WorkareaValue;
    public TMP_Text SchlittenValue;
    AudioSource as1;

    public GameObject NCControlPad;

    Vector3 newPositionCabine;
    Vector3 newPositionWorktable;

    bool cutterON;
    GameObject ncFaceCutter;
    public AudioClip aclip1;
    Animator animator1;

    public Animator ncFaceCutterAnimator;
    public Animator DoorsAnimator;
    //public Animator toolholderspindelTopAnimator;
    public TMP_Text cutterinfo1;

    public GameObject Task2Panelpart;
    public GameObject Task3Panelpart;

    Vector3 startPositionProduct;


    private void Start()
    {
        lamp.enabled = false;
        //product.transform.SetParent( Worktable.transform, true);
        //product2.transform.SetParent(Worktable.transform, true);
        ncFaceCutter = GameObject.Find("ncFaceCutter");
        as1 = GetComponent<AudioSource>();
        as1.clip = aclip1;
        animator1 = GetComponent<Animator>(); // animator für hauptsteuerung
        animator1.enabled = false;
        product.GetComponent<resetProdukt>().ActivateAllChildren();

        NCControlPad.SetActive(false);
        Task2Panelpart.SetActive(false);
        Task3Panelpart.SetActive(false);

        startPositionProduct = product.transform.position;



    }


    public void startCutterDemo() // durchläuft einen Demo durchlauf
    {
        cutterinfo1.text = "\n- Abene VHF-680 Demo läuft.";
        animator1.enabled= true;
        ncFaceCutter.GetComponent<Animator>().SetBool("spindelON", true); // Animator für Spindel
        as1.Play();
        as1.volume = 0.1f;
        lamp.intensity = 0.5f;
        animator1.SetTrigger("ncCutterOnMain");
        //animator1.enabled = false;
        Task2Panelpart.SetActive(true);
        Task3Panelpart.SetActive(true);
    }

    public void addCutterInfo1(string addinfo)
    {
        cutterinfo1.text += addinfo;
    }
    public void startCutterControl()
    {
        product.transform.position = startPositionProduct;
        animator1.enabled = false; // schalte haupt animator aus damit transform zugegriffen werden können
        DoorsAnimator.SetTrigger("closeDoors");
        ncFaceCutterAnimator.SetTrigger("startFaceCutter");
        StartCoroutine(PlayAudioInLoop());
        as1.volume = 0.1f;
        lamp.intensity = 0.5f;
        //animator1.enabled = false;
        //addCutterInfo1("\nSie haben den Cutter gestartet. \nBitte Abstand halten.");
        NCControlPad.SetActive(true);
        //product.GetComponent<Rigidbody>().isKinematic = true;
        //product2.GetComponent<Rigidbody>().isKinematic = true;
    }

    private IEnumerator PlayAudioInLoop()
    {
        while (true)
        {
            as1.Play();
            yield return new WaitForSeconds(20);
            as1.Stop();
        }
    }

    public void startNcFaceCutter()
    {
        
        ncFaceCutterAnimator.enabled = true;
        ncFaceCutterAnimator.SetBool("spindelON", true);
    }
    public void stopNcFaceCutter()
    {
        
        ncFaceCutterAnimator.SetBool("spindelON", false);
    }

    public void stopCutterControl()
    {
        Debug.Log("20240616 stopcuttercontrol aufgerufen");
        animator1.enabled = false;
        DoorsAnimator.SetTrigger("openDoors");
        ncFaceCutterAnimator.SetTrigger("stopFaceCutter");
        as1.Stop();
        as1.loop= false;
        lamp.intensity = 0f;
        //addCutterInfo1("\nSie haben den Cutter gestoppt. \nSie können herantreten.");
        NCControlPad.SetActive(false);
        //product.GetComponent<Rigidbody>().isKinematic = false;
        //product2.GetComponent<Rigidbody>().isKinematic = false;
    }


    public void MoveCabine(float y) // Bewegt die Cabine in Y-Richtung
    {
        
        
        newPositionCabine = MainCabin.transform.position;
        newPositionCabine.y += y;
        MainCabin.transform.position = newPositionCabine;
        cabinValue.text = newPositionCabine.y.ToString("F1");
    }

    public void MoveWorkarea(float x) // Bewegt den Schlitten in Z-Richtung
    {
        newPositionWorktable = Worktable.transform.position;
        newPositionWorktable.x += x;
        Worktable.transform.position = newPositionWorktable;
        //WorkareaValue.text = newPositionWorktable.x.ToString("F1");
    }
    public void MoveSchlitten(float z) // Bewegt den Schlitten in Z-Richtung
    {
        Vector3 newPosition = CutterHeadNearFar.transform.position;
        newPosition.z += z;
        CutterHeadNearFar.transform.position = newPosition;
        //SchlittenValue.text = newPosition.z.ToString("F1");
    }

    public void RotateSpindle(float x) // Rotiert die Spindel in X-Richtung
    {
        Vector3 newRotation = CutterHeadNearFar.transform.eulerAngles;
        newRotation.x += x;
        //CutterHeadNearFar.transform.eulerAngles = newRotation;
    }

    public void ToggleLamp() // Schaltet die Lampe ein/aus
    {
        lamp.enabled = !lamp.enabled;
    }
}
