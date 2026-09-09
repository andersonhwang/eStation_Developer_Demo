import msgpack
from dataclasses import dataclass, field
from Enums.pattern import Patterns

@dataclass
class DSLEntity3:
    """
    DSL Entity3 for eStation2
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
    IsGif: bool = False     # Is GIF
    Width: int = 320        # Width
    Height: int = 240       # Height
    Top: int = 0            # Top
    Left: int = 0           # Left

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
            self.Pattern.value if isinstance(self.Pattern, Patterns) else self.Pattern,
            self.CurrentKey,
            self.NewKey,
            self.IsGif,
            self.Width,
            self.Height,
            self.Top,
            self.Left
        ]
        return msgpack.packb(data, use_bin_type=True)

    @staticmethod
    def from_msgpack(packed: bytes):
        # Deserializes MessagePack data to DSLEntity3
        data = msgpack.unpackb(packed)
        return DSLEntity3(
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
            NewKey=data[11],
            IsGif=data[12],
            Width=data[13],
            Height=data[14],
            Top=data[15],
            Left=data[16]
        )
