using System;
using UnityEngine;
using UnityEngine.UI;

public class YeelightClientDebug : MonoBehaviour
{
    [SerializeField] private MagicStick magicStick;
    
    [SerializeField] private Image background;
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject manualOperationPanel;

    void Update()
    {
        statusText.text = $"{magicStick.YeelightStatus}";

        background.color = magicStick.YeelightStatus switch
        {
            MagicStick.YeelightStats.Connecting => Constants.YELLOW,
            MagicStick.YeelightStats.Connected => Constants.GREEN,
            MagicStick.YeelightStats.Error => Constants.RED,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    // REVISIT:
    // 1. 基本的には死活監視しているので自動で回復する。
    // 2. ただUnityの実行と停止、あるいはUnity自体を再起動しないといけない原因不明のケースがある。
    // 以上の2点から、手動でのRecconectは実質アプリの再起動になりそうなので、手動でのRecconenctはやめた。
    public void OnManualOperationToggleSwitched(Toggle toggle)
    {
        manualOperationPanel.SetActive(toggle.isOn);
    }

    public async void OnReconnectedButtonClicked()
    {
        Debug.Log("OnReconnectedButtonClicked");
        await magicStick.Reconnect();
    }
}
