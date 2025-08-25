import msgpack
from dataclasses import dataclass
from Enums.messageCode import MessageCodes

@dataclass
class eStationMessage:
    """
    eStation message
    """
    Code: MessageCodes = MessageCodes.Ok

    def to_msgpack(self) -> bytes:
        return msgpack.packb([self.Code.value])

    @staticmethod
    def from_msgpack(packed: bytes):
        data = msgpack.unpackb(packed)
        return eStationMessage(Code=MessageCodes(data[0]))