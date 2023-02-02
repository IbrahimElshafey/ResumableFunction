using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.InOuts
{
    public class EngineSettings
    {
        public string ProviderName { get; set; }
        public string SqliteConnection { get; set; }
        public string SqlServerConnection { get; set; }
    }
}
