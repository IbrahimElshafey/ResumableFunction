using System.Reflection;
using TestSomething.MayUse;
using ResumableFunction.Abstraction.Samples;
using ResumableFunction.Abstraction.WebApiProvider;
using ResumableFunction.Abstraction;
using ResumableFunction.Engine;

namespace Test
{
    public static class Program
    {

        static async Task Main(string[] args)
        {
            var type = Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .First(x => x.FullName == "ResumableFunction.Abstraction.Samples.ProjectApprovalWaitMany");
            Console.WriteLine(type.IsGenericType);
            Console.WriteLine(type.IsGenericTypeDefinition);
            var x = new FunctionWrapper(type);
            Console.WriteLine(x.InstanceId);
            Console.WriteLine(x.Data.GetType());
            //await TestWebApiEventProviderClient();
        }

        
        private static async Task TestWebApiEventProviderClient()
        {
            var x = new TestWebApiEventProvider();
            await x.Start();
        }

        ///To use Expression trees <see cref="PropertyManager.EnsurePropertySettersAndGettersForType"/> line 79 ( if (property.CanWrite))


        public class TestWebApiEventProvider : WebApiEventProviderHandler
        {
            protected override string ApiUrl => "https://localhost:7241/";

            protected override string ApiProjectName => "Example.Api";
        }
    }

    public class Test<T> where T : class, new()
    {
        public T Data { get; set; }
    }

    public class Data
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
