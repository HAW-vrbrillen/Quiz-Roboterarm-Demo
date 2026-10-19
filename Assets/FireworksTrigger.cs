using TMPro;

using UnityEngine;

public class FireworksTrigger : MonoBehaviour
{
    public ParticleSystem ps1;
    public ParticleSystem ps2;


    public void TriggerFireworks()
    {
        ps1.Play();
        ps2.Play();
    }


    public void StopFireworks()
    {
        ps1.Stop();
        ps2.Stop();
    }

}
