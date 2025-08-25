import msgpack
from dataclasses import dataclass, field
from typing import List
from Enums.messageCode import MessageCodes
from .tagHeartbeat import TagHeartbeat

@dataclass
class ApHeartbeat:
    """
    AP heartbeat data
    """
    Id: str = ""
    ConfigVersion: int = 0
    ApVersion: str = "0.0.0"
    ModVersion: str = ""
    Message: MessageCodes = MessageCodes.Ok
    MessageEx: str = ""
    WaitCount: int = -1
    SendCount: int = 0
    Tags: List[TagHeartbeat] = field(default_factory=list)

    def to_msgpack(self) -> bytes:
        data = [
            self.Id,
            self.ConfigVersion,
            self.ApVersion,
            self.ModVersion,
            self.Message.value,
            self.MessageEx,
            self.WaitCount,
            self.SendCount,
            [tag.to_msgpack() for tag in self.Tags]
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        data = msgpack.unpackb(packed)
        tags = [TagHeartbeat.from_msgpack(tag_bytes) for tag_bytes in data[8]]
        return ApHeartbeat(
            Id=data[0],
            ConfigVersion=data[1],
            ApVersion=data[2],
            ModVersion=data[3],
            Message=MessageCodes(data[4]),
            MessageEx=data[5],
            WaitCount=data[6],
            SendCount=data[7],
            Tags=tags
        )