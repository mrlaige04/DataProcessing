namespace DataProcessing.Models
{
    public class Payer
    {
        public string name { get; set; }
        public decimal payment { get; set; }
        public DateTime date { get; set; }
        public long account_number { get; set; }
    }
}
