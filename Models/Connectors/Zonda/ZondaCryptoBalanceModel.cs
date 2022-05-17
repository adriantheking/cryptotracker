using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Connectors.Zonda
{
    public class ZondaCryptoBalanceModel
    {
        //Cryptocurrency name
        public string Name { get; set; }
        public decimal? Amount { get; set; }
        public decimal? Invested { get; set; }
    }
}
