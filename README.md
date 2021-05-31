# bibbidi-bobbidi-boo
魔法の杖を授けます。

https://user-images.githubusercontent.com/1772636/120218272-72072d80-c274-11eb-80d1-2a8615c4c9fa.mp4

# 構成
M5StickC <-- wifi --> PC(Unity) -- kick --> PC(Python) <-- wifi --> Yeelight

## 詳細
* M5StickC
  * [Ryap](https://github.com/machidyo/RyapY) をインストールして、wifi 経由で本リポジトリのUnity部分とやりとりします。
* Yeelight 
  * [python-yeelight](https://gitlab.com/stavros/python-yeelight) を下地に、wifi 経由で本リポジトリの python プログラムで操作します。
* Unity 2021.1.1f1
* PC と wifi 2.4GHz(Yeelight の仕様）
