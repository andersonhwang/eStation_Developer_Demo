
import struct
import paho.mqtt.client as mqtt
import time
import array
import base64
import random
import msgpack
import gzip

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
            heartbeat = ApHeartbeat.from_msgpack(msg.payload)
            print(f"eStation Heartbeat: {heartbeat}")
            return
        case appConfig.TOPIC_MESSAGE:
            message = eStationMessage.from_msgpack(msg.payload)
            print(f"eStation Message: {message}")
            return
        case _:
            return

token = 0
# Function to get the next token
def get_token():
    global token
    token += 1
    if token >= 0xFFFF:
        token = 0
    return token

# Function to read a BMP file and convert it to BGRA bytes
def read_bmp_to_bytes(file_path):
        
    with open(file_path, 'rb') as bmp_file:
        # BMP Header (14 Bytes)
        header = bmp_file.read(14)
        if header[0:2] != b'BM':
            raise ValueError("Invalid BMP file")

        # Offset
        data_offset = struct.unpack('<I', header[10:14])[0]

        # Header Information (40 Bytes)
        info_header = bmp_file.read(40)
        if len(info_header) < 40:
            raise ValueError("BMP Header Information is incomplete")

        # Image Information
        width = struct.unpack('<i', info_header[4:8])[0]
        height = struct.unpack('<i', info_header[8:12])[0]
        bits_per_pixel = struct.unpack('<H', info_header[14:16])[0]

        # Check if the bit depth is supported
        if bits_per_pixel not in [24, 32]:
            raise ValueError(f"Unsupported bit depth: {bits_per_pixel}. Only 24 or 32 bit BMP is supported")

        print(f"Image Size: {width} x {height}, Bit Depth: {bits_per_pixel}")

        # Jump to the pixel data start position
        bmp_file.seek(data_offset)

        # Row Size
        row_size = (width * bits_per_pixel + 31) // 32 * 4
        padding = row_size - (width * bits_per_pixel // 8)

        # Read pixels and convert to RGBA
        rgba_bytes = bytearray()
        
        for y in range(height):
            row_data = bmp_file.read(row_size)
            
            for x in range(width):
                pixel_pos = x * (bits_per_pixel // 8)

                if bits_per_pixel == 32:
                    # 32 Bit BMP - BGRA
                    blue = row_data[pixel_pos]
                    green = row_data[pixel_pos + 1]
                    red = row_data[pixel_pos + 2]
                    alpha = row_data[pixel_pos + 3]
                else:  
                    # 24 Bit BMP - BGR
                    blue = row_data[pixel_pos]
                    green = row_data[pixel_pos + 1]
                    red = row_data[pixel_pos + 2]
                    alpha = 255  
                
                rgba_bytes.extend([blue, green, red, alpha])
            bmp_file.read(padding)
        
        return bytes(rgba_bytes), (width, height)
    
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
    image_bytes = read_bmp_to_bytes(image)
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
def publish_esl2(client, id, token, image, r, g, b):
    image_bytes = read_bmp_to_bytes(image)
    print(len(image_bytes[0]))
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

# Main function
def main():
    print("Starting eStation Demo(Python)...")
    print("0: Publish Config")
    print("1: Publish ESL")
    print("2: Publish ESL2")
    print("3: Publish DSL(TODO)")
    print("4: Publish DSL2(TODO)")
    print("5: Publish Firmware(TODO)")
    print("6: Publish OTA(TODO")
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
        
    global token
    token = random.randint(1, 0xFFFF) # Init token
    tag_id = "82000088A2C8"         # Test tag ID
    alias = "09"                    # Alias
    server = "192.168.10.100"    # MQTT server
    userName = "user"               # Username
    password = "12345678"               # Password
    encrypt = False                  # Encryption
    autoIP = False                  # Auto IP
    localIP = "192.168.10.101"       # Local IP
    subnetMask = "255.255.255.0"    # Subnet Mask
    gateway = "192.168.10.1"         # Gateway
    heartbeat = 60                  # Heartbeat
    test_image = "T1.bmp"           # Test image

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
                        publish_esl2(client, tag_id, get_token(), test_image, False, True, False)
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
                        # TODO: Implement OTA publish function
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