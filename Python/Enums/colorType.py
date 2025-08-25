from enum import Flag, auto

class ColorTypes(Flag):
    Black = auto()
    Red = auto()
    Yellow = auto()

    BlackRed = Black | Red
    BlackYellow = Black | Yellow
    BlackRedYellow = Black | Red | Yellow