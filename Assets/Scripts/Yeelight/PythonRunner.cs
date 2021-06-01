using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PythonRunner
{
    // Set up a file(ex python.exe, py.exe) to run python
    private static readonly string pyExePath = @"c:\path\python.exe";
    // Set up python code
    private static readonly  string pyCodePath = @"d:\path\Yeelight.py";

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