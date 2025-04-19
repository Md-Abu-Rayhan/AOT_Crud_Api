using System.Text.Json.Serialization;

namespace AOTCrudApi.Models
{
    [JsonSerializable(typeof(Product))]
    [JsonSerializable(typeof(List<Product>))]
    [JsonSerializable(typeof(IEnumerable<Product>))]
    internal partial class AppJsonSerializationContext : JsonSerializerContext
    {
    }
}