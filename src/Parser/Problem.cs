namespace shift
{
    public enum ProblemType
    {
        Error,
        Warning
    }

    public class OverwriteWarning : Problem
    {
        public OverwriteWarning(string varName) : base(ProblemType.Warning, $"New value overwrites previous `{varName}`") { }
    }

    public class Problem
    {
        public string Message { get; private set; }

        public ProblemType Type { get; private set; }

        public Problem(ProblemType type, string message)
        {
            this.Message = message;
            this.Type = type;
        }

        public void Report(int lineNumber = 0)
        {
            if (Type == ProblemType.Error)
                ShiftParser.Error(Message, lineNumber);
            else if (Type == ProblemType.Warning)
                ShiftParser.Warn(Message, lineNumber);
        }
    }
}