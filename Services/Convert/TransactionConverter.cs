using DataProcessing.Models;
using DataProcessing.Services.Convert.Interfaces;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DataProcessing.Services.Convert
{
    public class TransactionConverter : IConvert<Transaction, GroupCollection>
    {
        public Transaction Convert(GroupCollection groups, CancellationToken token = default)
        {
            string[] validformats = new[] { "MM/dd/yyyy", "yyyy/MM/dd", "MM/dd/yyyy", "MM/dd/yyyy", "yyyy-dd-MM", "yyyy-MM-dd" };
            Transaction trans = new Transaction();
            trans.account_number = long.Parse(groups["account_number"].Value);
            trans.city = groups["city"].Value;
            trans.date = DateTime.ParseExact(groups["date"].Value, validformats, null);
            trans.first_name = groups["first_name"].Value;
            trans.last_name = groups["last_name"].Value;
            trans.payment = decimal.Parse(groups["payment"].Value, CultureInfo.InvariantCulture);
            trans.service = groups["service"].Value;
            return trans;
        }
    }
}
