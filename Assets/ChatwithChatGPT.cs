using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;

public class ChatGPT : MonoBehaviour
{
    private string url = "https://api.openai.com/v1/chat/completions";
    public spezData spezData1;
    private string apiKey = "YOUR_API_KEY";
    private string prompt1;
    public TMP_Text output1;
    public TMP_InputField input1;
    public int maxTokens = 40;

    void Start()
    {
        apiKey = spezData1.openaikey;
        StartCoroutine(PostRequest());
    }
    public void getResponse()
    {
        prompt1 = input1.text;
        //StartCoroutine(PostRequest());
    }

    IEnumerator PostRequest()
    {
        // Erstellen Sie ein neues UnityWebRequest und weisen Sie die URL zu
        UnityWebRequest www = new UnityWebRequest(url, "POST");

        // Erstellen Sie die JSON-Daten für die Anforderung
        // $ ermöglicht string interpolation in C#
        string jsonData = $"{{\"model\":\"gpt-3.5-turbo\",\"messages\":[{{\"role\":\"user\",\"content\":\"{prompt1}\"}}],\"max_tokens\":{maxTokens} }}";
        // Konvertieren Sie die JSON-Daten in einen Byte-Array
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // Weisen Sie den Byte-Array und den Inhaltstyp zu
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.uploadHandler.contentType = "application/json";

        // Setzen Sie den Autorisierungsheader auf
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Erstellen Sie einen DownloadHandlerBuffer und weisen Sie ihn zu
        www.downloadHandler = new DownloadHandlerBuffer();

        // Senden Sie die Anforderung und warten Sie auf die Antwort
        yield return www.SendWebRequest();

        // Überprüfen Sie, ob es einen Netzwerkfehler gab
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("fehler : " + www.error);
        }
        else
        {
            // Zeigen Sie die Antwort an
            var jsonResponse = JSON.Parse(www.downloadHandler.text);
            string content = jsonResponse["choices"][0]["message"]["content"];
            string usage = jsonResponse["usage"].ToString();

            Debug.Log("Content: " + content);

            Debug.Log("Usage: " + usage);
            output1.text= "Content: " + content + "\n Usage: " + usage;

        }
    }
}
