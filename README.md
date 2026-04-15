# eStation Developer Edition
Welcome to eStation Developer Edition! 

eStation is designed for developers to quick integerate ETAG ESL&DSL with their projects. eStation use MQTT protocol and esay to configure/integerate.

Release Date: 2026-04-13

Firmware: 1.1.44

# 1. Work with Mosquitto

If you are working with Mosquitton, you need edit the Mosquitto configure file mosquitto.conf:
```
allow_anonymous false  
password_file Your_Password_File_Path_Here  # Pasword file path
listener XXXX                               # The MQTT port
max_topic_alias 255                         # The default value is 10, need change to 255
```

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

# 3. Java

The Java demo is a copy of Python demo, convert by CodeX.

> Start file: DemoApp.java
```Java
    // Demo parameters - these can be modified as needed for testing. 
    // The tag IDs should correspond to actual tags in your environment if you want to see the updates reflected on real devices. 
    // The image paths should point to valid image files on your system.
    private static final String TAG_ID = "82000088A2C8";
    private static final String TAG_ID_2 = "A00000CA8033";
    private static final String TAG_ID_3 = "810000AB114B";

    private static final Path TEST_IMAGE = Path.of("src/main/java/com/estation/demo/images/T1.bmp");
    private static final Path TEST_IMAGE_2 = Path.of("src/main/java/com/estation/demo/images/T2.bmp");
    private static final Path TEST_IMAGE_3_A = Path.of("src/main/java/com/estation/demo/images/T3A.bmp");
    private static final Path TEST_IMAGE_3_B = Path.of("src/main/java/com/estation/demo/images/T3B.bmp");

    // ApConfiguration parameters - adjust these to match your MQTT broker and network settings.
    private static final String ALIAS = "09";
    private static final String SERVER = "192.168.4.74:9071";
    private static final String USER_NAME = "test";
    private static final String PASSWORD = "123456";
    private static final boolean ENCRYPT = false;
    private static final boolean AUTO_IP = false;
    private static final String LOCAL_IP = "192.168.4.101";
    private static final String SUBNET_MASK = "255.255.255.0";
    private static final String GATEWAY = "192.168.4.1";
    private static final int HEARTBEAT = 60;
```

> Run Demo
```Java
java -jar target/estation-demo-1.0.0.jar
```
