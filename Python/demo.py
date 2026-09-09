import random

import paho.mqtt.client as mqtt
import time
import base64
import json
import ssl
import msgpack
import gzip
import hashlib
import urllib.request

from pathlib import Path
from PIL import Image

from Entities.apOta import OTAData
from Entities.apSecurity import ApSecurity
from Entities.dslEntity import DSLEntity
from Entities.dslEntity2 import DSLEntity2
from Entities.dslEntity3 import DSLEntity3
from Entities.eStationConfig import eStationConfig
from Entities.eStationInfor import eStationInfor
from Entities.eStationMessage import eStationMessage
from Entities.eslEntity import ESLEntity
from Entities.eslEntity2 import ESLEntity2
from Entities.tagType import TagType
from Entities.taskResult import TaskResult
from Entities.apHeartbeat import ApHeartbeat

from Enums.pageIndex import PageIndexes
from Enums.pattern import Patterns
from Entities.apHeartbeat import ApHeartbeat
import demoConfig    # Your application configuration
import fileHelper

# Callback when the client receives a CONNACK response from the server
def on_connect(client, userdata, flags, reason_code, properties):
    print(f"Connected to MQTT broker with result code {reason_code}")
    client.subscribe(demoConfig.TOPIC_INFOR)
    client.subscribe(demoConfig.TOPIC_RESULT)
    client.subscribe(demoConfig.TOPIC_HEARTBEAT)
    client.subscribe(demoConfig.TOPIC_MESSAGE)

# Callback when the client disconnects from the broker
def on_disconnect(client, userdata, reason_code, properties):
    print(f"Disconnected with result code {reason_code}")
    client.reconnect()

# Callback when a PUBLISH message is received from the server
def on_message(client, userdata, msg):
    print(f"[Recv] Topic: {msg.topic}")
    match msg.topic:
        case demoConfig.TOPIC_INFOR:
            infor = eStationInfor.from_msgpack(msg.payload)
            print(f"eStation Infor: {infor}")
            return
        case demoConfig.TOPIC_RESULT:
            result = TaskResult.from_msgpack(msg.payload)
            print(f"eStation Result: {result}")
            return
        case demoConfig.TOPIC_HEARTBEAT:
            # If you want to display heartbeat info, you can uncomment the following lines. Note that heartbeat messages are sent every 60 seconds by default, so it may flood your console if you print every heartbeat.
            # heartbeat = ApHeartbeat.from_msgpack(msg.payload)
            # print(f"eStation Heartbeat: {heartbeat}")
            return
        case demoConfig.TOPIC_MESSAGE:
            message = eStationMessage.from_msgpack(msg.payload)
            print(f"eStation Message: {message}")
            return
        case _:
            return

# Function to get the next token
token = random.randint(1, 0xFFFF)           # Init token
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
    client.publish(demoConfig.TOPIC_CONFIG, config.to_msgpack())

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
    client.publish(demoConfig.TOPIC_TASK_ESL, data)

# Function to publish ESL2 message
# Topic taskESL2 supports BGRA32, BGR24 and file bytes. 
# You can choose the data format by commenting/uncommenting the corresponding lines in the function. 
def publish_esl2(client, id, token, image, r, g, b):
    #0. BGRA32 data
    image_bytes = read_image_bgra(image)
    #1. BGR24 data
    #image_bytes = read_image_bgr(image)
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
    client.publish(demoConfig.TOPIC_TASK_ESL2, data)
    
def publish_dsl(client, ids, token, bin, r, g, b):
    # 0. Read bin file
    with open(bin, 'rb') as f:
        bin_bytes = f.read()
        
    # 1. Prepare DSL entities list        
    dsl_list = []
    for id in ids:
        dsl_list.append(
            DSLEntity(
                TagID=id, 
                Token=token,
                HexData=bin_bytes,  
                R=r, 
                G=g, 
                B=b,
            )
        )

    # 2. MessagePack serialization
    data = msgpack.packb([
        [
            e.TagID,
            e.R,
            e.G,
            e.B,
            e.Period,
            e.Interval,
            e.Duration,
            e.Token,
            e.HexData
        ] for e in dsl_list
    ], use_bin_type=True)
    
    #3. Call client.publish to send data
    client.publish(demoConfig.TOPIC_TASK_DSL, data)    

def publish_dsl3(client, ids, token, bin, r, g, b):
    # 0. Decode the image and copy its pixels as contiguous RGB24 data.
    with Image.open(bin) as img:
        bin_bytes = img.convert("RGB").tobytes()
    print(f"[Debug] bin_bytes (hex): {bin_bytes.hex()}")
        
    # 1. Prepare DSL entities list        
    dsl_list = []
    for id in ids:
        dsl_list.append(
            DSLEntity3(
                TagID=id, 
                Token=token,
                HexData=bin_bytes,  
                R=r, 
                G=g, 
                B=b,
                Width=320,
                Height=240,
                Top=0,
                Left=0,
                IsGif=False,
                Pattern=Patterns.UpdateDisplay,
            )
        )

    # 2. MessagePack serialization
    data = msgpack.packb([
        [
            e.TagID,
            e.R,
            e.G,
            e.B,
            e.Period,
            e.Interval,
            e.Duration,
            e.Token,
            e.HexData,
            e.Pattern.value,
            e.CurrentKey,
            e.NewKey,
            e.IsGif,
            e.Width,
            e.Height,
            e.Top,
            e.Left
        ] for e in dsl_list
    ], use_bin_type=True)
    
    #3. Call client.publish to send data
    client.publish(demoConfig.TOPIC_TASK_DSL3, data)

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
    client.publish(demoConfig.TOPIC_FIRMWARE, ota.to_msgpack())

