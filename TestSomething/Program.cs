using System.Formats.Asn1;
using System.IO.MemoryMappedFiles;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using TestSomething.MayUse;
using ResumableFunction.Abstraction.InOuts;
using ResumableFunction.Abstraction.Samples;
using ResumableFunction.Engine;
using ResumableFunction.Abstraction.WebApiProvider;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {

            var x = new TestWebApiEventProvider();
            await x.Start();
        }
        ///To use Expression trees <see cref="PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))
        private static void SetContextData(ProjectApprovalContextData FunctionData, string contextProp, object eventData)
        {

            var piInstance = FunctionData.GetType().GetProperty(contextProp);
            piInstance?.SetValue(FunctionData, eventData);
            //save data to database
        }

        public class TestWebApiEventProvider : WebApiEventProviderClient
        {
            protected override string ApiUrl => "https://localhost:7241/";

            protected override string ApiProjectName => "Example.Api";
        }
    }
}
