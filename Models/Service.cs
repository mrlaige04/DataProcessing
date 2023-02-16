using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProcessing.Models
{
    public class Service
    {
        public string name { get; set; }
        public List<Payer> payers { get; set; }
        public decimal total { get; set; }
    }
}
