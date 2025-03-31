using UnityEngine;
using UnityEngine.UI;
using RyapUnity.Network;

public class MagicStick : MonoBehaviour
{
    [SerializeField] private UDPReceiver udpReceiver;
    
    [SerializeField] private Text smallCircleText;
    [SerializeField] private Text bigShakeText;

    private int smallCircle = 0;
    private int bigShake = 0;
    
    private YeelightClient yeelightClient;

    void Start()
    {
        yeelightClient = new YeelightClient();
    }

    private async void OnApplicationQuit()
    {
        await yeelightClient.TurnOff();
    }

    async void Update()
    {
        // move small circle (Bibbidi-Bobbidi-)
        if (udpReceiver.AccData[0] > 1.5 ||
            udpReceiver.AccData[1] > 1.5 ||
            udpReceiver.AccData[2] > 1.5)
        {
            smallCircle++;
        }
        // move big shake (Boo)
        if (udpReceiver.AccData[0] > 5 ||
            udpReceiver.AccData[1] > 5 ||
            udpReceiver.AccData[2] > 5)
        {
            bigShake++;
        }
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (smallCircle > 20 && bigShake > 1)
        {
            await yeelightClient.TurnOn();
            smallCircle = 0;
            bigShake = 0;
        }

        smallCircleText.text = $"{smallCircle}";
        bigShakeText.text = $"{bigShake}";
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
