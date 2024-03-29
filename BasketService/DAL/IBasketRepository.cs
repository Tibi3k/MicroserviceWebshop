﻿using BasketService.Model;

namespace BasketService.DAL
{
    public interface IBasketRepository
    {
        Task<List<UserBasket>> getAllBasketsAsync();
        UserBasket FindBasketById(string id);
        Task<UserBasket?> FindBasketByUserIdAsync(string id);
        Task AddProductToBasketAsync(Product product, string userId, string email);

        Task<long> DeleteProductFromBasketAsync(string userId, string productSubId);
        Task ClearBasket(string userId);
        Task<int> GetBasketSize(string userId);
        public Task<Product?> FindQuantityByBasketSubId(string productId, string userId);
    }
}
