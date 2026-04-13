from dataclasses import dataclass
from typing import List
import msgpack


@dataclass
class OTAData:
    download_url: str = ""
    confirm_url: str = ""
    type: int = 0
    version: str = ""
    name: str = ""
    md5: str = ""

    # =========================
    # MessagePack Serialization
    # =========================
    def to_msgpack(self) -> bytes:
        data = [
            self.download_url,  # Key(0)
            self.confirm_url,   # Key(1)
            self.type,          # Key(2)
            self.version,       # Key(3)
            self.name,          # Key(4)
            self.md5            # Key(5)
        ]
        return msgpack.packb(data, use_bin_type=True)

    # =========================
    # MessagePack Deserialization
    # =========================
    @staticmethod
    def from_msgpack(data: bytes) -> "OTAData":
        unpacked = msgpack.unpackb(data, raw=False)

        return OTAData(
            download_url=unpacked[0],
            confirm_url=unpacked[1],
            type=unpacked[2],
            version=unpacked[3],
            name=unpacked[4],
            md5=unpacked[5]
        )