using DataProcessing.Models;
using DataProcessing.Models.Logging;
using DataProcessing.Services.AppManagement;
using DataProcessing.Services.Convert;
using DataProcessing.Services.Convert.Interfaces;
using DataProcessing.Services.FileProcessing.Interfaces;
using DataProcessing.Services.FileReading;
using DataProcessing.Services.FileReading.Interfaces;
using DataProcessing.Services.Validate;
using DataProcessing.Services.Validate.Interfaces;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DataProcessing.Services.FileProcessing
{
    public class FileProcessor : IFileProcessor
    {
        private IFileReader _fileReader;
        private IValidator _validator;
        private IConvert<Output, List<Transaction>> _file_output_converter;
        private IConvert<Transaction, GroupCollection> _transaction_regex_converter;
        public FileProcessor(IFileReader fileReader, IValidationOption validationOption)
        {
            _fileReader = new FileReader();
            _validator = new Validator(validationOption);
            _file_output_converter = new OutputConverter();
            _transaction_regex_converter = new TransactionConverter();
        }

        public async Task<Output> ProcessFileAsync(string filePath, MetaLogData metaData, CancellationToken token = default)
        {
            bool isFileInvalid = false;
            List<Transaction> transactions = new List<Transaction>();

            await AppManager.WaitForUnlock(new FileInfo(filePath));

            var lines = await _fileReader.ReadAllLinesAsync(filePath, token);

            try
            {
                Parallel.ForEach(lines, new ParallelOptions() { CancellationToken = token }, x =>
                {
                    var result = _validator.Validate(x) as RegexValidationResult;

                    if (result != null && result.IsValid)
                    {
                        metaData.parsed_lines++;
                        var transaction = _transaction_regex_converter.Convert(result.Groups);
                        transactions.Add(transaction);
                    }
                    else
                    {
                        metaData.found_errors++;
                        isFileInvalid = true;
                    }
                });
            } catch { Debug.WriteLine("Error"); }
            

            
            
            metaData.parsed_files++;
            if (isFileInvalid)
            {
                metaData.invalid_files.Add(filePath);
            }
            await AppManager.WaitForUnlock(new FileInfo(filePath));
            File.Delete(filePath);
            return _file_output_converter.Convert(transactions, token);
        }
    }
}
