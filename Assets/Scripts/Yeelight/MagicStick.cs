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
    
    public enum MagicStats
    {
        None,
        Bibbidi,
        Bobbidi,
        Boo
    }
    
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private UDPReceiver udpReceiver;
    
    [Header("DEBUG")]
    [SerializeField] private Text bigShakeText;

    public MagicStats MagicStatus { get; private set; } = MagicStats.None;
    
    private float bibbidiChargeTime = 0;
    private float bibbidiStoppingTime = 0;
    private int booCharge = 0;
    private float booTime = 0;
    
    private YeelightClient yeelightClient;
    private CancellationTokenSource yeelightCancellationToken;

    void Start()
    {
        yeelightClient = new YeelightClient();
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

    private async UniTask SwitchYeelightRepeatedlyByMagic(CancellationTokenSource tokenSource)
    {
        while (!tokenSource.IsCancellationRequested)
        {
            try
            {
                switch (MagicStatus)
                {
                    case MagicStats.None:
                        Debug.Log("TurnOff");
                        await yeelightClient.TurnOff();
                        await UniTask.Delay(1000);
                        break;
                    case MagicStats.Bibbidi:
                    case MagicStats.Bobbidi:
                        Debug.Log("BibbidiBobbidi");
                        soundManager.PlaySound(SoundManager.SoundNames.BibbidiBobbidi);
                        await yeelightClient.BibbidiBobbidi();
                        break;
                    case MagicStats.Boo:
                        Debug.Log("Boo");
                        soundManager.PlaySound(SoundManager.SoundNames.Boo);
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
