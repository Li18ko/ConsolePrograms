namespace Log {
    public class Logger {
        private readonly List<ILogger> _loggers;

        public Logger(IEnumerable<ILogger> loggers) {
            _loggers = loggers.ToList();   
        }

        public void AddLogger(ILogger logger) {
            _loggers.Add(logger);
        }
        
        public void Info(string message) => Log(message, LogLevel.Info);
        public void Warning(string message) => Log(message, LogLevel.Warning);
        public void Error(string message) => Log(message, LogLevel.Error);
        public void Debug(string message) => Log(message, LogLevel.Debug);

        private void Log(string message, LogLevel level) {
            string logEntry = $"{DateTime.Now:G} [{level}] {message}";
            foreach (var logger in _loggers) {
                logger.Write(logEntry); 
            }
        }


    }
}