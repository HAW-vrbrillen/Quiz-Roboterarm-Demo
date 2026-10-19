using UnityEngine;
using TMPro;

public class DynamicTable : MonoBehaviour
{
    public GameObject textPrefab; // Prefab für die Zelle
    public int rows = 5;
    public int columns = 3;
    public Transform panelTransform; // Das Panel, das die Grid Layout Group hat

    void Start()
    {
        GenerateTable();
    }

    void GenerateTable()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject cell = Instantiate(textPrefab, panelTransform);
                TextMeshProUGUI cellText = cell.GetComponent<TextMeshProUGUI>();
                cellText.text = $"Row {i + 1}, Col {j + 1}";
            }
        }
    }
}
