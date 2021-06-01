# bibbidi-bobbidi-boo
Gives the magic wand.

https://user-images.githubusercontent.com/1772636/120218272-72072d80-c274-11eb-80d1-2a8615c4c9fa.mp4

日本語は[こちら](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/README.jp.md)

# Connection image
`M5StickC` <-- wifi --> `PC(Unity)` -- run python --> `PC(Python)` <-- wifi --> `Yeelight`

# What you need
* A wand with magic
* M5StickC
  * Install [Ryap](https://github.com/machidyo/RyapY) and interact with the Unity part of this repository via wifi.
* [Yeelight](https://en.yeelight.com/product/819.html) 
  * The video uses [YLDP13YL](https://www.amazon.co.jp/dp/B086LPJHMT).
  * Based on [python-yeelight](https://gitlab.com/stavros/python-yeelight), operate with the python program of this repository via wifi.
* Unity 2021.1.1f1
* PC and wifi 2.4GHz(Yeelight specifications)

# Repository
## Folder structure
* It's basically a repository for Unity
* The program to realize this `PC (Python) <-wifi-> Yeelight` part is /Python/Yeelihg.py.

## Settings
### Python
* [IP](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Python/Yeelight.py#L5) for connecting to Yeelight 
### Unity
* [Environment variables and executable path](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Assets/Scripts/Yeelight/PythonRunner.cs#L7-L10) for executing Python 

# Supplement
## How to check Yeelight's IP
```
>>> import yeelight
>>> yeelight.discover_bulbs()
[{'ip': '192.168.10.23', 'port': 55443, 'capabilities': {'id': '0x0000000013f1fee1', 'model': 'color4', 'fw_ver': '24', 'support': 'get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music', 'power': 'on', 'bright': '79', 'color_mode': '2', 'ct': '4244', 'rgb': '255', 'hue': '240', 'sat': '100', 'name': ''}}]
```
### When the execution result is `[]`
* Try turning off the Windows Firewall setting
* If you are connected to multiple networks, try only wifi
