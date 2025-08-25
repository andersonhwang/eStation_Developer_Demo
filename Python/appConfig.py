BROKER = "127.0.0.1"    # MQTT broker address
PORT = 9071             # Port
AP_ID = "018Q"          # AP ID
USER_NAME = "test"      # User name
PASSWORD = "123456"     # Password

TOPIC_CONFIG = f"/estation/{AP_ID}/configure"
TOPIC_TASK_ESL = f"/estation/{AP_ID}/taskESL"
TOPIC_TASK_ESL2 = f"/estation/{AP_ID}/taskESL2"
TOPIC_TASK_DSL = f"/estation/{AP_ID}/taskDSL"
TOPIC_TASK_DSL2 = f"/estation/{AP_ID}/taskDSL2"
TOPIC_FIRMWARE = f"/estation/{AP_ID}/firmware"
TOPIC_OTA = f"/estation/{AP_ID}/ota"

TOPIC_INFOR = f"/estation/{AP_ID}/infor"
TOPIC_RESULT = f"/estation/{AP_ID}/result"
TOPIC_HEARTBEAT = f"/estation/{AP_ID}/heartbeat"
TOPIC_MESSAGE = f"/estation/{AP_ID}/message"

TAG_ID_LIST = "TagID.txt"