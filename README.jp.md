# bibbidi-bobbidi-boo
魔法の杖を授けます。

https://user-images.githubusercontent.com/1772636/120218272-72072d80-c274-11eb-80d1-2a8615c4c9fa.mp4

# 構成
`M5StickC` <-- wifi --> `PC(Unity)` <-- wifi --> `Yeelight`

# 必要なモノ
* 魔法を込める杖
* M5StickC
  * [Ryap](https://github.com/machidyo/RyapY) をインストールして、wifi 経由で本リポジトリのUnity部分とやりとりします
* [Yeelight](https://en.yeelight.com/product/819.html) 
  * 動画は[YLDP13YL](https://www.amazon.co.jp/dp/B086LPJHMT)を使っています
  * [YeelightAPI](https://github.com/roddone/YeelightAPI) を使用して、wifi 経由で操作します
* Unity 2021.3.37f1
* PC と wifi 2.4GHz(Yeelight の仕様）

# リポジトリ
## 設定変更箇所
* [YeelightAPIの接続先](https://github.com/machidyo/bibbidi-bobbidi-boo/blob/master/Assets/Scripts/Yeelight/YeelightClient.cs#L10)

# 補足
## Yeelight の IP の確認方法
### pythonで確認する方法
python3で、pip3 install yeelightして必要なライブラリを入れておいてください。

```
>>> import yeelight
>>> yeelight.discover_bulbs()
[{'ip': '192.168.10.23', 'port': 55443, 'capabilities': {'id': '0x0000000013f1fee1', 'model': 'color4', 'fw_ver': '24', 'support': 'get_prop set_default set_power toggle set_bright set_scene cron_add cron_get cron_del start_cf stop_cf set_ct_abx adjust_ct set_name set_adjust adjust_bright adjust_color set_rgb set_hsv set_music', 'power': 'on', 'bright': '79', 'color_mode': '2', 'ct': '4244', 'rgb': '255', 'hue': '240', 'sat': '100', 'name': ''}}]
```

実行結果が `[]` になってしまう場合
* Windows Firewall の設定をオフにしてみる
* 複数ネットワークに接続している場合は wifi だけにしてみる

### アプリで確認する方法
1. 確認したい電球を選択
2. 右上の設定を選択
3. デバイス情報を選択すると、IPが表示されます
