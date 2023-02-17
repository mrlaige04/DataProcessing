namespace DataProcessing.Models
{
    public class Output
    {
        public List<OutputCity> outputCities { get; set; }
        public class OutputCity
        {
            public string city { get; set; }
            public List<Service> services { get; set; }
            public decimal total { get; set; }
        }
    }
}
