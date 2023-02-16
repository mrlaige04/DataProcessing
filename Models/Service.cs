namespace DataProcessing.Models
{
    public class Service
    {
        public string name { get; set; }
        public List<Payer> payers { get; set; }
        public decimal total { get; set; }
    }
}
