using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ElementController : MonoBehaviour
{

    
    public GameObject Follower;
    public GameObject Predecessor; // vorgänger
    public GameObject MainStorage;

    // text prinzip: mehrer text elemente - alle hier definiert, aber aus anderen skripten über skript-link aktualisiert
    public TMP_Text Header;
    public TMP_Text infoPredecessor;
    public TMP_Text infoFollower;
    public TMP_Text testinfoForMove;

    // Start is called before the first frame update
    void Awake()
    {

        MainStorage = GameObject.Find("Main Storage");
        Header.text = "Machine Dashboard\n" + gameObject.name;
        Predecessor = MainStorage;
        Follower = MainStorage;
        infoFollower.text = "Follower : " + Follower.name;
        infoPredecessor.text = "Predecessor : " + Predecessor.name;
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}
