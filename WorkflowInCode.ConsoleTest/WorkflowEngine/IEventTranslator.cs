using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    /// <summary>
    /// Subscribe to one or more external events and based on some logic it will fire 
    /// internal events that feed to the engine events queue directly
    /// internal and external events can be used to trigger workflow step execution
    /// 
    ///     an example is when a translator subscribe to events (X,Y,Z) and publish event Z
    ///     when all three recived. this emulate Task.WhenAll(x,y,z)
    ///     
    /// 
    ///     another example is a translator subscribes to events (X,Y,Z) and publish 
    ///     event Z if any one recieved. this emulate Task.WhenAll(x,y,z)
    /// </summary>
    public interface IEventTranslator
    {
        Task WhenEventReceived();
    }
}
