using System.Text.Json;

namespace DataProcessing.Models
{
    public class Transaction
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string city { get; set; }
        public decimal payment { get; set; }
        public DateTime date { get; set; }
        public long account_number { get; set; }
        public string service { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
