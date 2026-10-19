using TMPro;
using UnityEngine;

public class AnimationControllerBrianHand : MonoBehaviour
{
    [Header("Anim-Speed = 0,ColliderEnter Speed = 1")]
    [Header("Achtung AudioSource muss beim ITseeKopf mit Lipsync sein!!")]
    public Animator animator; // Verweise auf den Animator im Inspector
    private bool isAnimationPaused = false;
    public float setTriggerTime = 5f;
    public float setPauseTime = 2f;
    public AudioSource as1;
    public AudioClip audioclip1;
    //public TMP_Text debugInfo;

    void Start()
    {
        // Setzt den Trigger shakeHands nach 5 Sekunden
        Invoke("SetShakeHandsTrigger", setTriggerTime);
        //debugInfo = GameObject.Find("DebugInfo").GetComponent<TMP_Text>();
    }

    void SetShakeHandsTrigger()
    {
        animator.SetTrigger("shakeHands"); // Setzt den Trigger im Animator
        // Startet einen weiteren Timer, um die Animation nach 2 zusätzlichen Sekunden zu pausieren
        Invoke("PauseAnimation", setPauseTime);
    }

    void PauseAnimation()
    {
        if (!isAnimationPaused)
        {
            animator.speed = 0; // Pausiert die Animation
            isAnimationPaused = true;
        }
    }

    public void addDebugInfo(string debugInfo1)
    {
        //debugInfo.text = debugInfo.text + debugInfo1;
    }

    void OnTriggerEnter(Collider other)
    {
       //addDebugInfo("vor If nach trigger mit collider von: " + other.gameObject.name);
        
        // Überprüft, ob das GameObject, das den Trigger betritt, den Tag "Player" hat
        if (isAnimationPaused && other.CompareTag("Player"))
        {
            //addDebugInfo("im if animation paused und tag ist player");
            ResumeAnimation();
            as1.clip = audioclip1;
            as1.Play();
        }
    }

    public void ResumeAnimation()
    {
        animator.speed = 1; // Setzt die Animation fort
        isAnimationPaused = false;
    }
}
// 0.13, -0.04, -0.05