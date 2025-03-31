using UnityEngine;
using Cysharp.Threading.Tasks;
using YeelightAPI;

public class Yeelight
{
    private const string YEELIGHT_IP = "192.168.1.23";
    
    private bool isConnected
    {
        get
        {
            if (device is not { IsConnected: true })
            {
                Debug.LogWarning("Yeelightに未接続です。");
            }
            return device is { IsConnected: true };
        }
    }

    private static Device device;
    
    public Yeelight()
    {
        Debug.Log("Yeelightの初期化を開始します。");
        device = new Device(YEELIGHT_IP);
        Connect().Forget();
    }
    private async UniTask Connect()
    {
        var isSuccess = await device.Connect();
        if (isSuccess)
        {
            Debug.Log("Yeelightに接続しました。");
        }
        else
        {
            Debug.LogError("Yeelightの接続に失敗しました。");
        }
    }
    
    public async UniTask TurnOff()
    {
        if (!isConnected) return;
        await device.SetPower(false);
    }

    public async UniTask TurnOn()
    {
        if (!isConnected) return;
        await device.SetPower();
        await device.SetRGBColor(255, 255, 255);
    }

    public async UniTask Toggle()
    {
        if (!isConnected) return;
        await device.Toggle();
    }

    private async void TurnOnWithRandomColor()
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
