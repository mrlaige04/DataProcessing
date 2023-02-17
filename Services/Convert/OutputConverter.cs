using DataProcessing.Models;
using DataProcessing.Services.Convert.Interfaces;
using static DataProcessing.Models.Output;

namespace DataProcessing.Services.Convert
{
    public class OutputConverter : IConvert<Output, List<Transaction>>
    {
        // TODO : Fix converting when one person bought multiple items(or fixing isnot  necessary for it)
        public Output Convert(List<Transaction> data)
        {
            Output output = new Output();
            output.outputCities = new List<OutputCity>();
            var citiesGrouping = data.GroupBy(x => x.city);
            foreach (var city in citiesGrouping) // all cities
            {
                OutputCity outputCity = new OutputCity(); // make a city
                outputCity.city = city.Key;
                outputCity.services = new List<Service>();
                var servicesGrouping = city.GroupBy(x => x.service);
                foreach (var service in servicesGrouping) // all services
                {
                    Service serviceOutput = new Service();  // make a service
                    serviceOutput.name = service.Key;
                    serviceOutput.payers = new List<Payer>();
                    var payersGrouping = service.GroupBy(x => x.account_number);
                    foreach (var payer in payersGrouping) // all payers
                    {
                        Payer payerOutput = new Payer();
                        payerOutput.account_number = payer.Key;
                        payerOutput.payment = payer.Min(x => x.payment);
                        payerOutput.date = payer.Max(x => x.date);
                        payerOutput.name = payer.First().first_name + " " + payer.First().last_name;
                        serviceOutput.payers.Add(payerOutput);
                    }
                    serviceOutput.total = serviceOutput.payers.Sum(x => x.payment);
                    outputCity.services.Add(serviceOutput);
                }
                outputCity.total = outputCity.services.Sum(x => x.total);
                output.outputCities.Add(outputCity);
            }
            return output;
        }
    }
}
