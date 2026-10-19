using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MachineBehaviour : MonoBehaviour
{
    public TMP_Text statusText;
    private string currentProduct = "";

    public void SetProduct(string productId)
    {
        currentProduct = productId;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        statusText.text = "Aktuelles Produkt: " + currentProduct;
    }
}
