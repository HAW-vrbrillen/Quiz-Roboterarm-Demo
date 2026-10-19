using System.IO;
using UnityEngine;

public static class ProductionLogger1
{
    private static string logPath = Path.Combine(Application.persistentDataPath, "production_log.txt");

    public static void Log(string message)
    {
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
