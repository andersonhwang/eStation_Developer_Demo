import paho.mqtt.client as mqtt
import time
import array
import base64
import random
import msgpack
import gzip
import hashlib

from pathlib import Path
from PIL import Image

from Entities.apOta import OTAData
from Entities.eStationConfig import eStationConfig
from Entities.eStationInfor import eStationInfor
from Entities.eStationMessage import eStationMessage
from Entities.eslEntity import ESLEntity
from Entities.eslEntity2 import ESLEntity2
from Entities.taskResult import TaskResult
from Entities.apHeartbeat import ApHeartbeat

from Enums.pageIndex import PageIndexes
from Enums.pattern import Patterns
from Entities.apHeartbeat import ApHeartbeat
import appConfig    # Your application configuration
import fileHelper

# Demo parameters - You can modify these parameters for testing
token = random.randint(1, 0xFFFF)           # Init token
tag_id = "82000088A2C8"                     # Test tag ID  - 4 colors
tag_id2 = "A00000CA8033"                    # Test tag ID - 6 colors
tag_id3= "810000AB114B"                    # Test tag ID - 2.13inch, 250*122
test_image = "T1.bmp"                       # Test image - 4 colors
test_image2 = "T2.bmp"                      # Test image - 6 colors
test_image3A = "T3A.bmp"                    # Test image - BGRA32
test_image3B = "T3B.bmp"                    # Test image - BGR24

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


# Callback when the client receives a CONNACK response from the server
def on_connect(client, userdata, flags, reason_code, properties):
    print(f"Connected to MQTT broker with result code {reason_code}")
    client.subscribe(appConfig.TOPIC_INFOR)
    client.subscribe(appConfig.TOPIC_RESULT)
    client.subscribe(appConfig.TOPIC_HEARTBEAT)
    client.subscribe(appConfig.TOPIC_MESSAGE)

# Callback when the client disconnects from the broker
def on_disconnect(client, userdata, reason_code, properties):
    print(f"Disconnected with result code {reason_code}")
    client.reconnect()

# Callback when a PUBLISH message is received from the server
def on_message(client, userdata, msg):
    print(f"[Recv] Topic: {msg.topic}")
    match msg.topic:
        case appConfig.TOPIC_INFOR:
            infor = eStationInfor.from_msgpack(msg.payload)
            print(f"eStation Infor: {infor}")
            return
        case appConfig.TOPIC_RESULT:
            result = TaskResult.from_msgpack(msg.payload)
            print(f"eStation Result: {result}")
            return
        case appConfig.TOPIC_HEARTBEAT:
            # If you want to display heartbeat info, you can uncomment the following lines. Note that heartbeat messages are sent every 60 seconds by default, so it may flood your console if you print every heartbeat.
            # heartbeat = ApHeartbeat.from_msgpack(msg.payload)
            # print(f"eStation Heartbeat: {heartbeat}")
            return
        case appConfig.TOPIC_MESSAGE:
            message = eStationMessage.from_msgpack(msg.payload)
            print(f"eStation Message: {message}")
            return
        case _:
            return

# Function to get the next token
def get_token():
    global token
    token += 1
    if token >= 0xFFFF:
        token = 0
    return token

# Read image and convert to BGRA32 byte array
def read_image_bgra(file_path):
    with Image.open(file_path) as img:
        img = img.convert("RGBA")  # RGBA

        width, height = img.size
        rgba = img.tobytes()

        # 转 BGRA
        bgra = bytearray(len(rgba))

        for i in range(0, len(rgba), 4):
            r = rgba[i]
            g = rgba[i + 1]
            b = rgba[i + 2]
            a = rgba[i + 3]

            bgra[i] = b
            bgra[i + 1] = g
            bgra[i + 2] = r
            bgra[i + 3] = a

        return bytes(bgra), (width, height)
    
# Read image and convert to BGR24 byte array 
def read_image_bgr(file_path):
    with Image.open(file_path) as img:
        img = img.convert("RGB")  # RGBA

        width, height = img.size
        rgb = img.tobytes()

        # 转 BGR
        bgr = bytearray(len(rgb))

        for i in range(0, len(rgb), 3):
            r = rgb[i]
            g = rgb[i + 1]
            b = rgb[i + 2]

            bgr[i] = b
            bgr[i + 1] = g
            bgr[i + 2] = r

        return bytes(bgr), (width, height)
    
# Function to configure AP
def publish_config(client, alias, server, userName, password, encrypt, autoIP, localIP, subnet, gateway, heartbeat):
    config = eStationConfig(
        Alias=alias,
        Server=server,
        ConnParam=[userName, password],
        Encrypt=encrypt,
        AutoIP=autoIP,
        LocalIP=localIP,
        Subnet=subnet,
        Gateway=gateway,
        Heartbeat=heartbeat
    )
    client.publish(appConfig.TOPIC_CONFIG, config.to_msgpack())

