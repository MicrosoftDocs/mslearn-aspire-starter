using eShop.Basket.API.Models;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace eShop.Basket.API.Storage;

public class MongoBasketStore
{
  private static IMongoCollection<CustomerBasket>? _basketCollection;

  public MongoBasketStore(IMongoClient mongoClient)
  {
    _basketCollection = mongoClient.GetDatabase("BasketDB").GetCollection<CustomerBasket>("basketitems")!;
  }

  public async Task<CustomerBasket?> GetBasketAsync(string customerId)
  {
    var filter = Builders<CustomerBasket>.Filter.Eq(r => r.BuyerId, customerId);

    return await _basketCollection!.Find(filter).FirstOrDefaultAsync();
  }

  public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
  {
    var filter = Builders<CustomerBasket>.Filter.Eq(r => r.BuyerId, basket.BuyerId);

    var result = await _basketCollection!.ReplaceOneAsync(filter, basket, new ReplaceOptions { IsUpsert = true });

    return result.IsModifiedCountAvailable == true ? basket : null;
  }
}
