namespace Demo_Common.Enum
{
    [Flags]
    public enum ColorType
    {
        Black = 1,
        Red = 2,
        Yellow = 4,

        BlackRed = Black | Red,
        BlackYellow = Black | Yellow,
        BlackRedYellow = Black | Red | Yellow,
    }
}
