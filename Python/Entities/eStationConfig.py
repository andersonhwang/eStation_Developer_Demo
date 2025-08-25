import msgpack
from dataclasses import dataclass, field
from typing import List

@dataclass
class eStationConfig:
    """
    eStation configuration
    """
    Alias: str = ""
    Server: str = "192.168.4.92"
    ConnParam: List[str] = field(default_factory=list)
    Encrypt: bool = False
    AutoIP: bool = True
    LocalIP: str = ""
    Subnet: str = ""
    Gateway: str = ""
    Heartbeat: int = 15

    def to_msgpack(self) -> bytes:
        data = [
            self.Alias,
            self.Server,
            self.ConnParam,
            self.Encrypt,
            self.AutoIP,
            self.LocalIP,
            self.Subnet,
            self.Gateway,
            self.Heartbeat
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        data = msgpack.unpackb(packed)
        return eStationConfig(
            Alias=data[0],
            Server=data[1],
            ConnParam=data[2],
            Encrypt=data[3],
            AutoIP=data[4],
            LocalIP=data[5],
            Subnet=data[6],
            Gateway=data[7],
            Heartbeat=data[8]
        )