using DataProcessing.Models;
using DataProcessing.Services;
using DataProcessing.Services.Convert;
using DataProcessing.Services.Validate;
using DataProcessing.Services.Validate.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;





string pattern = "^(?<first_name>[\\w]+),(?<last_name>[\\w]+),[“|\"|\'](?<city>\\w+),(?<street>[\\w]+)(?<building>\\d+),(?<apartment>\\d)[“|\"|'],(?<payment>[\\d.]+),(?<date>\\d{4}-\\d{2}-\\d{2}),(?<account_number>\\d+),(?<service>[\\w]+)$";
IValidator validator = new Validator(new RegexValidateOption(pattern));

TransactionConverter converter = new TransactionConverter();
string csvpath = @"E:\WORK\Radency\DataProcessing\DataProcessing\Files\folder_a\data2.csv";

List<Transaction> transactions = new List<Transaction>();
using (StreamReader sr = new StreamReader(csvpath))
{
    var file = await sr.ReadToEndAsync();
    var lines = file.Split("\r\n", StringSplitOptions.RemoveEmptyEntries).ToList();
    lines.RemoveAll(x => x.Contains("first_name,last_name,address,payment,date,account_number,service"));

    foreach (var item in lines)
    {
        var valid = validator.Validate(item) as RegexValidationResult;
        if (valid != null && valid.IsValid)
        {
            var obj = converter.Convert(valid.Groups);
            transactions.Add(obj);
        }
    }
}

Console.WriteLine(JsonSerializer.Serialize(transactions));
