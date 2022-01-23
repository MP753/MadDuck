namespace STORE_CHAIN.Models.Product
{
    public class Product
    { 
        public int Id { get; set; }
        public ProductTypes Type { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int QuantityInStock { get; set; }
    }
}
