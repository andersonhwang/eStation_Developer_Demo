namespace eStationDemo.Enum
{
    /// <summary>
    /// Pattern
    /// </summary>
    public enum Pattern
    {
        /// <summary>
        /// Update and display
        /// </summary>
        UpdateDisplay = 0x0,
        /// <summary>
        /// Update part no display
        /// </summary>
        UpdatePart = 0x1,
        /// <summary>
        /// Update no display
        /// </summary>
        Update = 0x2,
        /// <summary>˚
        /// Display
        /// </summary>
        Display = 0x3,
        /// <summary>
        /// Display current tag information
        /// </summary>
        DisplayInfor = 0x4,
        /// <summary>
        /// Clean current tag screen
        /// </summary>
        Query = 0x5,
        /// <summary>
        /// No change, just check tag if exist
        /// </summary>
        Check = 0x6,
        /// <summary>
        /// Reset key
        /// </summary>
        Key = 0x8,
        /// <summary>
        /// Only LED
        /// </summary>
        LED = 0xA,
        /// <summary>
        /// [PTL]Heartbeat time
        /// </summary>
        Heartbeat = 0xB,
        /// <summary>
        /// [PTL]Beacon time
        /// </summary>
        Beacon = 0xC,
    }
}