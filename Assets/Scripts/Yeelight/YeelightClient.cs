using UnityEngine;
using Cysharp.Threading.Tasks;
using YeelightAPI;
using YeelightAPI.Models.ColorFlow;

public class YeelightClient
{
    private const string YEELIGHT_IP = "192.168.1.23";
    
    private Device device;
    private bool isRunning = false;
    
    public YeelightClient()
    {
        Debug.Log("Yeelightの初期化を開始します。");
        device = new Device(YEELIGHT_IP);
        Connect().Forget();
    }
    private async UniTask<bool> Connect()
    {
        Debug.Log("Yeelightに接続を開始します。");
        var isSuccess = await device.Connect();
        if (isSuccess)
        {
            Debug.Log("Yeelightに接続しました。");
        }
        else
        {
            Debug.LogError("Yeelightの接続に失敗しました。");
        }
        return isSuccess;
    }

    ~YeelightClient()
    {
        Debug.Log("START デコンストラクタ");
        device?.Disconnect();
        device?.Dispose();
        Debug.Log("END   デコンストラクタ");
    }
    
    public async UniTask<bool> Reconnect()
    {
        Debug.Log("Disconnect");
        device?.Disconnect();
        Debug.Log("Dispose");
        device?.Dispose();
        
        Debug.Log("Yeelightの再接続を開始します。");
        device = new Device(YEELIGHT_IP);
        return await Connect();
    }
    
    public async UniTask TurnOff()
    {
        if (!IsConnected()) return;
        if (isRunning) return;
        
        isRunning = true;
        await device.StopColorFlow(); 
        await device.SetPower(false);
        isRunning = false;
    }

    public async UniTask TurnOn()
    {
        if (!IsConnected()) return;
        if (isRunning) return;
        
        isRunning = true;
        await device.SetPower();
        await device.SetBrightness(100);
        await device.SetRGBColor(255, 255, 255);
        isRunning = false;
    }

    public async UniTask TurnOnALittle()
    {
        if (!IsConnected()) return;
        if (isRunning) return;
        
        isRunning = true;
        await device.SetPower();
        
        // Sample
        // var flow = new ColorFlow(0, ColorFlowEndAction.Restore)
        // {
        //     new ColorFlowRGBExpression(255, 0, 0, 1, 500),      // color : red / brightness : 1% / duration : 500
        //     new ColorFlowRGBExpression(0, 255, 0, 100, 500),    // color : green / brightness : 100% / duration : 500
        //     new ColorFlowRGBExpression(0, 0, 255, 50, 500),     // color : blue / brightness : 50% / duration : 500
        //     new ColorFlowSleepExpression(2000),                 // sleeps for 2 seconds
        //     new ColorFlowTemperatureExpression(2700, 100, 500), // color temperature : 2700k / brightness : 100 / duration : 500
        //     new ColorFlowTemperatureExpression(5000, 1, 500)    // color temperature : 5000k / brightness : 100 / duration : 500
        // };

        var flow = new ColorFlow(1, ColorFlowEndAction.Keep)
        {
            GetRandomColor(300),
            GetRandomColor(300),
            GetRandomColor(400),
        };

        await device.StartColorFlow(flow);
        await UniTask.Delay(1000);
        isRunning = false;
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
    
    private ColorFlowRGBExpression GetRandomColor(int duration)
    {
        var color = Random.Range(1, 8);
        var brightness = Random.Range(1, 8);
        return color switch
        {
            1 => new ColorFlowRGBExpression(255, 255,   0, brightness, duration),
            2 => new ColorFlowRGBExpression(255,   0, 255, brightness, duration),
            3 => new ColorFlowRGBExpression(  0, 255, 255, brightness, duration),
            4 => new ColorFlowRGBExpression(255,   0,   0, brightness, duration),
            5 => new ColorFlowRGBExpression(  0, 255,   0, brightness, duration),
            6 => new ColorFlowRGBExpression(  0,   0, 255, brightness, duration),
            _ => new ColorFlowRGBExpression(255, 255, 255, brightness, duration)
        };
    }
}
