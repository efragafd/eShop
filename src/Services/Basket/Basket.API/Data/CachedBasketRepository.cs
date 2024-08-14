using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

/// <summary>
/// Cached implementation version of <b>IBasketRepository</b> that adds
/// cache logic to repository with proxy and decorator patterns
/// and Scrutor library to decorator pattern.
/// </summary>
/// <param name="repository"></param>
/// <param name="cache"></param>
public class CachedBasketRepository
    (IBasketRepository repository, IDistributedCache cache)
    : IBasketRepository
{

    /// <summary>
    /// Method that check if basket exists in cache before consult it in database
    /// in order to improve performance.
    /// If basket not exists in cache, this adds it.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Basket cached object</returns>
    public async Task<ShoppingCart> GetBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await cache.GetStringAsync(userName, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket)) return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
        var basket = await repository.GetBasketAsync(userName, cancellationToken);
        await cache.SetStringAsync(userName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    /// <summary>
    /// Add basket to database and to cache
    /// </summary>
    /// <param name="basket"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Basket cached object</returns>
    public async Task<ShoppingCart> StoreBasketAsync(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await repository.StoreBasketAsync(basket, cancellationToken);
        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket), cancellationToken);
        return basket;
    }

    /// <summary>
    /// Delete basket from database and from cache
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>True if deleted correctly, otherwise false</returns>
    public async Task<bool> DeleteBasketAsync(string userName, CancellationToken cancellationToken = default)
    {
        await repository.DeleteBasketAsync(userName, cancellationToken);
        await cache.RemoveAsync(userName, cancellationToken);
        return true;
    }
}
