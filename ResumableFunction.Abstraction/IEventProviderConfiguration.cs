using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction
{
    //Not used
    public interface IEventProviderConfiguration
    {
        /// <summary>
        /// Uniqe name to resolve this provider at runtime
        /// </summary>
        string EventProviderName { get; }
        bool IsRemoteService { get; }

        Type EventProviderType { get; }

        ConnectionType ConnectionType { get; }

        string ConnectionString { get; }
    }
    public enum ConnectionType
    {
        LoadObjectInstance,
        gRPC_HTTP,
        gRPC_TCP,
        gRPC_NamedPipe,
    }
}
