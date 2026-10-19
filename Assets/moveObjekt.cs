using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class moveObjekt : MonoBehaviour
{
    [Header("-x=left, +x=right, z=forward, -z=backward")]
    public float xspeed = 1.0f;
    public float yspeed = 0f;
    public float zspeed = 1.0f;

    public GameObject objectToMove;

    public bool moveON = false;
    public bool cursorMove = false; // Neues Flag für die Steuerung mit der Tastatur

    //ElementController ec1;

    

    private void Start()
    {
        //ec1 = GetComponent<ElementController>();
    }
    
    void Update()
    {
        if (moveON)
        {
            objectToMove.transform.position += new Vector3(xspeed * Time.deltaTime, yspeed * Time.deltaTime, zspeed * Time.deltaTime);
        }

        // Überprüft, ob die Tastatursteuerung aktiviert ist
        if (cursorMove)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                objectToMove.transform.position += new Vector3(-xspeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                objectToMove.transform.position += new Vector3(xspeed * Time.deltaTime, 0, 0);
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                objectToMove.transform.position += new Vector3(0, 0, zspeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                objectToMove.transform.position += new Vector3(0, 0, -zspeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.R))
            {
                objectToMove.transform.position += new Vector3(0, yspeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.F))
            {
                objectToMove.transform.position += new Vector3(0, -yspeed * Time.deltaTime, 0);
            }
        }

        //ec1.testinfoForMove.text = "Aktuelle X Position : " + transform.position.x.ToString();
    }
}
