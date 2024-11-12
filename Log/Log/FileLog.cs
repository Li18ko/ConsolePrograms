namespace Log {
    public class FileLog: ILogger{
        private readonly string _filePath;

        public FileLog(string filePath){
            _filePath = filePath;
        }

        public void Write(string message){
            try {
                File.AppendAllText(_filePath, message + Environment.NewLine);
            }
            catch (Exception ex) {
                Console.WriteLine($"Ошибка записи в файл: {ex.Message}");
            }
        }
    }
}