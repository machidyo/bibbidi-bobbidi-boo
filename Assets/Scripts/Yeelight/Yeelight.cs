using UnityEngine;
using YeelightAPI;

public class Yeelight
{
    private const string YEELIGHT_IP = "192.168.1.23";
    
    private static Device device;
    
    public static async void SwitchLight(bool isOn)
    {
        if (device == null)
        {
            device = new Device(YEELIGHT_IP);
            await device.Connect();
            Debug.Log("Connected to Yeelight");
        }
        
        if (!isOn)
        {
            TurnOff();
        }
        else
        {
            TurnOn();
        } 
    }

    public static async void TurnOff()
    {
        await device.SetPower(false);
    }

    public static async void TurnOn()
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
