using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;

namespace Demo.Kodez.Customers.Identity.Api.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var items = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                items.Add(item);
            }

            return items;
        }
    }
}