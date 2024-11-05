namespace Log {
    public interface ILogger {
        void Debug(string message);
        void Info(string message);
        void Error(string message, Exception ? ex = null);
    }
}