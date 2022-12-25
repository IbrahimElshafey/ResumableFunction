using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Engine
{
    public interface IProcess
    {
        //wake up command,output event,
        Task WakeUpCommand(IProcessInput input);
        IEvent<IProcessOutput> OutEvent {  get; }
        IProcessOutput Output {  get; }
        IProcessInput Input {  get; }

        ProcessOutputNode OutputNodes { get; }
    }

    public interface ProcessOutputNode
    {
        string Name { get; }
        void SetOutput(IProcessOutput processOutput);
        bool IsActive();
    }
}