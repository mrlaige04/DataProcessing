using DataProcessing.Exceptions;
using DataProcessing.Models.Logging;
using DataProcessing.Services.Config;
using DataProcessing.Services.FileProcessing;
using DataProcessing.Services.FileProcessing.Interfaces;
using DataProcessing.Services.FileReading;
using DataProcessing.Services.FolderCheck;
using DataProcessing.Services.Logging;
using DataProcessing.Services.TaskScheduler;
using DataProcessing.Services.Validate;
using Quartz;
using System.Globalization;
using System.Text.Json;

namespace DataProcessing.Services.AppManagement
{
    public class AppManager 
    {
        // Is app running
        private bool _isRunning = false;
        private object locker = new object();
        // Configuration service
        private ConfigurationManager config;

        private Scheduler scheduler;
        private FolderChecker folderChecker;
        private IFileProcessor fileProcessor;
        private MetaLogLogger metaLogger;

        private MetaLogData metaData;

        private int counter = 0;

        private string sourceFolder;


        private string baseDestinationFolder;
        private string fullDestinationFolder;


        private CancellationTokenSource cancellationTokenSource;


        private IEnumerable<string> extentions;

        /// <summary>
        /// 1) Check the config, if it's not valid, throw an exception, if it is, continue
        /// 2) Run await waiting for input command
        /// 3) Create instances of FileProcessor, FolderChecker, MetaLogger, Cancellation Token Source 
        /// </summary>
        /// <param name="config">Configuration Manager</param>
        /// <exception cref="ConfigException"></exception>
        public AppManager(ConfigurationManager config)
        {
            this.config = config;
            sourceFolder = config.GetValue("SourceFolder")!;
            baseDestinationFolder = config.GetValue("DestinationFolder")!;
            if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(baseDestinationFolder))
            {
                throw new ConfigException("Source or destination folder is not set");
            }

            fullDestinationFolder = Path.Combine(baseDestinationFolder, DateTime.Now.ToString("yyyy-MM-dd"));
            extentions = config.GetSection("AllowedExtentions")!;


            
            folderChecker = new FolderChecker(sourceFolder, extentions);
            fileProcessor = new FileProcessor(new FileReader(), new RegexValidateOption(config.GetValue("TransactionPattern")!));
            metaLogger = new MetaLogLogger();
            metaData = new MetaLogData();
            scheduler = new Scheduler();
            scheduler.InitScheduler(this);

            
            cancellationTokenSource = new CancellationTokenSource();
        }


        #region App management

        public async Task EnableConsoleControlAsync()
        {
            await WaitForInputAsync();
        }

        public async Task Start()
        {
            CreateTodayFolderIfNotExists();

            await scheduler.StartScheduler();

            folderChecker.EnableWatcher(ProcessAddedFile, cancellationTokenSource.Token);  
            var files = folderChecker.GetFiles(sourceFolder, extentions);
            Parallel.ForEach(files, async file =>
            {
                await ProcessFileAsync(file, cancellationTokenSource.Token);
            });

        }

        // TODO : IT DOESNT WORK :( (WHEN MANY FILES ALREADY IN FOLDER, CONSOLE DOESNT PROVIDE TO DO ANY INPUT, smth with thread pool(count of available threads))
        public async Task Stop()
        {
            folderChecker.DisableWatcher();
            cancellationTokenSource.Cancel();
        }

        public async Task Reset()
        {
            metaData.Dispose();
            Console.Clear();
        }
        #endregion


        private void CreateTodayFolderIfNotExists()
        {
            var fullPath = Path.Combine(baseDestinationFolder, DateTime.Now.ToShortDateString());
            fullDestinationFolder = fullPath;
            if (!Directory.Exists(fullPath)) Directory.CreateDirectory(fullPath);
        }
        

        #region Processing file
        /* TODO : FIX BUG - SOMETIME IT'S THROWING AN EXCEPTION (FILE IS USED BY ANOTHER PROCESS). IT APPERS SOMETIME WHEN 
         * VERY BIG COUNT OF FILES DROPPING IN FOLDER(WITH LOW COUNT IT`S OK) (smth with filesystemwatcher maybe)
        */
        private async void ProcessAddedFile(object sender, FileSystemEventArgs e)
        {
            CreateTodayFolderIfNotExists();
            var output = await fileProcessor.ProcessFileAsync(e.FullPath, metaData, cancellationTokenSource.Token);
            using (StreamWriter sw = new StreamWriter(Path.Combine(fullDestinationFolder, $"output{IncrementCounter()}.json"),
                new FileStreamOptions() { Mode = FileMode.OpenOrCreate, Access = FileAccess.Write }))
            {
                await sw.WriteAsync(JsonSerializer.Serialize(output));
            }
        }

        private async Task ProcessFileAsync(string filePath, CancellationToken token)
        {
            CreateTodayFolderIfNotExists();
            var output = await fileProcessor.ProcessFileAsync(filePath, metaData, token);
            using (StreamWriter sw = new StreamWriter(Path.Combine(fullDestinationFolder, $"output{IncrementCounter()}.json"),
                new FileStreamOptions() { Mode = FileMode.OpenOrCreate, Access=FileAccess.Write }))
            {
                await sw.WriteAsync(JsonSerializer.Serialize(output));
            }
        }
        #endregion


        public async Task DoAtMidnight()
        {
            await Stop();

            metaLogger.LogToFile(metaData, Path.Combine(fullDestinationFolder, "meta.log"));
            metaData.Dispose();
            CreateTodayFolderIfNotExists();
            counter = 0;
                        
            await Start();
        }


        private async Task WaitForInputAsync()
        {
            await Task.Run(async () =>
            {
                //Thread.CurrentThread.IsBackground = false;
                //Thread.CurrentThread.Priority = ThreadPriority.Normal; // Main thread will not block this thread and this will not main
                while (true)
                {
                    Console.Write("Application is: ");
                    if (_isRunning)
                    {
                        Console.WriteLine("running");
                    }
                    else Console.WriteLine("stopped");

                    if (!Console.KeyAvailable)
                    {
                        Console.WriteLine("Enter command:");
                        string input = Console.ReadLine()!.ToLower();
                        if (input == "start")
                        {
                            if (!_isRunning)
                            {
                                await Start();
                                _isRunning = true;
                            }
                        }
                        else if (input == "stop")
                        {
                            if (_isRunning)
                            {
                                await Stop();
                                _isRunning = false;
                            }
                        }
                        else if (input=="reset")
                        {
                            await Reset(); 
                        }
                        else if (input == "exit")
                        {
                            _isRunning = false;
                            Console.WriteLine("Exiting...");
                            Thread.Sleep(2000);
                            return;
                        }
                        else
                        {
                            Console.WriteLine("invalid command");
                        }
                        Console.Clear();
                    }
                }
            });
        }

        public static async Task WaitForUnlock(FileInfo file)
        {
            while (IsFileLocked(file))
            {
                await Task.Delay(10);
            }
        }
        public static bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
        // To get current counter for the day and increment it
        private int IncrementCounter()
        {
            lock (locker)
            {
                return counter++;
            }
        }
    }
}
