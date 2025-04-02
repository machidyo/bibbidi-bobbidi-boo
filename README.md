# bibbidi-bobbidi-boo
Gives the magic wand.

https://user-images.githubusercontent.com/1772636/120218272-72072d80-c274-11eb-80d1-2a8615c4c9fa.mp4

日本語は[こちら](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/README.jp.md)

# Connection image
`M5StickC` <-- wifi --> `PC(Unity)` <-- wifi --> `Yeelight`

# What you need
* A wand with magic
* M5StickC
  * Install [Ryap](https://github.com/machidyo/RyapY) and interact with the Unity part of this repository via wifi.
* [Yeelight](https://en.yeelight.com/product/819.html) 
  * The video uses [YLDP13YL](https://www.amazon.co.jp/dp/B086LPJHMT).
  * Operated via Wi-Fi using the [YeelightAPI](https://github.com/roddone/YeelightAPI).
* Unity 2021.3.37f1
* PC and wifi 2.4GHz(Yeelight specifications)

# Repository
## Configration Settings
* [YeelightAPI Conenction Details](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Assets/Scripts/Yeelight/YeelightClient.cs#L10) 

# Supplement
## How to Verify with Python
Python3 and the [python-yeelight](https://github.com/skorokithakis/python-yeelight) library are required. If you haven't already installed them.

```
>>> import yeelight
>>> yeelight.discover_bulbs()
[{'ip': '192.168.10.23', 'port': 55443, 'capabilities': {'id': '0x0000000013f1fee1', 'model': 'color4', 'fw_ver': '24', 'support': 'get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music', 'power': 'on', 'bright': '79', 'color_mode': '2', 'ct': '4244', 'rgb': '255', 'hue': '240', 'sat': '100', 'name': ''}}]
```

When the execution result is `[]`
* Try turning off the Windows Firewall setting
* If you are connected to multiple networks, try only wifi

## How to Verify with the App
1. Select the bulb you want to check.
2. Tap the settings icon in the upper right corner.
3. Select "Device info" (or similar). The IP address will be displayed.
