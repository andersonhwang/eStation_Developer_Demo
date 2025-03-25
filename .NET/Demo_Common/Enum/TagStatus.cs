namespace Demo_Common.Enum
{
    /// <summary>
    /// Tag status
    /// </summary>
    public enum TagStatus
    {
        Init = 0,
        Sending = 1,
        Success = 2,
        Error = 3,
        Heartbeat = 4,
        LowPower = 5,
        InvaidKey = 6,
        DuplicateToken = 7,
        LcmdIdError = 8,
        LcmdRefreshError = 9,
        McuReset = 10,        
        Unknown = 99
    }
}
