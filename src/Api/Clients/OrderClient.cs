using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Boukenken.Gdax
{
    public interface IOrderClient
    {
        Task<ApiResponse<IEnumerable<Order>>> GetOpenOrdersAsync();
        Task<ApiResponse<IEnumerable<Guid>>> CancelOpenOrdersAsync(string productId = null);
        Task<ApiResponse<Order>> PlaceOrderAsync(string side, string productId, decimal size, decimal price, string type, bool postOnly, string cancelAfter = null, string timeInForce = null);
    }

    public class OrderClient : GdaxClient
    {
        public OrderClient(string baseUrl, RequestAuthenticator authenticator)
            : base(baseUrl, authenticator)
        {
        }

        public async Task<ApiResponse<Order>> PlaceOrderAsync(string side, string productId, decimal size, decimal price, string type, bool postOnly = false, string cancelAfter = null, string timeInForce = null)
        {
            return await this.GetResponseAsync<Order>(
                new ApiRequest(HttpMethod.Post, "/orders", Serialize(new {
                    size = size,
                    side = side,
                    type = type,
                    price = price,
                    product_id = productId,
                    cancel_after = cancelAfter,
                    time_in_force = timeInForce,
                    post_only = postOnly
                }))
            );
        }

        public async Task<ApiResponse<IEnumerable<Order>>> GetOpenOrdersAsync()
        {
            return await this.GetResponseAsync<IEnumerable<Order>>(
                new ApiRequest(HttpMethod.Get, "/orders")
            );
        }

        public async Task<ApiResponse<Order>> GetOrderAsync(string orderId)
        {
            return await this.GetResponseAsync<Order>(
                new ApiRequest(HttpMethod.Get, $"/orders/{orderId}")
            );
        }


        public async Task<ApiResponse<IEnumerable<Guid>>> CancelOpenOrdersAsync(string productId = null)
        {
            return await this.GetResponseAsync<IEnumerable<Guid>>(
                new ApiRequest(HttpMethod.Delete, "/orders" + (productId == null ? "" : $"?product_id={productId}"))
            );
        }
    }
}