using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using ResumableFunction.Engine.InOuts;
using System.Reflection;

namespace ResumableFunction.Engine.Helpers
{
    public class TypeToStringConverter : ValueConverter<Type, string>
    {
        public TypeToStringConverter()
            : base(
                type => TypeToString(type),
                text => StringToType(text))
        {
        }

        private static Type StringToType(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var typeObject = JsonConvert.DeserializeObject<TypeInformation>(text);
            return Assembly.LoadFile(typeObject.AssemblyPath).GetType(typeObject.Name)!;
        }

        private static string TypeToString(Type type)
        {
            if (type == null) return null;
            var typeObject = new TypeInformation { Name = type.Name, AssemblyPath = type.Assembly.Location };
            return JsonConvert.SerializeObject(typeObject, Formatting.Indented);
        }
    }
}
