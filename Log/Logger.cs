using log4net;
using System.Reflection;
namespace Log {
    public class Logger: ILogger {
        private readonly ILog _logger;
        public Logger() {
            this._logger = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        }
        public void Debug(string message) {
            this._logger?.Debug(message);
        }
        public void Info(string message) {
            this._logger?.Info(message);
        }
        
        public void Error(string message, Exception? ex = null) {
            this._logger?.Error(message, ex);
            if (ex?.InnerException != null) {
                this._logger?.Error("Inner Exception: " + ex.InnerException.Message, ex.InnerException);
            }
        }


    }
}