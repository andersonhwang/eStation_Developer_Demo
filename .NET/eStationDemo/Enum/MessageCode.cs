namespace eStationDemo.Enum
{
    /// <summary>
    /// eStation message code
    /// </summary>
    public enum MessageCode
    {
        OK = 0,
        Result = 1,
        Idle = 2,
        Heartbeat = 3,
        PortError = 4,
        AppEror = 5,
        Busy = 6,
        MaxLimit = 7,
        Invalid_Task_ESL = 8,
        Invalid_Task_DS = 9,
        Invalid_Config = 10,
    }
}