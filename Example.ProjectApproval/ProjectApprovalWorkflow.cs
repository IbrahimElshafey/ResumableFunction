using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Example.InOuts;
using ResumableFunction.Abstraction;
using ResumableFunction.Abstraction.InOuts;

namespace Example.ProjectApproval
{
    public class ProjectApprovalWorkflow : ResumableFunctionInstance
    {
        public ProjectSumbitted ProjectSumbitted { get; internal set; }
        public ManagerApprovalEvent ManagerApprovalEvent { get; internal set; }
        public override async IAsyncEnumerable<Wait> Start()
        {
            yield return
                WaitEvent<ProjectSumbitted>(Constant.ProjectSumbittedEvent)
                .Match(x => x.Project.Id > 0)
                .SetData(() => ProjectSumbitted);

            await AskManagerApproval(ProjectSumbitted.Project.Id);
            yield return
                WaitEvent<ManagerApprovalEvent>(Constant.ManagerApprovalEvent)
                .Match(x => x.ProjectId == ProjectSumbitted.Project.Id)
                .SetData(() => ManagerApprovalEvent);

            if (ManagerApprovalEvent.Decision is true)
                Console.WriteLine("Done");
            else if (ManagerApprovalEvent.Decision is false)
                yield return GoBackAfter<ProjectSumbitted>();
        }

        private async Task AskManagerApproval(int id)
        {
            using (var client = new HttpClient())
                await client.PostAsync(
                    $"https://localhost:7241/api/project/AskManagerApproval?projectId={id}", null);
        }
    }
}
