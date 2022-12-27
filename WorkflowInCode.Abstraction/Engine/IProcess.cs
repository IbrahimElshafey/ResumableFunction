﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.Abstraction.Engine
{
    public interface IProcess<InputType, OutputType> : IProcess
    {
        new IProcess<InputType, OutputType> Fire();
        new ISubscribeEvent<OutputType> OutputEvent { get; }
        new OutputType Output { get; }
        new InputType Input { get; }
        new Dictionary<string, Func<OutputType, bool>> OutputNodes { get; }

    }

    public interface IProcess: IWorkflowProcessingUnit
    {
        IProcess WakeUp();
        IProcess WakeUp(IProcessInput input);
        IProcess WakeUp(object input);
        IProcess Cancel();
        ISubscribeEvent<IProcessOutput> OutputEvent {  get; }
        IProcessOutput Output {  get; }
        IProcessInput Input {  get; }

        Dictionary<string, ProcessOutputNode> OutputNodes { get; }

    }

    public interface IWorkflowProcessingUnit
    {

    }

    public interface ProcessOutputNode: IWorkflowProcessingUnit
    {

    }

}