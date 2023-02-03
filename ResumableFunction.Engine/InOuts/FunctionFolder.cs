using System.ComponentModel.DataAnnotations.Schema;

namespace ResumableFunction.Engine.InOuts
{
    public class FunctionFolder
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public DateTime LastScanDate { get; set; } = DateTime.MinValue;
        public List<TypeInformation> EventProviderTypes { get; set; } = new List<TypeInformation>();
        public List<TypeInformation> FunctionInfos { get; set; } = new List<TypeInformation>();

        [NotMapped]
        public List<string> NeedScanDlls { get; set; } = new List<string>();
        public bool NewUpdatesFound => LastScanDate > DateTime.Now;
    }

}
