using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.InOuts;
using Example.ProjectApproval.InOuts;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;

namespace Example.ProjectApproval
{
    public class ProjectApprovalWorkflow : ResumableFunction<ProjectApprovalWorkflowData>
    {
        protected override async IAsyncEnumerable<EventWaitingResult> Start()
        {
            yield return
                WaitEvent<ProjectSumbitted>(Constant.ProjectSumbittedEvent)
                .Match(x => x.Project.Id > 0)
                .SetProp(() => Data.ProjectSumbitted);

            await AskManagerApproval(Data.ProjectSumbitted.Project.Id);
            yield return
                WaitEvent<ManagerApprovalEvent>(Constant.ManagerApprovalEvent)
                .Match(x => x.ProjectId == Data.ProjectSumbitted.Project.Id)
                .SetProp(() => Data.ManagerApprovalEvent);
            Console.WriteLine("Done");
        }

        private async Task AskManagerApproval(int id)
        {
            using (var client = new HttpClient())
                await client.PostAsync(
                    $"https://localhost:7241/api/project/AskManagerApproval?projectId={id}",null);
        }
    }
}
