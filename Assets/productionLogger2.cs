using System.IO;
using UnityEngine;

public class productionLogger2 : MonoBehaviour
{
    private static string logPath;
    public static productionLogger2 Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            logPath = Path.Combine(Application.persistentDataPath, "production_log.txt");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Log(string message)
    {
        if (Instance == null)
        {
            Debug.LogError("ProductionLogger not initialized.");
            return;
        }

        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine(message);
        }
    }

    public static void ClearLog()
    {
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }
    }
}
