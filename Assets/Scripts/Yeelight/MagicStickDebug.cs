using System;
using UnityEngine;
using UnityEngine.UI;

public class MagicStickDebug : MonoBehaviour
{
    [SerializeField] private MagicStick magicStick;
    
    [SerializeField] private Image background;
    [SerializeField] private Text smallCircleText;
    [SerializeField] private GameObject manualOperationPanel;

    void Update()
    {
        smallCircleText.text = $"{magicStick.MagicStatus}";

        background.color = magicStick.MagicStatus switch
        {
            MagicStick.MagicStats.None => Constants.GRAYLISH_LIGHT_YELLOW,
            MagicStick.MagicStats.Bibbidi => Constants.YELLOW,
            MagicStick.MagicStats.Bobbidi => Constants.YELLOW_GREEN,
            MagicStick.MagicStats.Boo => Constants.GREEN,
            MagicStick.MagicStats.Debug => Constants.MAGENTA,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public void OnManualOperationToggleSwitched(Toggle toggle)
    {
        magicStick.SwitchDebugMode(toggle.isOn);
        manualOperationPanel.SetActive(toggle.isOn);
    }

    public async void OnClicked1()
    {
        await magicStick.TurnOff();
    }
    
    public async void OnClicked2()
    {
        await magicStick.BibbidiBobbidi();
    }
    
    public async void OnClicked3()
    {
        await magicStick.Boo();
    }
}
