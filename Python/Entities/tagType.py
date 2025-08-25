from dataclasses import dataclass
from Enums import ColorType

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
        return self.Height