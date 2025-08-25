
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
            infor = eStationInfor.eStationInfor.from_msgpack(msg.payload)
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

# Function to read a file
def read_file(file_path, compress):
    try:
        with open(file_path, 'rb') as file:
            content = file.read()
            return content
    except FileNotFoundError:
        print(f"File {file_path} not found.")
        return array.array('B')
    
# Function to configure AP
def publish_config(client, alias, server, userName, password, encrypt, autoIP, localIP, subnetMask, gateway, heartbeat):
    config = eStationConfig(
        Alias=alias,
        Server=server,
        ConnParam=[userName, password],
        Encrypt=encrypt,
        AutoIP=autoIP,
        LocalIP=localIP,
        SubnetMask=subnetMask,
        Gateway=gateway,
        Heartbeat=heartbeat
    )
    client.publish(appConfig.TOPIC_CONFIG, config.to_msgpack())

# Function to publish ESL message
def publish_esl(client, id, image, r, g, b):
    esl = [eslEntity(
        TagID=id, 
        Token=get_token(),
        Pattern=Patterns.UpdateDisplay, 
        PageIndex=PageIndexes.P0, 
        Base64String=base64.b64encode(read_file(image)),
        R=r, 
        G=g, 
        B=b)]
    client.publish(appConfig.TOPIC_TASK_ESL, esl.to_msgpack())

# Function to publish ESL2 message
def publish_esl2(client, id, token, image, r, g, b):
    esl = [eslEntity2(
        TagID=id, 
        Token=token,
        Pattern=Patterns.UpdateDisplay, 
        PageIndex=PageIndexes.P0, 
        HexData=base64.b64encode(read_file(image)),
        R=r, 
        G=g, 
        B=b)]
    client.publish(appConfig.TOPIC_TASK_ESL2, esl.to_msgpack())

# Main function
def main():
    print("Starting eStation Demo(Python)...")

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
    test_image = "T0.bmp"           # Test image
    tag_id = "3D00000102D3"         # Test tag ID
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

    try:
        while True:
            try:
                user_input = input("Enter code to send: ")    
                if user_input.upper() == "E":
                    return
                code = int(user_input)
                match code:
                    case 1:
                        publish_config(client, alias, server, userName, password, encrypt, autoIP, localIP, subnetMask, gateway, heartbeat)
                        break
                    case 2:
                        publish_esl(client, tag_id, get_token(), test_image, True, False, False)
                        break
                    case 3:
                        publish_esl2(client, tag_id, get_token(), test_image, False, True, False)
                        break
                    case 4:
                        # TODO: Implement DSL publish function
                        break
                    case 5:
                         # TODO: Implement DSL2 publish function
                         break
                    case 6:
                        # TODO: Implement firmware publish function
                        break
                    case 7:
                        # TODO: Implement OTA publish function
                        break
                    case _:
                        print("Unknown code.")
            except ValueError:
                print("Invalid input. Please enter a valid code.")
            except KeyboardInterrupt:
                print("Exiting...")
    finally:
        client.loop_stop()
        client.disconnect()

if __name__ == "__main__":
    main()