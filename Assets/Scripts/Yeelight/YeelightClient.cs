using UnityEngine;
using Cysharp.Threading.Tasks;
using YeelightAPI;

public class YeelightClient
{
    private const string YEELIGHT_IP = "192.168.1.23";
    
    private Device device;
    
    public YeelightClient()
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

    ~YeelightClient()
    {
        device?.Disconnect();
        device?.Dispose();
    }
    
    public async UniTask TurnOff()
    {
        if (!IsConnected()) return;
        await device.SetPower(false);
    }

    public async UniTask TurnOn()
    {
        if (!IsConnected()) return;
        await device.SetPower();
        await device.SetRGBColor(255, 255, 255);
    }

    public async UniTask Toggle()
    {
        if (!IsConnected()) return;
        await device.Toggle();
    }

    private bool IsConnected()
    {
        if (device is not { IsConnected: true })
        {
            Debug.LogWarning("Yeelightに未接続です。");
        }
        return device is { IsConnected: true };
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
