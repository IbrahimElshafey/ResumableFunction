using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkflowInCode.ConsoleTest.WorkflowEngine
{
    public class BasicQuery<Data>: IQuery<Data>
    {
        public BasicQuery(string queryName,dynamic parameters)
        {

        }
        public Task<Data> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
