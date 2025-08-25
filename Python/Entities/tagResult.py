import msgpack
from dataclasses import dataclass, field
from typing import List

@dataclass
class TagResult:
    """
    Tag result
    """
    TagId: str = ""
    RfPower: int = -256
    Battery: int = 0
    Version: int = 0
    Status: int = 0
    Token: int = 0
    Temperature: int = 0
    Channel: int = 0
    UtcTime: bytes = field(default_factory=bytes)
    TimePercent: bytes = field(default_factory=bytes)
    Count: int = 0

    def to_msgpack(self) -> bytes:
        data = [
            self.TagId,
            self.RfPower,
            self.Battery,
            self.Version,
            self.Status,
            self.Token,
            self.Temperature,
            self.Channel,
            self.UtcTime,
            self.TimePercent,
            self.Count
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        data = msgpack.unpackb(packed)
        return TagResult(
            TagId=data[0],
            RfPower=data[1],
            Battery=data[2],
            Version=data[3],
            Status=data[4],
            Token=data[5],
            Temperature=data[6],
            Channel=data[7],
            UtcTime=data[8],
            TimePercent=data[9],
            Count=data[10]
        )