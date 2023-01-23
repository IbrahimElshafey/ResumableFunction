using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.InOuts
{
    public class FunctionFolder
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public DateTime LastScanDate { get; set; } = DateTime.MinValue;
        public List<string> NeedScanDlls { get; set; } = new List<string>();
        public bool NewUpdatesFound => LastScanDate > DateTime.Now;
    }

}
