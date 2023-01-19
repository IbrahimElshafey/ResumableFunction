using Refit;

namespace ResumableFunction.Abstraction.WebApiProvider
{
    public interface IWebApiEventProviderClient
    {
        [Get("/api/EventProvider/GetProviderName")]
        Task<string> GetProviderName();

        [Get("/api/EventProvider/Start")]
        Task Start();

        [Get("/api/EventProvider/Stop")]
        Task Stop();

        [Get("/api/EventProvider/SubscribeToApiAction?actionPath={actionPath}")]
        Task<bool> SubscribeToApiAction(string actionPath);

        [Get("/api/EventProvider/UnsubscribeApiAction?actionPath={actionPath}")]
        Task<bool> UnsubscribeApiAction(string actionPath);
    }
}
