using System.Collections.Generic;

namespace STORE_CHAIN.Models.Shop
{
    public class Shop
    {
        public int Id { get; set; }
        public ShopTypes Type { get; set; }
        public string Name { get; set; }
        public List<Product.Product> Products { get; set; }
        public List<Report> Reports { get; set; }
        //reset every new year
        public int CounterForBill { get; set; } = 1;
    }
}
