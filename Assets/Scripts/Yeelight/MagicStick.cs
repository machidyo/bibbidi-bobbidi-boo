using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using RyapUnity.Network;

public class MagicStick : MonoBehaviour
{
    private const float BIBBIDI_TIME_THRESHOLD = 0.5f;
    private const float BIBBIDI_STOP_TIME_THRESHOLD = 1.5f;
    private const float BIBBIDI_ACCELERATION_THRESHOLD = 1.2f;
    private const float BOO_ACCELERATION_THRESHOLD = 3.0f;
    
    public enum MagicStats
    {
        None,
        Bibbidi,
        Bobbidi,
        Boo,
        Debug,
    }
    
    public enum YeelightStats
    {
        Connecting,
        Connected,
        Error,
    }
    
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private UDPReceiver udpReceiver;
    
    [Header("DEBUG")]
    [SerializeField] private Text bigShakeText;

    public MagicStats MagicStatus { get; private set; } = MagicStats.None;
    public YeelightStats YeelightStatus { get; private set; } = YeelightStats.Connecting;
    
    private float bibbidiChargeTime = 0;
    private float bibbidiStoppingTime = 0;
    private int booCharge = 0;
    private float booTime = 0;
    
    private YeelightClient yeelightClient;
    private CancellationTokenSource yeelightHealthChaeckCanellationToken;
    private CancellationTokenSource yeelightCancellationToken;

    void Start()
    {
        yeelightClient = new YeelightClient();
        yeelightHealthChaeckCanellationToken = new CancellationTokenSource();
        ExecuteHealthCheck(yeelightHealthChaeckCanellationToken).Forget();
        yeelightCancellationToken = new CancellationTokenSource();
        SwitchYeelightRepeatedlyByMagic(yeelightCancellationToken).Forget();
    }

    private async void OnApplicationQuit()
    {
        yeelightCancellationToken?.Cancel();
        if (yeelightClient != null)
        {
            await yeelightClient.TurnOff();
        }
    }

    void Update()
    {
        if (MagicStatus == MagicStats.Debug) return;
        if (MagicStatus == MagicStats.Boo)
        {
            booTime += Time.deltaTime;
            if (booTime < 3.0)
            {
                Debug.Log("Boo time now");
                return;
            }
            else
            {
                Debug.Log("Boo time over");
                MagicStatus = MagicStats.None;
                booTime = 0;
            }
        }
        else
        {
            booTime = 0;
        }
        
        // big shake (to Boo or fault)
        if (udpReceiver.AccData[0] >= BOO_ACCELERATION_THRESHOLD ||
            udpReceiver.AccData[1] >= BOO_ACCELERATION_THRESHOLD ||
            udpReceiver.AccData[2] >= BOO_ACCELERATION_THRESHOLD)
        {
            if (bibbidiChargeTime < BIBBIDI_TIME_THRESHOLD)
            {
                Debug.Log($"fault: Boo but you have to do bibbidi before, {bibbidiChargeTime}");
                bibbidiChargeTime = 0;
            }
            else
            {
                Debug.Log("Boo");
                booCharge++;
            }
        }
        // small circle (to Bibbidi-Bobbidi-)
        else if (udpReceiver.AccData[0] >= BIBBIDI_ACCELERATION_THRESHOLD ||
                 udpReceiver.AccData[1] >= BIBBIDI_ACCELERATION_THRESHOLD ||
                 udpReceiver.AccData[2] >= BIBBIDI_ACCELERATION_THRESHOLD)
        {
            // Debug.Log("Bibbidi");
            bibbidiChargeTime += Time.deltaTime;
            bibbidiStoppingTime = 0;
        }
        else
        {
            // Debug.Log($"No magic, {bibbidiStoppingTime}");
            bibbidiStoppingTime += Time.deltaTime;
            if (bibbidiStoppingTime > BIBBIDI_STOP_TIME_THRESHOLD)
            {
                bibbidiChargeTime = 0;
            }
            else if (MagicStatus is MagicStats.Bibbidi or MagicStats.Bobbidi)
            {
                bibbidiChargeTime += Time.deltaTime;
            }
        }

        bigShakeText.text = $"{bibbidiChargeTime:F2}, {bibbidiStoppingTime:F2}, {booCharge}";
        
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (bibbidiChargeTime >= BIBBIDI_TIME_THRESHOLD && booCharge >= 1)
        {
            MagicStatus = MagicStats.Boo;

            bibbidiChargeTime = 0;
            bibbidiStoppingTime = 0;
            booCharge = 0;
        }
        else if (bibbidiChargeTime > 0.0)
        {
            MagicStatus = bibbidiChargeTime < BIBBIDI_TIME_THRESHOLD
                ? MagicStats.Bibbidi
                : MagicStats.Bobbidi;
        }
        else
        {
            MagicStatus = MagicStats.None;

            bibbidiChargeTime = 0;
            booCharge = 0;
        }
    }

