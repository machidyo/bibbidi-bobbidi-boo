using RyapUnity.Network;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MagicStick : MonoBehaviour
{
    [SerializeField] private Text smallCircleText;
    [SerializeField] private Text bigShakeText;
    
    [SerializeField] private Text accX;
    [SerializeField] private Text accY;
    [SerializeField] private Text accZ;
    
    private UDPReceiver udpReceiver;
    
    private bool isOn = true;

    private int smallCircle = 0;
    private int bigShake = 0;

    void Start()
    {
        udpReceiver = FindObjectOfType<UDPReceiver>();
    }

    void Update()
    {
        CheckM5StickCAButton();

        // move small circle (Bibbidi-Bobbidi-)
        if (udpReceiver.AccData[0] > 1 ||
            udpReceiver.AccData[1] > 1 ||
            udpReceiver.AccData[2] > 1)
        {
            smallCircle++;
        }
        // move big shake (Boo)
        if (udpReceiver.AccData[0] > 2 ||
            udpReceiver.AccData[1] > 2 ||
            udpReceiver.AccData[2] > 2)
        {
            bigShake++;
        }
        // change the light color by Bibbidi-Bobbidi-Boo and reset
        if (smallCircle > 20 && bigShake > 1)
        {
            Yeelight.SwitchLight(true);
            smallCircle = 0;
            bigShake = 0;
        }

        accX.text = $"{udpReceiver.AccData[0]}";
        accY.text = $"{udpReceiver.AccData[1]}";
        accZ.text = $"{udpReceiver.AccData[2]}";
        smallCircleText.text = $"{smallCircle}";
        bigShakeText.text = $"{bigShake}";
    }

    /// <summary>
    /// for debug, check to connect M5StickC and Yeelight.
    /// </summary>
    private void CheckM5StickCAButton()
    {
        if (udpReceiver.IsButtonAClicked)
        {
            OnClicked();
            udpReceiver.IsButtonAClicked = false;
        }
    }

    public void OnClicked()
    {
        isOn = !isOn;
        Yeelight.SwitchLight(isOn);
    }
}
