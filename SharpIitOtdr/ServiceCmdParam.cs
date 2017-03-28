namespace IitOtdrLibrary
{
    public enum ConnectionTypes
    {
        Tcp = 103,
    }
    public enum ServiceCmdParam
    {
        Unit               =  1 ,
        Lmax               =  2 ,
        L1                 =  3 ,
        L2                 =  4 ,
        Res                =  5 ,
        Pulse              =  6 ,
        Navr               =  7 ,
        Time               =  8 ,
        IsTime             =  9 ,
        Gi                 =  10,
        Ri                 =  10,
        Bc                 =  11,
        HiRes              =  12,
        LoPow              =  13,
        LowPow             =  13,
        Refresh            =  14,
        WlEnabled          =  15,
        Filter             =  16,
        L1Val              =  17,
        L2Val              =  18,
        LmaxVal            =  72,
        PulseVal           =  73,
        NavrVal            =  74,
        MaxLmax            =  75,
        MaxPulse           =  76,
        TimeVal            =  77,
        Conn               =  78,
        OverrideScaleAvr   =  79,
        ScaleAvr           =  80,
        ScaleAvrVal        =  81,
        DwdmChannel        =  82,
        FastAvrNumber      =  83,
    }
}
