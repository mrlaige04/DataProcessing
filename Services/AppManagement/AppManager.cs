using DataProcessing.Exceptions;
using DataProcessing.Models.Logging;
using DataProcessing.Services.AppManagement.Interfaces;
using DataProcessing.Services.Config;
using DataProcessing.Services.FileProcessing;
using DataProcessing.Services.FileProcessing.Interfaces;
using DataProcessing.Services.FileReading;
using DataProcessing.Services.FolderCheck;
using DataProcessing.Services.Logging;
using DataProcessing.Services.Validate;
using System.Globalization;
using System.Text.Json;

namespace DataProcessing.Services.AppManagement
{
    public class AppManager 
    {
        private ConfigurationManager config;


        private FolderChecker folderChecker;
        private IFileProcessor fileProcessor;
        private MetaLogLogger metaLogger;

        private MetaLogData metaData;
        
        private int counter = 0;

        private string sourceFolder;

        
        private string baseDestinationFolder;
        private string fullDestinationFolder;


        private IEnumerable<string> extentions;
        public AppManager(ConfigurationManager config)
        {
            this.config = config;
            sourceFolder = config.GetValue("SourceFolder")!;


            // TODO : Check if the folder path in config exists (it`s maybe null or empty) so that's way we need to !stop program.           
            baseDestinationFolder = config.GetValue("DestinationFolder")!;
            if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(baseDestinationFolder))
            {
                throw new ConfigException("Source or destination folder is not set");
            }

            
            baseDestinationFolder = Path.Combine(baseDestinationFolder, DateTime.Now.ToShortDateString());



            extentions = config.GetSection("AllowedExtentions")!;
            folderChecker = new FolderChecker(sourceFolder, extentions);
            fileProcessor = new FileProcessor(new FileReader(), new RegexValidateOption(config.GetValue("TransactionPattern")!));

            metaLogger = new MetaLogLogger();

            metaData = new MetaLogData();
        }

        public async Task Start()
        {
            if (!Directory.Exists(baseDestinationFolder)) Directory.CreateDirectory(baseDestinationFolder);
            
            folderChecker.EnableWatcher(ProcessAddedFile);
            var files = folderChecker.GetFiles(sourceFolder, extentions);
            Parallel.ForEach(files, async file =>
            {
                await ProcessFileAsync(file);
            });
            

            Console.ReadKey();// TODO: REMOVE
        }

        private async void ProcessAddedFile(object sender, FileSystemEventArgs e)
        {
            await fileProcessor.ProcessFileAsync(e.FullPath, metaData);
        }

        private async Task ProcessFileAsync(string filePath)
        {
            var output = await fileProcessor.ProcessFileAsync(filePath, metaData);
            using (StreamWriter sw = new StreamWriter(Path.Combine(baseDestinationFolder, $"output{IncrementCounter()}.json")))
            {
                await sw.WriteAsync(JsonSerializer.Serialize(output));
            }
            
        }









        /* TODO : 1) Use QUARTH with IJOB Interface to schedule the task(write metalog at midnight at destination folder),
         * 2) also dispose the MetaData object to make 0 all fiels.
         * 3) Create new folder with name of current date
        */

        /* To avoid problems, when it`s a midnight program stops and makes meta log,
         * changes destination folder, dispose metadata object*/
        public async void DoAtMidnight()
        {
            Stop();
            await metaLogger.LogToFile(metaData, Path.Combine(baseDestinationFolder, "meta.log"));
            metaData.Dispose();
            baseDestinationFolder = config.GetValue("DestinationFolder")!;
            baseDestinationFolder = Path.Combine(baseDestinationFolder, DateTime.Now.ToString(CultureInfo.CurrentCulture)); // Create new folder with today date
            Start();
        }





        public string GetFullDestinationFolder()
        {
            var configDest = config.GetValue("DestinationFolder")!;
            if (string.IsNullOrEmpty(baseDestinationFolder))
            {
                throw new ConfigException("Destination folder is not set");
            }

            var fullDestFolderNow = Path.Combine(configDest, DateTime.Now.ToString("d"));
            Console.WriteLine(fullDestFolderNow);
            return fullDestFolderNow;
        }



        
        public void Stop()
        {
            folderChecker.DisableWatcher();
        }
        
        public void Reset()
        {
            throw new NotImplementedException();
        }

        private int IncrementCounter()
        {
            object locker = new object();
            lock(locker)
            {
                return counter++;
            }
        }
    }
}