# Function to publish ESL message
def publish_esl(client, id, token, image, r, g, b):
    image_bytes = read_image_bgra(image)
    base64_str = base64.b64encode(image_bytes[0]).decode('utf-8')
    esl_list = [
        ESLEntity(
            TagID=id, 
            Token=token,
            Pattern=Patterns.UpdateDisplay.value, 
            PageIndex=PageIndexes.P0.value, 
            Base64String=base64_str,
            R=r, 
            G=g, 
            B=b
        )
    ]
    data = msgpack.packb([
        [
            e.TagID,
            e.Pattern,
            e.PageIndex,
            e.R,
            e.G,
            e.B,
            e.Times,
            e.Token,
            e.CurrentKey,
            e.NewKey,
            e.Base64String
        ] for e in esl_list
    ])
    client.publish(appConfig.TOPIC_TASK_ESL, data)

# Function to publish ESL2 message
# Topic taskESL2 supports BGRA32, BGR24 and file bytes. 
# You can choose the data format by commenting/uncommenting the corresponding lines in the function. 
def publish_esl2(client, id, token, image, r, g, b):
    #0. BGRA32 data
    image_bytes = read_image_bgra(image)
    #1. BGR24 data
    image_bytes = read_image_bgr(image)
    #2. File bytes
    # image_bytes = open(image, 'rb').read(),
    esl_list = [
        ESLEntity2(
            TagID=id, 
            Token=token,
            Pattern=Patterns.UpdateDisplay.value, 
            PageIndex=PageIndexes.P0.value, 
            Bytes=gzip.compress(image_bytes[0]),  # Compress the image bytes
            Compress=True,  # Compress is true
            R=r, 
            G=g, 
            B=b
        )
    ]
    data = msgpack.packb([
        [
            e.TagID,
            e.Pattern,
            e.PageIndex,
            e.R,
            e.G,
            e.B,
            e.Times,
            e.Token,
            e.CurrentKey,
            e.NewKey,
            e.Bytes,
            e.Compress
        ] for e in esl_list
    ])
    client.publish(appConfig.TOPIC_TASK_ESL2, data)

# Function to publish OTA message
def publish_ota(client, path, version, downloadUrl, confirmUrl):
    p = Path(path)
    ota = OTAData(
        download_url=downloadUrl,
        confirm_url=confirmUrl,
        type=0,
        version=version,
        name=p.name,
        md5= str.upper(calc_md5(path))
    )
    client.publish(appConfig.TOPIC_FIRMWARE, ota.to_msgpack())

# Function to calculate MD5 checksum of a file
def calc_md5(file_path: str) -> str:
    md5 = hashlib.md5()
    with open(file_path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            md5.update(chunk)
    return md5.hexdigest()

# Main function
def main():
    print("Starting eStation Demo(Python)...")
    print("0: Publish Config")
    print("1: Publish ESL")
    print("2: Publish ESL2")
    print("3: Publish DSL(TODO)")
    print("4: Publish DSL2(TODO)")
    print("5: Publish ESL OTA(TODO)")
    print("6: Publish Firmware OTA")
    print("Press 'E' to exit.")

    # Configure your client connection parameters in appConfig.py
    client = mqtt.Client(
        client_id="test_server",
        protocol=mqtt.MQTTv5,
        reconnect_on_failure=True
    )
    client.username_pw_set(appConfig.USER_NAME, appConfig.PASSWORD)
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_disconnect = on_disconnect

    client.connect(appConfig.BROKER, appConfig.PORT, 60)

    # Start the loop in a separate thread
    client.loop_start()

    ota_download_url = "http://192.168.4.74:9070/ota/2/eStation2.1.0.44.OTA.tar?id={0}&time={1}"
    ota_confirm_url = "http://192.168.4.74:9070/confirm?id={0}&time={1}"

    try:
        while True:
            try:
                user_input = input("Enter code to send: ")    
                if user_input.upper() == "E":
                    return
                code = int(user_input)
                match code:
                    case 0:
                        publish_config(client, alias, server, userName, password, encrypt, autoIP, localIP, subnetMask, gateway, heartbeat)
                        continue
                    case 1:
                        publish_esl(client, tag_id, get_token(), test_image, True, False, False)
                        continue
                    case 2:
                        publish_esl2(client, tag_id2, get_token(), test_image2, False, True, False)
                        continue
                    case 3:
                        # TODO: Implement DSL publish function
                        continue
                    case 4:
                         # TODO: Implement DSL2 publish function
                         continue
                    case 5:
                        # TODO: Implement firmware publish function
                        continue
                    case 6:
                        publish_ota(
                            client, 
                            "/Users/andersonh/Documents/GitHub/eStation/OTA/AP_OTA/eStation2.1.0.44.OTA.tar", 
                            "1.0.44", 
                            str.format(ota_download_url, "0001", int(time.time())),
                            str.format(ota_confirm_url, "0001", int(time.time()))
                        )
                        continue
                    case _:
                        print("Unknown code.")
                        break
            except ValueError:
                print("Invalid input. Please enter a valid code.")
                continue
            except KeyboardInterrupt:
                print("Exiting...")
                break
    finally:
        client.loop_stop()
        client.disconnect()

if __name__ == "__main__":
    main()