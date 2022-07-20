//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WorkflowInCode.ConsoleTest.WorkflowEngine;

//namespace WorkflowInCode.ConsoleTest.Examples
//{
//    public class ProjectApprovalWorkflowStepsBased : WorkflowInstance<ProjectApprovalWorkflowData>
//    {

//        class ProjectApprovalCommands
//        {
//            internal const string InformProjectManagerAboutNewProject = "InformProjectManagerAboutNewProject";
//            internal const string PmoApprovalRequested = "PmoApprovalRequested";
//            internal const string OwnerApprovalRequested = "OwnerApprovalRequested";
//            internal const string SponsorApprovalRequested = "SponsorApprovalRequested";
//            internal const string InformProjectManagerAboutProjectRejected = "InformProjectManagerAboutProjectRejected";
//            internal const string ProjectAccepted = "ProjectAccepted";
//            internal const string ProjectManagerCancelProject = "ProjectManagerCancelProject";
//        }

//        public ProjectApprovalWorkflowStepsBased(IWorkflow<ProjectApprovalWorkflowData> workflow) : base(workflow)
//        {
//            workflow.RegisterStep(
//                stepEvent: new BasicEvent<dynamic>(nameof(ProjectAdded)),
//                stepAction: ProjectAdded);

//            workflow.RegisterStep(
//               stepEvent: new BasicEvent<dynamic>(nameof(PactAdded)),
//               stepAction: PactAdded,
//               eventFilter: (contextData, eventData) => contextData.ProjectData.Id == eventData.ProjectId);

//            workflow.RegisterStep(
//              stepEvent: new BasicEvent<dynamic>(nameof(PmoApproval)),
//              stepAction: PmoApproval,
//              eventFilter: (contextData, eventData) =>
//                contextData.ProjectData.Id == eventData.ProjectId &&
//                contextData.PmoApprovalData.Id == eventData.PactId);

//            workflow.RegisterStep(
//              stepEvent: new BasicEvent<dynamic>(nameof(OwnerApproval)),
//              stepAction: OwnerApproval,
//              eventFilter: (contextData, eventData) =>
//                contextData.ProjectData.Id == eventData.ProjectId);

//            workflow.RegisterStep(
//             stepEvent: new BasicEvent<dynamic>(nameof(SponsorApproval)),
//             stepAction: SponsorApproval,
//             eventFilter: (contextData, eventData) =>
//               contextData.ProjectData.Id == eventData.ProjectId);

//            workflow.RegisterStep(
//            stepEvent: new BasicEvent<dynamic>(nameof(ProjectManagerAfterRejectAction)),
//            stepAction: ProjectManagerAfterRejectAction,
//            eventFilter: (contextData, eventData) =>
//              contextData.ProjectData.Id == eventData.ProjectId);
//        }

//        private async Task ProjectAdded(dynamic projectData)
//        {
//            ContextData.ProjectData = projectData;
//            await SaveState();
//            //inform project manager that a project added and him assigned as a project manager
//            await new BasicCommand(ProjectApprovalCommands.InformProjectManagerAboutNewProject, new { projectData.Id, projectData.Manager }).Initiate();
//            await Workflow.ExpectNextStep(PactAdded);
//        }

//        private async Task PactAdded(dynamic pactData)
//        {
//            ContextData.PmoApprovalData = pactData;
//            await SaveState();
//            //command to inform PMO that a new project added and ask to approve
//            await new BasicCommand(ProjectApprovalCommands.PmoApprovalRequested, ContextData.ProjectData.Id).Initiate();
//            await Workflow.ExpectNextStep(PmoApproval);
//        }

//        private async Task PmoApproval(dynamic pmoApprovalData)
//        {
//            ContextData.PmoApprovalData = pmoApprovalData;
//            await SaveState();
//            if (pmoApprovalData.Accepted)
//            {
//                await new BasicCommand(ProjectApprovalCommands.OwnerApprovalRequested, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(OwnerApproval);
//            }
//            else if (pmoApprovalData.Rejected)
//            {
//                await new BasicCommand(ProjectApprovalCommands.InformProjectManagerAboutProjectRejected, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(ProjectManagerAfterRejectAction);
//            }
//        }

//        private async Task OwnerApproval(dynamic ownerApprovalData)
//        {
//            ContextData.OwnerApprovalData = ownerApprovalData;
//            await SaveState();
//            if (ownerApprovalData.Accepted)
//            {
//                await new BasicCommand(ProjectApprovalCommands.SponsorApprovalRequested, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(SponsorApproval);
//            }
//            else if (ownerApprovalData.Rejected)
//            {
//                await new BasicCommand(ProjectApprovalCommands.InformProjectManagerAboutProjectRejected, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(ProjectManagerAfterRejectAction);
//            }
//        }

//        private async Task SponsorApproval(dynamic sponsorApprovalData)
//        {
//            ContextData.SponsorApprovalData = sponsorApprovalData;
//            await SaveState();
//            if (sponsorApprovalData.Accepted)
//            {
//                await new BasicCommand(ProjectApprovalCommands.ProjectAccepted, ContextData.ProjectData.Id).Initiate();
//                await Workflow.End();
//            }
//            else if (sponsorApprovalData.Rejected)
//            {
//                await new BasicCommand(ProjectApprovalCommands.InformProjectManagerAboutProjectRejected, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(ProjectManagerAfterRejectAction);
//            }
//        }

//        private async Task ProjectManagerAfterRejectAction(dynamic projectManagerAfterRejectAction)
//        {
//            ContextData.ProjectManagerAfterRejectAction = projectManagerAfterRejectAction;
//            await SaveState();
//            if (projectManagerAfterRejectAction.Cancel)
//            {
//                await new BasicCommand(ProjectApprovalCommands.ProjectManagerCancelProject, ContextData.ProjectData.Id).Initiate();
//                await Workflow.End();
//            }
//            else if (projectManagerAfterRejectAction.Resend)
//            {
//                //back to specific postion in workflow
//                await new BasicCommand(ProjectApprovalCommands.PmoApprovalRequested, ContextData.ProjectData.Id).Initiate();
//                await Workflow.ExpectNextStep(PmoApproval);
//            }
//        }
//    }
//}
