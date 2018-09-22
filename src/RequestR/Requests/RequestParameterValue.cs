namespace RequestR.Requests
{
    internal class RequestParameterValue
    {
        internal string Value { get; set; }
        internal RequestParameterType Type { get; }

        public RequestParameterValue(string value, RequestParameterType type)
        {
            Value = value;
            Type = type;
        }
    }
}