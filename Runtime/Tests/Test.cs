namespace Runtime.Tests
{
    public abstract class Test
    {
        public abstract (TestResult, string) Start();
    }

    public enum TestResult
    {
        Success,
        Failure
    }
}
