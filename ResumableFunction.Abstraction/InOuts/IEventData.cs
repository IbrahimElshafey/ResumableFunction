using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction.InOuts
{
    /// <summary>
    /// Any event data must implement this interface.
    /// Any event data must have a constructor less parameters.
    /// </summary>
    public interface IEventData
    {
        string EventProviderName { get;}
        string EventIdentifier { get; }
    }

}
