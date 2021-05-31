import sys
from yeelight import Bulb

bulb = Bulb("192.168.10.53")
arg = int(sys.argv[1])

bulb.turn_off()
bulb.turn_on()

if arg == 0:
  print("off")
  bulb.turn_off()
elif arg == 1:
  print("yellow green")
  bulb.set_rgb(255, 255, 0)
elif arg == 2:
  print("purple")
  bulb.set_rgb(255, 0, 255)
elif arg == 3:
  print("sky blue")
  bulb.set_rgb(0, 255, 255)
elif arg == 4:
  print("red")
  bulb.set_rgb(255, 0, 0)
elif arg == 5:
  print("green")
  bulb.set_rgb(0, 255, 0)
elif arg == 6:
  print("blue")
  bulb.set_rgb(0, 0, 255)
else:
  print("white")
  bulb.set_rgb(255, 255, 255)
