using ResumableFunction.Abstraction.InOuts;
using System.Text.Json;

namespace ResumableFunction.Engine.Service
{
    public static class Extensions
    {
        public static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T ToObject<T>(this PushedEvent element)
        {
            var json = JsonSerializer.Serialize(element);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
