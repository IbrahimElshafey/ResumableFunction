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
            var typeObject = JsonConvert.DeserializeObject<SystemTypeClone>(text);
            var assembly = Assembly.LoadFile(typeObject.AssemblyPath);
            Extensions.SetCurrentFunctionAssembly(assembly);
            return assembly.GetType(typeObject.Name)!;
        }

        private static string TypeToString(Type type)
        {
            if (type == null) return null;
            var typeObject = new SystemTypeClone { Name = type.FullName, AssemblyPath = type.Assembly.Location };
            return JsonConvert.SerializeObject(typeObject, Formatting.Indented);
        }

        public class SystemTypeClone
        {
            public string Name { get; set; }
            public string AssemblyPath { get; set; }
        }
    }
}
