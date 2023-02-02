using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace ResumableFunction.Engine.Helpers
{
    public class ObjectToJsonConverter : ValueConverter<object, string>
    {
        public ObjectToJsonConverter() : base(o => ObjectToJson(o), json => JsonToObject(json))
        {
        }

        private static object JsonToObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }

        private static string ObjectToJson(object o)
        {
            return JsonConvert.SerializeObject(o);
        }
    }
}
