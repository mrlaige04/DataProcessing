using DataProcessing.Models;
using DataProcessing.Services.Convert;
using DataProcessing.Services.Validate;
using DataProcessing.Services.Validate.Interfaces;
using System.Text.RegularExpressions;

string pattern = "^(?<first_name>[\\w]+),(?<last_name>[\\w]+),[“|\"|\'](?<city>\\w+),(?<street>[\\w]+)(?<building>\\d+),(?<apartment>\\d)[“|\"|'],(?<payment>[\\d.]+),(?<date>\\d{4}-\\d{2}-\\d{2}),(?<account_number>\\d+),(?<service>[\\w]+)$";
IValidator validator = new Validator(new RegexValidateOption(pattern));
//var valid = validator.Validate("John, Doe, “Lviv, Kleparivska 35, 4”, 500.0, 2022-27-01, 1234567, Water");
//foreach (Group item in ((RegexValidationResult)valid).Groups)
//{
//    Console.WriteLine(item.Name + " - " + item.Value);
//}
TransactionConverter converter = new TransactionConverter();
string csvpath = @"E:\WORK\Radency\DataProcessing\DataProcessing\Files\data.csv";

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

var groupbycities = transactions.GroupBy(x => x.city);
foreach (var city in groupbycities)
{
    Console.WriteLine("City: " + city.Key);
    var services = city.GroupBy(x => x.service);
    foreach (var service in services)
    {
        Console.WriteLine("\t Service: " + service.Key);
        foreach (var payer in service)
        {
            Console.WriteLine("\t\t Payer: " + payer.first_name + " " + payer.last_name);
            Console.WriteLine("\t\t\t Payment: " + payer.payment + " Date: " + payer.date.ToShortDateString() + " Account Number: " + payer.account_number);
            Console.WriteLine("----------");
        }
    }
    Console.WriteLine("====================================");
}

