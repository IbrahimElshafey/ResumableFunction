using ResumableFunction.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.Abstraction
{
    public interface IEventProviderRepository
    {
        Task<IEventProvider> GetByName(string name);
        Task<bool> RegsiterEventProvider(IEventProvider eventProvider);
    }
}
