from pathlib import Path
import random
import array

BROKER = "127.0.0.1"    # MQTT broker address
PORT = 9071             # Port
STORE_CODE = "0001"     # Store code
AP_ID = "00C8"          # AP ID
USER_NAME = "test"      # User name
PASSWORD = "123456"     # Password

TOPIC_CONFIG = f"/estation/{AP_ID}/configure"
TOPIC_TASK_ESL = f"/estation/{AP_ID}/taskESL"
TOPIC_TASK_ESL2 = f"/estation/{AP_ID}/taskESL2"
TOPIC_TASK_DSL = f"/estation/{AP_ID}/taskDSL"
TOPIC_TASK_DSL3 = f"/estation/{AP_ID}/taskDSL3"
TOPIC_FIRMWARE = f"/estation/{AP_ID}/firmware"
TOPIC_OTA = f"/estation/{AP_ID}/ota"
TOPIC_CERT = f"/estation/{AP_ID}/cert"
TOPIC_TAG_TYPE = f"/estation/{AP_ID}/tagType"

TOPIC_INFOR = f"/estation/{AP_ID}/infor"
TOPIC_RESULT = f"/estation/{AP_ID}/result"
TOPIC_HEARTBEAT = f"/estation/{AP_ID}/heartbeat"
TOPIC_MESSAGE = f"/estation/{AP_ID}/message"

TAG_ID_LIST = "TagID.txt"


BASE_DIR = Path(__file__).resolve().parent
TAG_TYPE_JSON_URL = "https://github.com/andersonhwang/eStation_Developer_Demo/blob/main/TagType.json"
TAG_TYPE_JSON_FILE = BASE_DIR / "TagType.json"

# Web host configuration
web_host_ip = "192.168.3.33"    # Replace with your actual web host IP address
web_host_port = "9070"          # Replace with your actual web host port
firmware_folder =  "Your_Firmware_Folder"   # Replace with your actual firmware folder path
firmware_name = "eStation2.1.1.4.OTA.tar"   # Replace with your actual firmware package name
cert_folder = "Your_Cert_Folder"            # Replace with your actual certificate folder path
cert_name = "Your_Device_Cert_Package.zip"  # Replace with your actual certificate package name

ota_download_url = f"http://{web_host_ip}:{web_host_port}/ota/2/{firmware_name}?id={{0}}&time={{1}}"
cert_download_url = f"http://{web_host_ip}:{web_host_port}/cert/{{0}}" 
confirm_url = f"http://{web_host_ip}:{web_host_port}/confirm?id={{0}}&time={{1}}"

# Demo parameters - You can modify these parameters for testing
tag_id = "4C00000F6E70"                     # Test tag ID  - 4 colors
tag_id2 = "4C00000F6E70"                    # Test tag ID - 6 colors
tag_id3= "810000F48DAA"                     # Test tag ID - 2.13inch, 250*122
dsl_id_lst = ["D0000020FA56", "D000003412AA", "D0000034129E", "D00000341341", "D00000341158"]
                                            # Test DSL tag IDs
test_image = BASE_DIR / "Images/4C.bmp"     # Test image - 4 colors
test_image2 = BASE_DIR / "Images/4C.bmp"    # Test image - 6 colors
test_image3A = BASE_DIR / "Images/T3A.bmp"  # Test image - BGRA32
test_image3B = BASE_DIR / "Images/T3B.bmp"  # Test image - BGR24
dsl_image = BASE_DIR / "Images/T4.bin"      # Test image for DSL, 320*240
dsl_image5A = BASE_DIR / "Images/T5A.bin"   # Test image for DSL, 320*240
dsl_image5B = BASE_DIR / "Images/T5B.bin"   # Test image for DSL, 320*240
dsl_image6A = BASE_DIR / "Images/T6A.png"   # Test image for DSL, 320*240
dsl_image6B = BASE_DIR / "Images/T6B.gif"   # Test image for DSL, 320*240

# AP Config parameters - Modify these parameters according to your network environment
alias = "08"                                # Alias
server = "192.168.4.90:9071"                # MQTT server
userName = "test"                           # Username
password = "123456"                         # Password
encrypt = True                              # Encryption
autoIP = False                              # Auto IP
localIP = "192.168.4.221"                   # Local IP
subnetMask = "255.255.255.0"                # Subnet Mask
gateway = "192.168.4.1"                     # Gateway
heartbeat = 60                              # Heartbeat