using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using R3;
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

    private ReactiveProperty<MagicStats> magicStats = new (MagicStats.None);
    
    private float bibbidiChargeTime = 0;
    private float bibbidiStoppingTime = 0;
    private int booCharge = 0;
    
    private YeelightClient yeelightClient;

    private CompositeDisposable disposable = new();

    void Start()
    {
        yeelightClient = new YeelightClient();
        
        var isRunning = false;
        magicStats.DistinctUntilChanged()
            .Where(_ => !isRunning)
            .Subscribe( async stats =>
            {
                try
                {
                    isRunning = true;
                    switch (stats)
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
                            await UniTask.Delay(1000);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(stats), stats, null);
                    }
                }
                catch (Exception e)
                {
                    // エラーは投げ捨てる
                }
                finally
                {
                    isRunning = false;
                }
            }).AddTo(disposable);
    }
    
    private void OnDestroy()
    {
        disposable.Dispose();
    }

    private async void OnApplicationQuit()
    {
        await yeelightClient.TurnOff();
    }

    void Update()
    {
        // big shake (to Boo or fault)
        if (udpReceiver.AccData[0] >= 5 ||
            udpReceiver.AccData[1] >= 5 ||
            udpReceiver.AccData[2] >= 5)
        {
            if (bibbidiChargeTime < 2.0)
            {
                Debug.Log("fault: Boo but you have to do bibbidi before");
                bibbidiChargeTime = 0;
            }
            else
            {
                // Debug.Log("Boo");
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
            if (bibbidiStoppingTime > 1.0)
            {
                bibbidiChargeTime = 0;
            }
        }

        bigShakeText.text = $"{bibbidiChargeTime:F2}, {bibbidiStoppingTime:F2}, {booCharge}";
        
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (bibbidiChargeTime >= 2.0 && booCharge >= 1)
        {
            magicStats.Value = MagicStats.Boo;

            bibbidiChargeTime = 0;
            bibbidiStoppingTime = 0;
            booCharge = 0;
        }
        else if (bibbidiChargeTime > 0.0)
        {
            magicStats.Value = MagicStats.Bibbidi;
        }
        else
        {
            magicStats.Value = MagicStats.None;
            
            bibbidiChargeTime = 0;
            booCharge = 0;
        }
        
        smallCircleText.text = $"{magicStats}";
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
