import msgpack
from dataclasses import dataclass, field
from typing import List

@dataclass
class eStationInfor:
    """
    AP information
    """
    ID: str = "0000"
    Alias: str = ""
    IP: str = "127.0.0.1"
    MAC: str = "90A9F7300000"
    ApType: int = 4
    ApVersion: str = "0.0.0"
    ModVersion: str = ""
    DiskSize: int = 0
    FreeSpace: int = 0
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
            self.ID,
            self.Alias,
            self.IP,
            self.MAC,
            self.ApType,
            self.ApVersion,
            self.ModVersion,
            self.DiskSize,
            self.FreeSpace,
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
        return eStationInfor(
            ID=data[0],
            Alias=data[1],
            IP=data[2],
            MAC=data[3],
            ApType=data[4],
            ApVersion=data[5],
            ModVersion=data[6],
            DiskSize=data[7],
            FreeSpace=data[8],
            Server=data[9],
            ConnParam=data[10],
            Encrypt=data[11],
            AutoIP=data[12],
            LocalIP=data[13],
            Subnet=data[14],
            Gateway=data[15],
            Heartbeat=data[16]
        )