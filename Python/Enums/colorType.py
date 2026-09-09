from enum import IntEnum


class ColorType(IntEnum):
    """
    Tag color type
    """
    Black = 1
    Red = 2
    Yellow = 4

    BlackRed = Black | Red
    BlackYellow = Black | Yellow
    BlackRedYellow = Black | Red | Yellow

    Color4 = 8
    Color6 = 9
    Color7 = 10
