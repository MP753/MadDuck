using System.Collections.Generic;
using STORE_CHAIN.Models.Bill;
using STORE_CHAIN.Models.Product;
using STORE_CHAIN.Models.Shop;

namespace STORE_CHAIN.Services.Contracts
{
    public interface IShopServices
    {
        Shop CreateShop(int shopId, ShopTypes type, string name);
        Shop AddProductsToShop(int shopId, List<Product> products, List<Shop> shops);
        Bill CreateBill(int shopId, List<BoughtProduct> boughtProducts, List<Shop> shops, Customer customer);
    }
}
