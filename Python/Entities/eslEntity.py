import msgpack
from dataclasses import dataclass, field
from Enums.pattern import Patterns
from Enums.pageIndex import PageIndexes

@dataclass
class ESLEntity:
    """
    ESL entity for eStation2
    """
    TagID: str = ""
    Pattern: Patterns = Patterns.UpdateDisplay
    PageIndex: PageIndexes = PageIndexes.P0
    R: bool = False
    G: bool = False
    B: bool = False
    Times: int = 0
    Token: int = 0
    CurrentKey: str = ""
    NewKey: str = ""
    Base64String: str = ""

    def to_msgpack(self) -> bytes:
        # Serializes the object to MessagePack format using key order
        data = [
            self.TagID,
            self.Pattern,
            self.PageIndex,
            self.R,
            self.G,
            self.B,
            self.Times,
            self.Token,
            self.CurrentKey,
            self.NewKey,
            self.Base64String
        ]
        return msgpack.packb(data)

    @staticmethod
    def from_msgpack(packed: bytes):
        # Deserializes MessagePack data to ESLEntity
        data = msgpack.unpackb(packed)
        return ESLEntity(
            TagID=data[0],
            Pattern=data[1],
            PageIndex=data[2],
            R=data[3],
            G=data[4],
            B=data[5],
            Times=data[6],
            Token=data[7],
            CurrentKey=data[8],
            NewKey=data[9],
            Base64String=[10]
        )