using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using RyapUnity.Network;

public class MagicStick : MonoBehaviour
{
    private const float BIBBIDI_TIME_THRESHOLD = 1.0f;
    private const float BIBBIDI_STOP_TIME_THRESHOLD = 1.0f;
    private const float BIBBIDI_ACCELERATION_THRESHOLD = 2.0f;
    private const float BOO_ACCELERATION_THRESHOLD = 7.0f;
    
    private enum MagicStats
    {
        None,
        Bibbidi,
        Bobbidi,
        Boo
    }
    
    [SerializeField] private UDPReceiver udpReceiver;
    
    [SerializeField] private Text smallCircleText;
    [SerializeField] private Text bigShakeText;

    private MagicStats magicStats = MagicStats.None;
    
    private float bibbidiChargeTime = 0;
    private float bibbidiStoppingTime = 0;
    private int booCharge = 0;
    private float booTime = 0;
    
    private YeelightClient yeelightClient;

    private CancellationTokenSource yeelightSwithcer;

    void Start()
    {
        yeelightClient = new YeelightClient();
        yeelightSwithcer = new CancellationTokenSource();
        SwitchYeelightRepeatedlyByMagic(yeelightSwithcer).Forget();
    }

    private async void OnApplicationQuit()
    {
        yeelightSwithcer?.Cancel();
        if (yeelightClient != null)
        {
            await yeelightClient.TurnOff();
        }
    }

    void Update()
    {
        if (magicStats == MagicStats.Boo)
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
                magicStats = MagicStats.None;
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
            else if (magicStats is MagicStats.Bibbidi or MagicStats.Bobbidi)
            {
                bibbidiChargeTime += Time.deltaTime;
            }
        }

        bigShakeText.text = $"{bibbidiChargeTime:F2}, {bibbidiStoppingTime:F2}, {booCharge}";
        
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (bibbidiChargeTime >= BIBBIDI_TIME_THRESHOLD && booCharge >= 1)
        {
            magicStats = MagicStats.Boo;

            bibbidiChargeTime = 0;
            bibbidiStoppingTime = 0;
            booCharge = 0;
        }
        else if (bibbidiChargeTime > 0.0)
        {
            magicStats = bibbidiChargeTime < BIBBIDI_TIME_THRESHOLD
                ? MagicStats.Bibbidi
                : MagicStats.Bobbidi;
        }
        else
        {
            magicStats = MagicStats.None;
            
            bibbidiChargeTime = 0;
            booCharge = 0;
        }
        
        smallCircleText.text = $"{magicStats}";
    }

    private async UniTask SwitchYeelightRepeatedlyByMagic(CancellationTokenSource tokenSource)
    {
        while (!tokenSource.IsCancellationRequested)
        {
            try
            {
                switch (magicStats)
                {
                    case MagicStats.None:
                        Debug.Log("TurnOff");
                        await yeelightClient.TurnOff();
                        await UniTask.Delay(1000);
                        break;
                    case MagicStats.Bibbidi:
                    case MagicStats.Bobbidi:
                        Debug.Log("BibbidiBobbidi");
                        await yeelightClient.BibbidiBobbidi();
                        break;
                    case MagicStats.Boo:
                        Debug.Log("Boo");
                        await yeelightClient.Boo();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Yeelightとの接続に問題が発生しました、再接続にトライします。{e}");
                
                // 少し間をおいてから接続に再挑戦する
                await UniTask.Delay(500);

                yeelightClient = new YeelightClient();

                await UniTask.Delay(500);
            }
        }
    }

    public async void OnClicked()
    {
        await yeelightClient.TurnOff();
    }
    
    public async void OnClicked1()
    {
        await yeelightClient.BibbidiBobbidi();
    }
    
    public async void OnClicked2()
    {
        await yeelightClient.TurnOn();
    }
}
