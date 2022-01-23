using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using STORE_CHAIN.Models.Bill;
using STORE_CHAIN.Models.Product;
using STORE_CHAIN.Models.Shop;
using STORE_CHAIN.Services.Contracts;

namespace STORE_CHAIN.Services
{
    public class ShopServices : IShopServices
    {
        private readonly ILogger<ShopServices> _logger;

        public ShopServices(ILogger<ShopServices> logger)
        {
            _logger = logger;
        }

        public Shop CreateShop(int shopId, ShopTypes type, string name)
        {
            return  new Shop
            {
                Id = shopId,
                Type = type,
                Name = name
            };
        }


        public Shop AddProductsToShop(int shopId, List<Product> products, List<Shop> shops)
        {
            var foundShop = GetShop(shopId, shops);
            if (foundShop == null) return null;
            var shopType = foundShop.Type;
            foreach (var product in products)
            {
                if ((product.Type == ProductTypes.Cigarettes && shopType != ShopTypes.CornerShop) ||
                    (product.Type == ProductTypes.Medicine && shopType != ShopTypes.Pharmacy))
                {
                    _logger.LogError($"Product type {product.Type} not belonging to shop {shopType}" );
                    continue;
                }
                foundShop.Products.Add(product);
            }
            return foundShop;
        }
        //if I had a time, i would refactor boughtProducts to be list of ProductId and Quantity/Amount,
        //and price and serial number would be extracted from  foundShop products
        public Bill CreateBill(int shopId, List<BoughtProduct> boughtProducts, List<Shop> shops, Customer customer)
        {
            var foundShop = GetShop(shopId, shops);
            if (foundShop == null) return null;
            if (!ValidateBillProducts(boughtProducts, foundShop, customer)) return null;
            var now = DateTimeOffset.UtcNow;
            var result = new Bill
            {
                Number = foundShop.CounterForBill,
                Date = now,
                Customer = customer,
                BoughtProducts = boughtProducts

            };
            foundShop.CounterForBill++;
            foreach (var product in boughtProducts)
            {
                var productInShop = foundShop.Products.FirstOrDefault(p => p.Id == product.ProductId);
                var newAmount = productInShop.QuantityInStock - product.Amount;
                _logger.LogInformation($"Bought product: {foundShop.Type},{productInShop.Type},{productInShop.Price},{productInShop.QuantityInStock},{newAmount},{now}");
                productInShop.QuantityInStock -= product.Amount;
            }

            return result;
        }

        private Shop GetShop(int shopId, List<Shop> shops)
        {
            var foundShop = shops.FirstOrDefault(x => x.Id == shopId);
            if (foundShop != null) return foundShop;
            _logger.LogError("Can't find shop with Id " + shopId);
            return null;
        }

        private bool ValidateBillProducts(List<BoughtProduct> boughtProducts, Shop shop, Customer customer)
        {
            var shopProducts = shop.Products;
            //seems like some of customer data should be required
            if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.LastName)) return false;
            foreach (var boughtProduct in boughtProducts)
            {
                var productExistInShop = shopProducts.FirstOrDefault(p => p.Id == boughtProduct.ProductId);
                if (productExistInShop == null)
                {
                    _logger.LogError($"Can't find product with id {boughtProduct.ProductId} in a shop with id {shop.Id}");
                    return false;
                }

                if (productExistInShop.QuantityInStock < boughtProduct.Amount)
                {
                    _logger.LogError($"Don't have enough products with id {boughtProduct.ProductId} in a shop stock with id {shop.Id}");
                    return false;
                }

                if ((productExistInShop.Type == ProductTypes.Medicine || productExistInShop.Type == ProductTypes.ParkingTickets) &&
                    string.IsNullOrEmpty(boughtProduct.SerialNumber))
                {
                    _logger.LogError($"SerialNumber missing for product with id {boughtProduct.ProductId}");
                    return false;
                }
            }

            return true;
        }
    }
}
