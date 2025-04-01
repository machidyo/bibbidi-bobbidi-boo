using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using RyapUnity.Network;

public class MagicStick : MonoBehaviour
{
    private enum MagicStats
    {
        None,
        Bibbidi,
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

    void Start()
    {
        yeelightClient = new YeelightClient();
        SwitchYeelightRepeatedlyByMagic().Forget();
    }

    private async void OnApplicationQuit()
    {
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
        if (udpReceiver.AccData[0] >= 5 ||
            udpReceiver.AccData[1] >= 5 ||
            udpReceiver.AccData[2] >= 5)
        {
            if (bibbidiChargeTime < 0.5)
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
        else if (udpReceiver.AccData[0] >= 1.5 ||
                 udpReceiver.AccData[1] >= 1.5 ||
                 udpReceiver.AccData[2] >= 1.5)
        {
            // Debug.Log("Bibbidi");
            bibbidiChargeTime += Time.deltaTime;
            bibbidiStoppingTime = 0;
        }
        else
        {
            // Debug.Log($"No magic, {bibbidiStoppingTime}");
            bibbidiStoppingTime += Time.deltaTime;
            if (bibbidiStoppingTime > 0.6)
            {
                bibbidiChargeTime = 0;
            }
        }

        bigShakeText.text = $"{bibbidiChargeTime:F2}, {bibbidiStoppingTime:F2}, {booCharge}";
        
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (bibbidiChargeTime >= 0.5 && booCharge >= 1)
        {
            magicStats = MagicStats.Boo;

            bibbidiChargeTime = 0;
            bibbidiStoppingTime = 0;
            booCharge = 0;
        }
        else if (bibbidiChargeTime > 0.0)
        {
            magicStats = MagicStats.Bibbidi;
        }
        else
        {
            magicStats = MagicStats.None;
            
            bibbidiChargeTime = 0;
            booCharge = 0;
        }
        
        smallCircleText.text = $"{magicStats}";
    }

    private async UniTask SwitchYeelightRepeatedlyByMagic()
    {
        while (true)
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
                        Debug.Log("TurnOnALittle");
                        await yeelightClient.TurnOnALittle();
                        break;
                    case MagicStats.Boo:
                        Debug.Log("TurnOn");
                        await yeelightClient.TurnOn();
                        await UniTask.Delay(3000);
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
        await yeelightClient.TurnOnALittle();
    }
    
    public async void OnClicked2()
    {
        await yeelightClient.TurnOn();
    }
}
