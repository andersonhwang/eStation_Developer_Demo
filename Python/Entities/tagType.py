import msgpack
from dataclasses import dataclass
from typing import List

from Enums.colorType import ColorType


@dataclass
class TagType:
    """
    Tag type
    """
    Code: str
    Width: int
    Height: int
    Type: str
    Color: ColorType = ColorType.BlackRed

    @property
    def Length(self) -> int:
        return self.Height * self.Width * 4

    @property
    def Length2(self) -> int:
        width_aligned = self.Width if (self.Width % 8) == 0 else (((self.Width // 8) + 1) * 8)
        return width_aligned * self.Height * 4

    def to_array(self):
        return [
            self.Code,          # Key(0)
            self.Height,        # Key(1)
            self.Width,         # Key(2)
            self.Type,          # Key(3)
            int(self.Color)     # Key(4)
        ]

    def to_msgpack(self) -> bytes:
        return msgpack.packb(self.to_array())

    @staticmethod
    def from_dict(data: dict) -> "TagType":
        color = data.get("Color", ColorType.BlackRed)
        if not isinstance(color, ColorType):
            color = ColorType(color)
        return TagType(
            Code=data["Code"],
            Width=data["Width"],
            Height=data["Height"],
            Type=data["Type"],
            Color=color
        )

    @staticmethod
    def from_msgpack(packed: bytes) -> "TagType":
        data = msgpack.unpackb(packed, raw=False)
        return TagType(
            Code=data[0],
            Height=data[1],
            Width=data[2],
            Type=data[3],
            Color=ColorType(data[4])
        )

    @staticmethod
    def list_to_msgpack(items: List["TagType"]) -> bytes:
        return msgpack.packb([item.to_array() for item in items])

    @staticmethod
    def list_from_json(items: List[dict]) -> List["TagType"]:
        return [TagType.from_dict(item) for item in items]
