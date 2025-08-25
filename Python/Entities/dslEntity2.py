import msgpack
from dataclasses import dataclass, field
from Enums.pattern import Patterns

@dataclass
class DSLEntity2:
    """
    DSL Entity2 for eStation2
    """
    TagID: str = ""
    R: bool = False
    G: bool = False
    B: bool = False
    Period: int = 3600      # -1~3600, Default 3600
    Interval: int = 1000    # 100~10000, Default 1000
    Duration: int = 50      # 50~100, Default 50
    Token: int = 0
    HexData: bytes = field(default_factory=bytes)
    Pattern: Patterns = Patterns.UpdateDisplay     # Pattern
    CurrentKey: str = ""
    NewKey: str = ""

    def to_msgpack(self) -> bytes:
        # Serializes the object to MessagePack format using key order
        data = [
            self.TagID,
            self.R,
            self.G,
            self.B,
            self.Period,
            self.Interval,
            self.Duration,
            self.Token,
            self.HexData,
            self.Pattern,      
            self.CurrentKey,
            self.NewKey
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        # Deserializes MessagePack data to DSLEntity2
        data = msgpack.unpackb(packed)
        return DSLEntity2(
            TagID=data[0],
            R=data[1],
            G=data[2],
            B=data[3],
            Period=data[4],
            Interval=data[5],
            Duration=data[6],
            Token=data[7],
            HexData=data[8],
            Pattern=data[9],
            CurrentKey=data[10],
            NewKey=data[11]
        )