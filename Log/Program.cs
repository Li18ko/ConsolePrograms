namespace Log {
    public class Program {
        public static void Main(string[] args) {
            Logger logger = new Logger();
            
            logger.AddLogger(new ConsoleLog());
            logger.AddLogger(new FileLog("log.txt"));
            
            logger.Info("Программа началась");
            logger.Debug("Действие...");
        }
    }  
}