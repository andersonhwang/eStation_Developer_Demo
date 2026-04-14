# eStation Developer Edition
Welcome to eStation Developer Edition! 

eStation is designed for developers to quick integerate ETAG ESL&DSL with their projects. eStation use MQTT protocol and esay to configure/integerate.

Release Date: 2026-04-13

Firmware: 1.1.44

# 1. Work with Mosquitto

If you are working with Mosquitton, you need edit the Mosquitto configure file mosquitto.conf:
、、、
allow_anonymous false  
password_file Your_Password_File_Path_Here  # Pasword file path
listener XXXX                               # The MQTT port
max_topic_alias 255                         # The default value is 10, need change to 255

# 2. Python
> Start file: demo.py
```Python
# Demo parameters - You can modify these parameters for testing
token = random.randint(1, 0xFFFF)           # Init token
tag_id = "82000088A2C8"                     # Test tag ID  - 4 colors
tag_id2 = "A00000CA8033"                    # Test tag ID - 6 colors
test_image = "T1.bmp"                       # Test image - 4 colors
test_image2 = "T2.bmp"                      # Test image - 6 colors

# AP Config parameters - Modify these parameters according to your network environment
alias = "09"                                # Alias
server = "192.168.4.74:9071"                # MQTT server
userName = "test"                           # Username
password = "123456"                         # Password
encrypt = False                             # Encryption
autoIP = False                              # Auto IP
localIP = "192.168.4.101"                   # Local IP
subnetMask = "255.255.255.0"                # Subnet Mask
gateway = "192.168.4.1"                     # Gateway
heartbeat = 60                              # Heartbeat
```

> Run demo.py
```Python
python3 demo.py
```

# 3. Understand the E-ink screen
...
