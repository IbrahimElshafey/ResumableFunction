using Aq.ExpressionJsonSerializer;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using ResumableFunction.Abstraction.InOuts;
using System.Linq.Expressions;
using System.Reflection;

namespace ResumableFunction.Engine.Helpers
{
    public class ExpressionToJsonConverter : ValueConverter<Expression, string>
    {
        public ExpressionToJsonConverter()
            : base(
                expression => ExpressionToJson(expression),
                Json => JsonToExpression(Json))
        {
        }

        internal static string ExpressionToJson(Expression expression)
        {
            if (expression != null)
                return JsonConvert.SerializeObject(expression, JsonSettings());
            return null!;
        }
        internal static Expression JsonToExpression(string json)
        {
            if (!string.IsNullOrWhiteSpace(json))
                return JsonConvert.DeserializeObject<LambdaExpression>(json, JsonSettings())!;
            return null!;
        }

        private static JsonSerializerSettings JsonSettings()
        {
            var settings = new JsonSerializerSettings();
            //Todo:Replace Assembly.GetExecutingAssembly() with "GetCurrentFunctionAssembly()"
            settings.Converters.Add(
                new ExpressionJsonConverter(Extensions.GetCurrentFunctionAssembly()));
            return settings;
        }
    }
}
