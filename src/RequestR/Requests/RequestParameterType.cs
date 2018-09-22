namespace RequestR.Requests
{
    using System;

    [Flags]
    public enum RequestParameterType : byte
    {
        None = 0,
        Query = 0b1,
        Body = 0b10,

        QueryOrBody = Query | Body
    }
}