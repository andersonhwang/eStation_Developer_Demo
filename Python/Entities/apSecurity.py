from dataclasses import dataclass, field
from typing import List
import msgpack


@dataclass
class ApSecurity:
    downloadUrl: str = ""
    confirmUrl: str = ""
    certName: str = ""
    keyName: str = ""
    customTrustStore: List[str] = field(default_factory=list)
    extraStore: List[str] = field(default_factory=list)
    mD5: str = ""

    # =========================
    # MessagePack Serialization
    # =========================
    def to_msgpack(self) -> bytes:
        data = [
            self.downloadUrl,  # Key(0)
            self.confirmUrl,   # Key(1)
            self.certName,     # Key(2)
            self.keyName,      # Key(3)
            self.customTrustStore, # Key(4)
            self.extraStore,    # Key(5)
            self.mD5            # Key(6)
        ]
        return msgpack.packb(data, use_bin_type=True)

    # =========================
    # MessagePack Deserialization
    # =========================
    @staticmethod
    def from_msgpack(data: bytes) -> "ApSecurity":
        unpacked = msgpack.unpackb(data, raw=False)

        return ApSecurity(
            downloadUrl=unpacked[0],
            confirmUrl=unpacked[1],
            certName=unpacked[2],
            keyName=unpacked[3],
            customTrustStore=unpacked[4],
            extraStore=unpacked[5],
            mD5=unpacked[6]
        )