namespace shift
{
    public enum ProblemType
    {
        Error,
        Warning
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
    }
}