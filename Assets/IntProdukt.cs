using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ArbeitsplanEintrag
{
    public int workcellID;
    public float bearbeitungszeit;
}

public class IntProdukt : MonoBehaviour
{
    public float speed = 1.0f;
    private Queue<ArbeitsplanEintrag> arbeitsplanQueue;
    private ArbeitsplanEintrag aktuellerEintrag;
    private IntWC zielWorkcell;

    public void SetArbeitsplan(List<ArbeitsplanEintrag> arbeitsplan)
    {
        arbeitsplanQueue = new Queue<ArbeitsplanEintrag>(arbeitsplan);
        SetNextZiel();
    }

    void Update()
    {
        if (zielWorkcell != null)
        {
            MoveToZiel();
        }
    }

    private void MoveToZiel()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, zielWorkcell.transform.position, step);

        if (Vector3.Distance(transform.position, zielWorkcell.transform.position) < 0.001f)
        {
            zielWorkcell.ReceiveProduct(this, aktuellerEintrag.bearbeitungszeit);
            SetNextZiel();
        }
    }

    private void SetNextZiel()
    {
        if (arbeitsplanQueue.Count > 0)
        {
            aktuellerEintrag = arbeitsplanQueue.Dequeue();
            zielWorkcell = FindWorkcellByID(aktuellerEintrag.workcellID);
        }
        else
        {
            zielWorkcell = null;
        }
    }

    private IntWC FindWorkcellByID(int id)
    {
        IntWC[] alleWorkcells = FindObjectsOfType<IntWC>();
        foreach (IntWC wc in alleWorkcells)
        {
            if (wc.workcellID == id)
            {
                return wc;
            }
        }
        return null; // Keine Workcell mit dieser ID gefunden
    }
}
