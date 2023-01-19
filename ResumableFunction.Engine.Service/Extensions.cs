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
    }
}
