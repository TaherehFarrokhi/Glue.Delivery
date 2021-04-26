using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Glue.Delivery.WebApi.IntegrationTests.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T> ValidateAndReadContentAsync<T>(this HttpResponseMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            
            message.EnsureSuccessStatusCode();
            
            var content = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}