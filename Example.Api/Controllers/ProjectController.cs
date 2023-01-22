using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;

namespace Example.Api.Controllers
{
    [ApiController]
    [Route("api/project")]
    [EnableEventProvider]
    public class ProjectController : ControllerBase
    {
        private Data _data;

        public ProjectController(Data data)
        {
            _data = data;
        }

        [HttpPost(nameof(SumbitProject))]
        public IActionResult SumbitProject(Project project)
        {
            _data.Projects.Add(project);
            return Ok(true);
        }

        [HttpPost(nameof(AskManagerApproval))]
        [DisableEventProvider]
        public IActionResult AskManagerApproval(int projectId)
        {
            var project = _data.Projects.FirstOrDefault(x => x.Id == projectId);
            if (project == null)
                return NotFound("Project not found.");
            _data.Tasks.Add(new UserTask(project.Id, "Manager"));
            return Ok(true);
        }

        [HttpPost(nameof(ManagerApproval))]
        public IActionResult ManagerApproval(int projectId, bool decision)
        {
            var task = _data.Tasks.FirstOrDefault(x => x.ProjectId == projectId && x.Owner == "Manager");
            if (task == null)
                return NotFound("Task not found.");
            task.Decision = decision;
            return Ok(true);
        }
    }

    public class Data
    {
        public List<Project> Projects { get; set; } = new List<Project>();
        public List<UserTask> Tasks { get; set; } = new List<UserTask>();
    }


}