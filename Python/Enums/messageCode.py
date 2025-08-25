from enum import Enum

class MessageCodes(Enum):
    """
    eStation message codes
    """
    Ok = 0
    Idle = 1
    Result = 2
    Heartbeat = 3
    ModError = 4
    AppEror = 5
    Busy = 6
    MaxLimit = 7
    Invalid_Task_ESL = 8
    Invalid_Task_DSL = 9
    Invalid_Config = 10
    Invalid_OTA = 11
    Invalid_Firmware = 12