using ResumableFunction.Abstraction.WebApiProvider;

namespace Example.ProjectApproval
{
    public class ExampleApiEventProvider : WebApiEventProviderHandler
    {
        protected override string ApiUrl => "http://localhost:7241/";

        protected override string ApiProjectName => "Example.Api";
    }
}
