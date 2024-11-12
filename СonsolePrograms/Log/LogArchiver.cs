using System.IO.Compression;

namespace СonsolePrograms {
    public class LogArchiver {

        public static void ArchiveLogs(string sourceDirectory, string zipPath) {
            IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory, "*.log");
            IEnumerable<string> logFiles = files.Where(file => {
                DateTime fileLastWriteTime = File.GetLastWriteTime(file);
                return fileLastWriteTime <= DateTime.Now.AddDays(-1);
            });
            
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Update)) {
                foreach (string logFile in logFiles) {
                    string entryName = Path.Combine("archive", Path.GetFileName(logFile));
                    zip.CreateEntryFromFile(logFile, entryName, CompressionLevel.Optimal);
                    File.Delete(logFile);
                }
                LimitLogFilesInArchive(zip);
            }
        }
        
        private static void LimitLogFilesInArchive(ZipArchive zip){
            List<ZipArchiveEntry> logEntries = zip.Entries
                .Where(entry => entry.FullName.StartsWith("archive\\") && entry.Name.EndsWith(".log"))
                .OrderBy(entry => entry.LastWriteTime)  
                .ToList();
            

            while (logEntries.Count > 10) {
                ZipArchiveEntry entryToDelete = logEntries.First();
                entryToDelete.Delete();
                logEntries.RemoveAt(0); 
            }
        }
        
    }
}