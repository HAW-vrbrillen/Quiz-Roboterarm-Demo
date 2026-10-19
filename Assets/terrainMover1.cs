using UnityEngine;

public class TerrainMover : MonoBehaviour
{
    public GameObject terrain;
    private Vector3 lastPosition;
    private waterControl WaterControl; // Referenz auf das waterControl Skript


    void Start()
    {
        lastPosition = transform.position;
        WaterControl = FindObjectOfType<waterControl>();

    }


    void Update()
    {
        Vector3 currentPosition = transform.position;
        Vector3 deltaMovement = currentPosition - lastPosition;

        if (deltaMovement != Vector3.zero)
        {
            WaterControl.PauseFactoryAndResetWater();
            terrain.transform.position += deltaMovement;
            WaterControl.resetWorld();
        }

        lastPosition = currentPosition;
    }

   
}
