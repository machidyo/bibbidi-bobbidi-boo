# bibbidi-bobbidi-boo
魔法の杖を授けます。

https://user-images.githubusercontent.com/1772636/120218272-72072d80-c274-11eb-80d1-2a8615c4c9fa.mp4

# 構成
`M5StickC` <-- wifi --> `PC(Unity)` -- run python --> `PC(Python)` <-- wifi --> `Yeelight`

# 必要なモノ
* 魔法を込める杖
* M5StickC
  * [Ryap](https://github.com/machidyo/RyapY) をインストールして、wifi 経由で本リポジトリのUnity部分とやりとりします。
* [Yeelight](https://en.yeelight.com/product/819.html) 
  * 動画は[YLDP13YL](https://www.amazon.co.jp/dp/B086LPJHMT)を使っています
  * [python-yeelight](https://gitlab.com/stavros/python-yeelight) を下地に、wifi 経由で本リポジトリの python プログラムで操作します。
* Unity 2021.1.1f1
* PC と wifi 2.4GHz(Yeelight の仕様）

# リポジトリ
## フォルダ構成
* 基本的に Unity 用のリポジトリです
* この `PC(Python) <-- wifi --> Yeelight` 部分を実現するためのプログラムが /Python/Yeelihg.py になっています

## 設定変更箇所
### Python側
* Yeelight に接続するための[IP](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Python/Yeelight.py#L4)
### Unity側
* Python 実行するための[環境変数と実行ファイルのパス](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Assets/Scripts/Yeelight/PythonRunner.cs#L7-L8)

# 補足
## Yeelight の IP の確認方法
```
>>> import yeelight
>>> yeelight.discover_bulbs()
[{'ip': '192.168.10.23', 'port': 55443, 'capabilities': {'id': '0x0000000013f1fee1', 'model': 'color4', 'fw_ver': '24', 'support': 'get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music', 'power': 'on', 'bright': '79', 'color_mode': '2', 'ct': '4244', 'rgb': '255', 'hue': '240', 'sat': '100', 'name': ''}}]
```
### 実行結果が `[]` になってしまう場合
* Windows Firewall の設定をオフにしてみる
* 複数ネットワークに接続している場合は wifi だけにしてみる
