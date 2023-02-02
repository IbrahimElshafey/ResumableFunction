using Microsoft.EntityFrameworkCore;
using ResumableFunction.Abstraction.InOuts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Engine.InOuts
{
    public class FunctionFolder
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime LastScanDate { get; set; } = DateTime.MinValue;
        public List<TypeInfo> EventProviderTypes { get; set; }
        public DbSet<TypeInfo> FunctionInfos { get; set; }

        [NotMapped]
        public List<string> NeedScanDlls { get; set; } = new List<string>();
        public bool NewUpdatesFound => LastScanDate > DateTime.Now;
    }

}
