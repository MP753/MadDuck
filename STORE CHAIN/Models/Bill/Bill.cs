using System;
using System.Collections.Generic;

namespace STORE_CHAIN.Models.Bill
{
    public class Bill
    {
        public int Number { get; set; }
        public DateTimeOffset Date { get; set; }
        public Customer Customer { get; set; }
        public List<BoughtProduct> BoughtProducts { get; set; } = new List<BoughtProduct>();
    }
}
