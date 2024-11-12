namespace Log {
    public class ConsoleLog: ILogger {
        public void Write(string message) {
            Console.WriteLine(message);
        }
    }
}
