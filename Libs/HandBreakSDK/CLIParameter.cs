namespace HandBreakSDK
{
    internal struct CLIValue;
    internal readonly struct CLIParameter(string key)
    {
        public string Key { get; } = key;
        public CLIValue? Value { get; } = null;

        public CLIParameter(string key, CLIValue value) : this(key)
        {
            Value = value;
        }
    }
}