using DataProcessing.Models;
using DataProcessing.Models.Logging;
using DataProcessing.Services.Convert;
using DataProcessing.Services.Convert.Interfaces;
using DataProcessing.Services.FileProcessing.Interfaces;
using DataProcessing.Services.FileReading;
using DataProcessing.Services.FileReading.Interfaces;
using DataProcessing.Services.Validate;
using DataProcessing.Services.Validate.Interfaces;
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
        
        public async Task<Output> ProcessFileAsync(string filePath, MetaLogData metaData)
        {
            List<Transaction> transactions = new List<Transaction>();
            var lines = await _fileReader.ReadAllLinesAsync(filePath);
            File.Delete(filePath);
            Parallel.ForEach(lines, x =>
            {
                var result = _validator.Validate(x) as RegexValidationResult;
                if (result != null && result.IsValid)
                {
                    var transaction = _transaction_regex_converter.Convert(result.Groups);
                    transactions.Add(transaction);
                }
            });                      
            return _file_output_converter.Convert(transactions);
        }
    }    
}
