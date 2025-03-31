using UnityEngine;
using Cysharp.Threading.Tasks;
using YeelightAPI;

public class Yeelight
{
    private const string YEELIGHT_IP = "192.168.1.23";
    
    private static Device device;

    static Yeelight()
    {
        device = new Device(YEELIGHT_IP);
        Connect().Forget();
    }
    private static async UniTask Connect()
    {
        await device.Connect();
        Debug.Log("Connected to Yeelight");
    }
    
    public static async UniTask SwitchLight(bool isOn)
    {
        if (!isOn)
        {
            await TurnOff();
        }
        else
        {
            await TurnOn();
        } 
    }

    public static async UniTask TurnOff()
    {
        await device.SetPower(false);
    }

    public static async UniTask TurnOn()
    {
        await device.SetPower();
        await device.SetRGBColor(255, 255, 255);
    }

    private static async void TurnOnWithRandomColor()
    {
        var arg = Random.Range(1, 8);
        Debug.Log($"arg = {arg}");
        
        await device.SetPower();
        // await device.SetBrightness(10);
        var color = arg switch
        {
            1 => new Color(255, 255,   0),
            2 => new Color(255,   0, 255),
            3 => new Color(  0, 255, 255),
            4 => new Color(255,   0,   0),
            5 => new Color(  0, 255,   0),
            6 => new Color(  0,   0, 255),
            _ => new Color(255, 255, 255)
        };
        await device.SetRGBColor((int)color.r, (int)color.g, (int)color.b);
    }
}
