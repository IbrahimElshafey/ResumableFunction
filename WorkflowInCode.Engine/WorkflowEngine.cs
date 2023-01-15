using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.Abstraction;
using WorkflowInCode.Abstraction.InOuts;

namespace WorkflowInCode.Engine
{
    public class WorkflowEngine : IWorkflowEngine
    {
        public Task RegisterAssembly(string assemblyName)
        {
            /*
              *  ## Load Event Providers
              */
            /*
             * ## LoadWorkflows
            * find classes that inherit from WorkflowInstance
            * for each class check if active definition is alerady exist or not
            * create database table or collection for InstanceData type
            * 
            * Load Instance data and run workflow
            * subscribe to the first EventWaiting 
            */
            return Task.CompletedTask;
        }
    }
}
