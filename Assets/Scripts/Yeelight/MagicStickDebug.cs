using System;
using UnityEngine;
using UnityEngine.UI;

public class MagicStickDebug : MonoBehaviour
{
    [SerializeField] private MagicStick magicStick;
    
    [SerializeField] private Image background;
    [SerializeField] private Text smallCircleText;

    void Update()
    {
        smallCircleText.text = $"{magicStick.MagicStatus}";

        background.color = magicStick.MagicStatus switch
        {
            MagicStick.MagicStats.None => Color.gray,
            MagicStick.MagicStats.Bibbidi => Color.yellow,
            MagicStick.MagicStats.Bobbidi => Color.cyan,
            MagicStick.MagicStats.Boo => Color.green,
            _ => throw new ArgumentOutOfRangeException()
        };
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
