using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class CubeFormationChecker : MonoBehaviour
{
    public GameObject[] cubes;
    public GameObject table;
    public TMP_Text cubeBuildingResult;
    public TMP_Text cubeThrowResult;
    public TMP_Text cubeShiftResult;
    public PositionResetter positionResetter;

    public AudioSource as1;
    public AudioClip aclip1;
    public AudioClip aclipWerfen;
    public AudioClip aclipBauen;
    public AudioClip aclipUmwerfen;
    public AudioClip aclipReset;
    public AudioClip aclipWelcome;
    public bool AufgabeErledigt = false;

    private int phase = 0;
    private Vector3[] initialPositions;
    private Quaternion[] initialRotations;

    void Start()
    {
        initialPositions = cubes.Select(cube => cube.transform.position).ToArray();
        initialRotations = cubes.Select(cube => cube.transform.rotation).ToArray();
        setPhase(0); // ansage alles startet neu
        StartCoroutine(CallFunctionPeriodically());
        
    }

    public void setPhase(int phaseNew)
    {
        if (phaseNew == 1) { as1.PlayOneShot(aclipWerfen); }
        if (phaseNew == 2) { as1.PlayOneShot(aclipBauen); }
        if (phaseNew == 3) { as1.PlayOneShot(aclipUmwerfen); }
        if (phaseNew == 0) { as1.PlayOneShot(aclipReset); }
        phase = phaseNew;
        AufgabeErledigt = false;  // Reset der AufgabeErledigt Flag bei Phasenwechsel
        
    }

    

    public void asPlayOneShotWelcome()
    {
        as1.PlayOneShot(aclipWelcome);
    }

    public void presentTheFormationResult()
    {
        // Alle Textfarben zurücksetzen
        cubeBuildingResult.color = Color.black; // 202408131808
        cubeThrowResult.color = Color.black; // 202408131808
        cubeShiftResult.color = Color.black; // 202408131808

        switch (phase)
        {
            case 4:
                ResetCubes(); 
                break;
            case 0:  // reset
                ResetCubes();
                cubeShiftResult.text = "Aufgabe nicht aktiv.";
                cubeBuildingResult.text = "Aufgabe nicht aktiv.";
                cubeThrowResult.text = "Aufgabe nicht aktiv.";
                break;
            case 1:             // würfel hinter Tafel werfen
                cubeShiftResult.color = Color.red;
                if (CheckCubesThrown() )
                {
                    cubeShiftResult.text = "Bravo! Alle 6 korrekt geworfen.";
                    if (!AufgabeErledigt)
                    {
                        as1.PlayOneShot(aclip1);
                        AufgabeErledigt = true;
                    }
                    
                    phase = 4;
                }
                else
                {
                    int anzerfolgshifted = cubes.Count(cube => cube.transform.position.z > table.transform.position.z + 0.2f &&
        cube.transform.position.y < table.transform.position.y - 0.1f);
                    cubeShiftResult.text = "Ihr Erfolg von 6 bisher " + anzerfolgshifted;
                }
                break;
            case 2:             // formation aufstellen
                cubeBuildingResult.color = Color.red;
                if (CheckCubeFormation())
                {
                    cubeBuildingResult.text = "Super! 3-2-1 Formation ";

                    if (!AufgabeErledigt)
                    {
                        as1.PlayOneShot(aclip1);
                        Invoke("PlaySecondClip", aclip1.length);
                        //as1.PlayOneShot(aclipUmwerfen);
                        AufgabeErledigt = true;
                    }
                    //ResetCubes();
                    AufgabeErledigt = false;
                    phase = 3;

                }
                else
                {
                    cubeBuildingResult.text = "Noch nicht korrekt.";
                }
                break;
            case 3:
                cubeThrowResult.color = Color.red;
                if (CheckNoCubeStapled())
                {
                    cubeThrowResult.text = "Toll! Alle Stapel umgeworfen.";
                    if (!AufgabeErledigt)
                    {
                        as1.PlayOneShot(aclip1);
                        AufgabeErledigt = true;
                    }
                    phase = 4;
                    //ResetCubes();
                }
                else
                {
                    cubeThrowResult.text = "Noch umzuwerfen: " + cubes.Count(cube => cube.transform.position.y > table.transform.position.y + 0.1f);
                    // Achtung transform.position bezieht sich auf world coordinates, table y ist in Mitte vom Tisch also oberfläche 
                    // ist table y + 0.015 dann noch mitte vom cube dazu also 0.05 dann sind wir bei  y+0.065
                }

                break;
        }
    }

    void PlaySecondClip()
    {
        as1.PlayOneShot(aclipUmwerfen);
    }

    System.Collections.IEnumerator CallFunctionPeriodically()
    {
        while (true)
        {
            presentTheFormationResult();
            yield return new WaitForSeconds(1);
        }
    }

    bool CheckCubesThrown()
    {
        return cubes.All(cube => cube.transform.position.z > table.transform.position.z + 0.2f && 
        cube.transform.position.y < table.transform.position.y - 0.1f);
    }

    bool CheckCubeFormation()
    {
        if (cubes.Length != 6)
        {
            Debug.LogError("Es sollten genau 6 Cubes vorhanden sein.");
            return false;
        }

        var sortedCubes = cubes.OrderBy(cube => cube.transform.position.y).ToList();
        float baseHeight = sortedCubes[0].transform.position.y;
        bool level1Correct = sortedCubes.Take(3).All(cube => Mathf.Abs(cube.transform.position.y - baseHeight) < 0.05f);
        bool level2Correct = sortedCubes.Skip(3).Take(2).All(cube => Mathf.Abs(cube.transform.position.y - (baseHeight + 0.1f)) < 0.05f);
        bool level3Correct = Mathf.Abs(sortedCubes.Last().transform.position.y - (baseHeight + 0.2f)) < 0.05f;

        return level1Correct && level2Correct && level3Correct;
    }

    bool CheckNoCubeStapled()
    {
        return cubes.All(cube => cube.transform.position.y <= table.transform.position.y + 0.1f);
    }

    void ResetCubes()
    {
        positionResetter.ResetPositions();

    }
    // 202408132300: Neue Methode hinzugefügt, um das Spiel auf verschiedene Phasen zurückzusetzen
    // 202408132300: Neue Methode hinzugefügt, um das Spiel je nach aktueller Phase zurückzusetzen
    public void ResetCurrentPhase()
    {
        if (phase == 1)
        {
            // Zurücksetzen von Phase 1: Erfolge und Positionen der Würfel und roten Kugel zurücksetzen
            ResetCubes();
            AufgabeErledigt = false; // 202408132300 AufgabeErledigt zurücksetzen für Phase 1
        }
        else if (phase == 2 || phase == 3)
        {
            // Zurücksetzen auf den Anfangszustand von Phase 2, ohne den Erfolg von Phase 1 zu ändern
            ResetCubes();
            AufgabeErledigt = false; // 202408132300 AufgabeErledigt zurücksetzen für Phase 2 und 3
            phase = 2; // 202408132300 Phase auf 2 setzen, damit es nicht in Phase 1 zurückgeht
        }
    }
}
