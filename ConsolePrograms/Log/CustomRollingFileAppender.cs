using log4net.Appender;
using log4net.Core;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using log4net.Config;

namespace ConsolePrograms {
    public class CustomRollingFileAppender: RollingFileAppender {
        
        public int MaxFileInDirectories { get; set; }
        public int MaxFileInArchive { get; set; }
        public string? LocationLog { get; set; }
        public string? LocationLogArchive { get; set; }
        
        public override void ActivateOptions() {
            base.ActivateOptions();

            Console.WriteLine($"MaxFileInDirectories: {MaxFileInDirectories}");
            Console.WriteLine($"MaxFileInArhiv: {MaxFileInArchive}");
            Console.WriteLine($"LocationLog: {LocationLog}");
            Console.WriteLine($"LocationLogArchive: {LocationLogArchive}");
        }
        
        protected override void AdjustFileBeforeAppend() {
            ArchiveLogs(LocationLog, LocationLogArchive);
            base.AdjustFileBeforeAppend();
        }

        private void ArchiveLogs(string? sourceDirectory, string? zipPath) {
            if (sourceDirectory != null) {
                IEnumerable<string> files = Directory.EnumerateFiles(sourceDirectory, "*.log");
                IEnumerable<string> logFiles = files.Where(file => {
                    DateTime fileLastWriteTime = System.IO.File.GetLastWriteTime(file);
                    DateTime currentDate = DateTime.Now.Date;
                    DateTime fileDate = fileLastWriteTime.Date;
                    return (currentDate - fileDate).Days >= MaxFileInDirectories;
                });

                if (zipPath != null)
                    using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Update)) {
                        foreach (string logFile in logFiles) {
                            string entryName = Path.Combine("archive", Path.GetFileName(logFile));
                            zip.CreateEntryFromFile(logFile, entryName, CompressionLevel.Optimal);
                            System.IO.File.Delete(logFile);
                        }

                        LimitLogFilesInArchive(zip);
                    }
            }
        }
        
        private void LimitLogFilesInArchive(ZipArchive zip){
            List<ZipArchiveEntry> logEntries = zip.Entries
                .Where(entry => entry.FullName.StartsWith("archive\\") && entry.Name.EndsWith(".log"))
                .OrderBy(entry => entry.LastWriteTime)  
                .ToList();
            

            while (logEntries.Count > MaxFileInArchive) {
                ZipArchiveEntry entryToDelete = logEntries.First();
                entryToDelete.Delete();
                logEntries.RemoveAt(0); 
            }
        }
        
    }
}