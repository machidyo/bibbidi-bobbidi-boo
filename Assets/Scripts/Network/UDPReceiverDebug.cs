using UnityEngine;
using UnityEngine.UI;
using RyapUnity.Network;

public class UDPReceiverDebug : MonoBehaviour
{
    [SerializeField] private Text status;
    [SerializeField] private Text acc;
    [SerializeField] private Text gyro;
    [SerializeField] private Text angle;

    private UDPReceiver udpReceiver;

    void Start()
    {
        udpReceiver = GetComponent<UDPReceiver>();
    }
    
    void Update()
    {
        CheckM5StickCAButton();

        if (status == null || acc == null || gyro == null || angle == null)
        {
            Debug.LogError("Text components are not assigned.");
            return;
        }
        
        status.text = $"{udpReceiver.Status}";
        acc.text = $"{udpReceiver.AccData[0]:F2}, {udpReceiver.AccData[1]:F2}, {udpReceiver.AccData[2]:F2}";
        gyro.text = $"{udpReceiver.GyroData[0]:F2}, {udpReceiver.GyroData[1]:F2}, {udpReceiver.GyroData[2]:F2}";
        var agl = new Quaternion(
            udpReceiver.AhrsData[0], 
            udpReceiver.AhrsData[1], 
            udpReceiver.AhrsData[2],
            udpReceiver.AhrsData[3]).eulerAngles;
        angle.text = $"{agl.x:F2}, {agl.y:F2}, {agl.z:F2}";
    }

    private async void CheckM5StickCAButton()
    {
        if (udpReceiver.IsButtonAClicked)
        {
            var yeelightClient = new YeelightClient();
            await yeelightClient.TurnOff();
            udpReceiver.IsButtonAClicked = false;
        }
    }
}