    private int disconnectedCount = 0;
    private async UniTask ExecuteHealthCheck(CancellationTokenSource token)
    {
        // 初期起動時は3秒待ってからチェックするようにする
        await UniTask.Delay(3000, cancellationToken: token.Token);
        
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (yeelightClient.IsConnected())
                {
                    YeelightStatus = YeelightStats.Connected;
                    disconnectedCount = 0;
                }
                else
                {
                    YeelightStatus = YeelightStats.Connecting;
                    disconnectedCount++;
                    
                    if (disconnectedCount < 3)
                    {
                        Debug.LogWarning($"Yeelightとの接続に問題が発生している可能性があります。チェック{disconnectedCount}回目。");
                    }
                    else
                    {
                        throw new ApplicationException($"Yeelightとの死活監視（チェック{disconnectedCount}回目）に検出されました");
                    }
                }

                await UniTask.Delay(30000, cancellationToken: token.Token);
            }
            catch (Exception e)
            {
                YeelightStatus = YeelightStats.Error;
                Debug.LogError($"Yeelightとの接続に問題が発生しました、再接続にトライします。{e}");
                
                // 少し間をおいてから接続に再挑戦する
                await UniTask.Delay(3000, cancellationToken: token.Token);
                disconnectedCount = 0;
                //await Reconnect();
            }
        }
    }

    private bool isYeelgihtOn = false;
    private async UniTask SwitchYeelightRepeatedlyByMagic(CancellationTokenSource tokenSource)
    {
        while (!tokenSource.IsCancellationRequested)
        {
            try
            {
                switch (MagicStatus)
                {
                    case MagicStats.None:
                        if (isYeelgihtOn)
                        {
                            Debug.Log("None : TurnOff");
                            isYeelgihtOn = false;
                            await yeelightClient.TurnOff();
                            await UniTask.Delay(1000);
                        }
                        else
                        {
                            Debug.Log("None : Wainting...");
                            await UniTask.Delay(500);
                        }
                        break;
                    case MagicStats.Bibbidi:
                    case MagicStats.Bobbidi:
                        Debug.Log("BibbidiBobbidi");
                        soundManager.PlaySound(SoundManager.SoundNames.BibbidiBobbidi);
                        var isBibbidiBobbidi = await yeelightClient.BibbidiBobbidi();
                        if (isBibbidiBobbidi)
                        {
                            isYeelgihtOn = true;
                            await UniTask.Delay(1000);
                        }
                        else
                        {
                            Debug.Log("ライトに未接続、あるいはすでに実行中のため、BibbidiBobbidiは1回スキップされました");
                            await UniTask.Delay(500);
                        }
                        break;
                    case MagicStats.Boo:
                        Debug.Log("Boo");
                        soundManager.PlaySound(SoundManager.SoundNames.Boo);
                        var isBoo = await yeelightClient.Boo();
                        if (isBoo)
                        {
                            isYeelgihtOn = true;
                            await UniTask.Delay(1000);
                        }
                        else
                        {
                            Debug.Log("ライトに未接続、あるいはすでに実行中のため、Booは1回スキップされました");
                            await UniTask.Delay(500);
                        }
                        break;
                    case MagicStats.Debug:
                        // Debug Modeのときは何もせずに1秒待つ
                        await UniTask.Delay(1000);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Yeelightとの間で何かしらの問題（{e}）が発生しました。少し間を置いてからリトライします。");
                await UniTask.Delay(3000);
            }
        }
    }

    public void SwitchDebugMode(bool isDebug)
    {
        MagicStatus = isDebug ? MagicStats.Debug : MagicStats.None;
    }
    
    public async UniTask Reconnect()
    {
        yeelightClient = new YeelightClient();
        await UniTask.Delay(1000);
    }
    
    public async UniTask TurnOff()
    {
        await yeelightClient.TurnOff();
    }
    
    public async UniTask BibbidiBobbidi()
    {
        await yeelightClient.BibbidiBobbidi();
    }
    
    public async UniTask Boo()
    {
        await yeelightClient.Boo();
    }
}
