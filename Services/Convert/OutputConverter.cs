using DataProcessing.Models;
using DataProcessing.Services.Convert.Interfaces;
using static DataProcessing.Models.Output;

namespace DataProcessing.Services.Convert
{
    public class OutputConverter : IConvert<Output, List<Transaction>>
    {
        public Output Convert(List<Transaction> data, CancellationToken token = default)
        {
            Output output = new Output() { outputCities = new List<OutputCity>()};
            var citiesGrouping = data.GroupBy(x => x?.city);
            foreach (var city in citiesGrouping) // all cities
            {
                OutputCity outputCity = new OutputCity() { city = city.Key, services = new List<Service>()}; // make a city
                var servicesGrouping = city.GroupBy(x => x.service);
                foreach (var service in servicesGrouping) // all services
                {
                    Service serviceOutput = new Service() { name=service.Key, payers = new List<Payer>()};  
                    var payersGrouping = service.GroupBy(x => new { x.account_number, x.date });
                    foreach (var payer in payersGrouping) // all payers
                    {
                        Payer newPayer = new Payer()
                        {
                            account_number = payer.Key.account_number,
                            payment = payer.Sum(x => x.payment),
                            date = payer.Key.date,
                            name = payer.First().first_name + " " + payer.First().last_name,
                        };
                        serviceOutput.payers.Add(newPayer);
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
