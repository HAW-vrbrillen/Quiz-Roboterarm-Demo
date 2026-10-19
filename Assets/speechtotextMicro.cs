using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;
using System;
using TMPro;
using UnityEngine.UI;

public class speechtotextMicro : MonoBehaviour
{
    // Erstellen Sie eine Variable für das Mikrofon
    private AudioClip microphoneInput;
    private string openAIUrl = "https://api.openai.com/v1/completions";
    private string apiKey = "Ihr OpenAI API-Schlüssel";
    public spezData spezData1;
    private bool flagrec = false;
    //public TMP_Text output;
    public Text output;
    private void Start()
    {
        apiKey = spezData1.openaikey;
    }

    // Starten Sie die Aufnahme vom Mikrofon
    public void startstopMicro()
    {
        if (!flagrec)
        {
            
            output.text = "Aufnahme läuft!";
            microphoneInput = Microphone.Start(Microphone.devices[0], true, 10, 44100);
            flagrec = true;
        }
        else
        {
            output.text = "Aufnahme gestoppt!";
            Microphone.End("Eingabegerät");
            var audioData = ConvertAudioClipToByteArray(microphoneInput);
            var result = SendAudioToOpenAI(audioData);
            Debug.Log(result);
            output.text = output.text + "\n" + result.ToString();
            flagrec = false;
        }
    }

    

    // Konvertieren Sie AudioClip-Daten in ein Byte-Array
    byte[] ConvertAudioClipToByteArray(AudioClip clip)
    {
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);
        var byteArray = new byte[samples.Length * 4];
        Buffer.BlockCopy(samples, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    // Senden Sie die Audiodaten an die OpenAI-API
    string SendAudioToOpenAI(byte[] audioData)
    {
        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(openAIUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers["Authorization"] = "Bearer " + apiKey;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"audio\":\"" + Convert.ToBase64String(audioData) + "\"}";
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return result;
            }
        }
        catch (WebException ex)
        {
            using (var stream = ex.Response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                Debug.Log(result);
                return result;
            }
        }
    }

}
