using TMPro;
using UnityEngine;
using System;

public class waterControl : MonoBehaviour
{
    public GameObject water1;
    public float normalHeight = 0.038f; // Normale Wasserhöhe
    public float maxHeight = 0.05f; // Maximale Wasserhöhe
    public float risespeed = 0.00457f; // Geschwindigkeit des Wasseranstiegs
    public ParticleSystem particle1; // Partikelsystem für die Fabrik
    public AudioSource audios1; // Audioquelle für Soundeffekte
    public AudioClip alarmclip1; // Alarm-Audioclip
    private Vector3 waterOriPosition;
    private bool factoryRunning = false;
    private bool factoryPaused = false;
    private TerrainMover terrainMover;
    public GameObject windmill;
    public TMP_Text waterinfo1; //zeige wasserlevel aktuell und delta
    public TMP_Text waterinfo2; //zeige wasserlevel aktuell und delta
    public TMP_Text co2InfoText; // TMP_Text Element für CO2-Anzeige
    public GameObject alarmText1;

    private float co2Emission = 0f;
    private float factoryRunTime = 0f;
    private const float CO2_PER_SECOND = 1.11f; // 1,11 kg CO2/sec

    private float displayWaterLevelMin = 0f;
    private float displayWaterLevelMax = 1.95f;

    void Start()
    {
        terrainMover = FindObjectOfType<TerrainMover>();
        waterOriPosition = water1.transform.localPosition;
        particle1.Stop();
        factoryRunning = false;
        windmill.SetActive(false);
        alarmText1.SetActive(false);
        UpdateCO2Display();
        UpdateWaterLevelDisplay();
    }

    public void PauseFactoryAndResetWater()
    {
        if (!factoryPaused)
        {
            factoryPaused = true;
            StopFactory();
            ResetWaterLevel();
        }
    }

    public void ResumeFactory()
    {
        if (factoryPaused)
        {
            factoryPaused = false;
            StartFactory();
        }
    }

    public void StopFactory()
    {
        if (factoryRunning)
        {
            factoryRunning = false;
            particle1.Stop();
            audios1.Stop();
            windmill.SetActive(true);
        }
    }

    public void StartFactory()
    {
        if (!factoryRunning && !factoryPaused)
        {
            factoryRunning = true;
            particle1.Play();
            windmill.SetActive(false);
        }
    }

    public void resetWorld()
    {
        StopFactory();
        factoryPaused = false;
        windmill.SetActive(false);
        alarmText1.SetActive(false);
        ResetWaterLevel();
        co2Emission = 0f;
        factoryRunTime = 0f;
        UpdateCO2Display();
    }

    private void ResetWaterLevel()
    {
        water1.transform.localPosition = waterOriPosition;
    }

    void Update()
    {
        if (factoryRunning && !factoryPaused)
        {
            factoryRunTime += Time.deltaTime;
            co2Emission += CO2_PER_SECOND * Time.deltaTime;
            UpdateCO2Display();

            if (water1.transform.localPosition.y < waterOriPosition.y + maxHeight)
            {
                water1.transform.Translate(0, risespeed * Time.deltaTime, 0);
                UpdateWaterLevelDisplay();
            }
            else
            {
                StopFactory();
                alarmText1.SetActive(true);
                if (audios1 != null && alarmclip1 != null)
                {
                    audios1.PlayOneShot(alarmclip1);
                }
            }
        }
    }

    private void UpdateWaterLevelDisplay()
    {
        float currentWaterHeight = water1.transform.localPosition.y - waterOriPosition.y;
        float waterProgress = Mathf.InverseLerp(0, maxHeight, currentWaterHeight);
        float displayWaterLevel = Mathf.Lerp(displayWaterLevelMin, displayWaterLevelMax, waterProgress);

        waterinfo1.text = $"Waterlevel aktuell: {displayWaterLevel:F2}m\n" +
                          $"Waterlevel Max: {displayWaterLevelMax:F2}m";
        waterinfo2.text = waterinfo1.text;
    }

    private void UpdateCO2Display()
    {
        string displayText = "Gießerei 20000 t CO2 /Jahr\n" +
                             "1,11 kg CO2/sec\n" +
                             $"Ausstoß: Dauer: {TimeSpan.FromSeconds(factoryRunTime):hh\\:mm\\:ss}\n" +
                             $"CO2: {co2Emission:F2} kg";

        co2InfoText.text = displayText;
    }
}