using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.Abstraction.Samples
{


    public class ProjectApprovalFunctionData
    {
        public ProjectRequestedEvent Project { get; set; }
        public ManagerApprovalEvent OwnerApprovalResult { get; set; }
        public ManagerApprovalEvent SponsorApprovalResult { get; set; }
        public ManagerApprovalEvent ManagerApprovalResult { get; set; }
    }
}
