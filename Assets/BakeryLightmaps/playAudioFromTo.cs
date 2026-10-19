using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{
    private AudioSource as1;
    public AudioClip audioClip;
    public int From;
    public int To;
    

    void Start()
    {
        as1 = GetComponent<AudioSource>();
        //PlayClipFromTo(5, 7);
    }

    public void PlayClipFromTo()
    {
        if(as1 == null) return;

        as1.time = From; // Setzt die Startzeit des AudioClips
        as1.clip= audioClip;
        as1.Play(); // Startet die Wiedergabe
        Invoke("StopAudio(as1)", To - From); // Ruft StopAudio nach der Differenz zwischen endSeconds und startSeconds auf
    }

    void StopAudio()
    {
        if(as1.isPlaying) 
            as1.Stop(); // Stoppt die Wiedergabe
    }
}
