using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PythonRunner
{
    private static readonly string pyExePath = @"c:\Users\yorih\anaconda3\python.exe";
    private static readonly  string pyCodePath = @"d:\14_python\Yeelight.py";

    public static void SwitchLight(bool isOn)
    {
        var arg = isOn ? Random.Range(1, 8) : 0;
        var processStartInfo = new ProcessStartInfo
        {
            FileName = pyExePath,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            Arguments = pyCodePath + " " + arg, 
        };

        var process = Process.Start(processStartInfo);
        
        var streamReader = process.StandardOutput;
        var pythonSdtOut = streamReader.ReadLine();

        process.WaitForExit();
        process.Close();
        
        Debug.Log($"python debug log: {pythonSdtOut}");
    }
}