# Function to publish certificate message
def publish_certificate(client, certName, keyName, customTrustStore, extraStore, certPath, downloadUrl, confirmUrl):
    cert = ApSecurity(
        downloadUrl=downloadUrl,
        confirmUrl=confirmUrl,
        certName=certName,
        keyName=keyName,
        customTrustStore=customTrustStore,
        extraStore=extraStore,
        mD5=str.upper(calc_md5(certPath))
    )
    client.publish(demoConfig.TOPIC_CERT, cert.to_msgpack())
    
def _ssl_context():
    try:
        import certifi
        return ssl.create_default_context(cafile=certifi.where())
    except ImportError:
        return ssl.create_default_context()

def _to_raw_github_url(url: str) -> str:
    if "github.com" in url and "/blob/" in url:
        return url.replace("https://github.com/", "https://raw.githubusercontent.com/").replace("/blob/", "/")
    return url

# Download TagType.json from GitHub
def download_tag_type_json(url: str = demoConfig.TAG_TYPE_JSON_URL, dest: Path = demoConfig.TAG_TYPE_JSON_FILE) -> Path:
    raw_url = _to_raw_github_url(url)
    dest = Path(dest)
    dest.parent.mkdir(parents=True, exist_ok=True)
    request = urllib.request.Request(raw_url, headers={"User-Agent": "eStation-Developer-Demo"})
    with urllib.request.urlopen(request, context=_ssl_context()) as response, open(dest, "wb") as out:
        out.write(response.read())
    print(f"Downloaded TagType.json to {dest}")
    return dest

# Convert TagType.json to a TagType list
def load_tag_types(path: Path = demoConfig.TAG_TYPE_JSON_FILE):
    with open(path, "r", encoding="utf-8") as f:
        items = json.load(f)
    tag_types = TagType.list_from_json(items)
    print(f"Loaded {len(tag_types)} tag types from {path}")
    return tag_types

# Function to publish TagType list
def publish_tag_type(client, tag_types):
    client.publish(demoConfig.TOPIC_TAG_TYPE, TagType.list_to_msgpack(tag_types))

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
    print("3: Publish DSL")
    print("4: Publish DSL3")
    print("5: Publish ESL OTA(TODO)")
    print("6: Publish Firmware OTA")
    print("7: Publish Certificate")
    print("8: Publish Tag Type")
    print("Press 'E' to exit.")

    # Configure your client connection parameters in demoConfig.py
    client = mqtt.Client(
        client_id="test_server",
        protocol=mqtt.MQTTv5,
        reconnect_on_failure=True
    )
    client.username_pw_set(demoConfig.USER_NAME, demoConfig.PASSWORD)
    # client.tls_set(
    #     ca_certs="Your_CA_Chain.crt",   # Replace with your actual CA chain certificate
    #     certfile="Your_Device_Cert.crt",    # Replace with your actual device certificate
    #     keyfile="Your_Device_Key.key"   # Replace with your actual device key
    # )  # Enable TLS/SSL
    client.on_connect = on_connect
    client.on_message = on_message
    client.on_disconnect = on_disconnect

    client.connect(demoConfig.BROKER, demoConfig.PORT, 60)

    # Start the loop in a separate thread
    client.loop_start()

    try:
        while True:
            try:
                user_input = input("Enter code to send: ")    
                if user_input.upper() == "E":
                    return
                code = int(user_input)
                match code:
                    case 0:
                        publish_config(client, demoConfig.alias, demoConfig.server, demoConfig.userName, demoConfig.password, demoConfig.encrypt, demoConfig.autoIP, demoConfig.localIP, demoConfig.subnetMask, demoConfig.gateway, demoConfig.heartbeat)
                        continue
                    case 1:
                        publish_esl(client, demoConfig.tag_id, get_token(), demoConfig.test_image, True, False, False)
                        continue
                    case 2:
                        publish_esl2(client, demoConfig.tag_id2, get_token(), demoConfig.test_image2, False, True, False)
                        continue
                    case 3:
                        publish_dsl(client, demoConfig.dsl_id_lst, get_token(), demoConfig.dsl_image5A, False, True, False)
                        continue
                    case 4:
                         publish_dsl3(client, demoConfig.dsl_id_lst, get_token(), demoConfig.dsl_image6A, False, True, False)
                         continue
                    case 5:
                        # TODO: Implement firmware publish function
                        continue
                    case 6:
                        publish_ota(
                            client, 
                            f"{demoConfig.firmware_folder}/{demoConfig.firmware_name}", 
                            "1.1.0", 
                            str.format(demoConfig.ota_download_url, demoConfig.STORE_CODE, int(time.time())),
                            str.format(demoConfig.confirm_url, demoConfig.STORE_CODE, int(time.time()))
                        )
                        continue
                    case 7:
                        publish_certificate(
                            client,
                            "Your_Device_Cert.crt",     # Replace with your actual device certificate
                            "Your_Device_Key.key",      # Replace with your actual device key
                            ["Your_Root_CA.crt"],       # Replace with your actual root CA certificate
                            ["Your_Issuing_CA.crt"],    # Replace with your actual issuing CA certificate
                            f"{demoConfig.cert_folder}/{demoConfig.cert_name}",
                            str.format(demoConfig.cert_download_url, demoConfig.cert_name),
                            str.format(demoConfig.confirm_url, demoConfig.cert_name, int(time.time()))
                        )
                        continue
                    case 8:
                        json_path = download_tag_type_json()
                        tag_types = load_tag_types(json_path)
                        publish_tag_type(client, tag_types)
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
