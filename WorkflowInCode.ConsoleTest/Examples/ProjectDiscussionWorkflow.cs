using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkflowInCode.ConsoleTest.WorkflowEngine;

namespace WorkflowInCode.ConsoleTest.Examples
{
    public class ProjectDiscussionWorkflow_ContextData
    {
        public dynamic RequestBasicData { get; internal set; }
        public List<dynamic> Members { get; internal set; }
        public List<dynamic> Topics { get; internal set; }
        public dynamic CurrentTopic { get; internal set; }
        public List<dynamic> ChefApprovals { get; internal set; }
        public List<dynamic> MemberApprovals { get; internal set; }
        public int CurrentTopicVotedMembersCount { get; internal set; }
        public dynamic ChefFinalApproval { get; internal set; }
    }
    public class ProjectDiscussionWorkflow : WorkflowInstance<ProjectDiscussionWorkflow_ContextData>
    {
        public ProjectDiscussionWorkflow(IWorkflow<ProjectDiscussionWorkflow_ContextData> workflow) : base(workflow)
        {
            workflow.RegisterStartStep(
                stepAction: DiscussionStarted,
                stepEvent: new BasicEvent<dynamic>(nameof(DiscussionStarted)));

            workflow.RegisterStep(
               stepEvent: new BasicEvent<dynamic>(nameof(MemberTopicApproval)),
               stepAction: MemberTopicApproval,
               eventFilter: (contextData, eventData) =>
                contextData.RequestBasicData.RequestId == eventData.RequestId &&
                contextData.CurrentTopic.TopicId == eventData.TopicId);

            workflow.RegisterStep(
              stepEvent: new BasicEvent<dynamic>(nameof(ChefTopicApproval)),
              stepAction: ChefTopicApproval,
              eventFilter: (contextData, eventData) =>
               contextData.RequestBasicData.RequestId == eventData.RequestId &&
               contextData.CurrentTopic.TopicId == eventData.TopicId);

            workflow.RegisterStep(
             stepEvent: new BasicEvent<dynamic>(nameof(ChefFinalApproval)),
             stepAction: ChefFinalApproval,
             eventFilter: (contextData, eventData) =>
              contextData.RequestBasicData.RequestId == eventData.RequestId);
        }

        [WorkFlowStep]
        private async Task DiscussionStarted(dynamic requestBasicData)
        {
            ContextData.RequestBasicData = requestBasicData;
            await SaveState();//save state here if the next queries failes

            var members = new BasicQuery<dynamic>("GetDiscussionMembers", requestBasicData.RequestId).Execute();
            var topics = new BasicQuery<dynamic>("GetDiscussionTopics", requestBasicData.RequestId).Execute();
            await Task.WhenAll(members, topics);
            ContextData.Members = members.Result;
            ContextData.Topics = topics.Result;
            ContextData.CurrentTopic = ContextData.Topics.First();
            await SaveState();

            //send discussion invetation foreach member
            await SendInvetationsToMembers();
            //send invetation for the chef
            await SendInvetationToChef();
        }

        private async Task SendInvetationToChef()
        {
            await new BasicCommand("SendDiscussionRequestToChef",
                            new { ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            await Workflow.ExpectNextStep(ChefTopicApproval);
        }

        private async Task SendInvetationsToMembers()
        {
            foreach (var member in ContextData.Members)
            {

                await new BasicCommand("SendDiscussionRequestToMemeber",
                    new { member.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            await Workflow.ExpectNextStep(MemberTopicApproval);
        }

        [WorkFlowStep]
        private async Task MemberTopicApproval(dynamic memperApproval)
        {
            ContextData.MemberApprovals.Add(memperApproval);
            await SaveState();
            if (memperApproval.Accepted)
            {
                await new BasicCommand("MemberAcceptedTopic",
                    new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            else if (memperApproval.Rejected)
            {
                await new BasicCommand("MemberRejectedTopic",
                    new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            else if (memperApproval.RequestDataComplete)
            {
                await new BasicCommand("MemberRequestDataCompleteForTopic",
                  new { memperApproval.MemberId, ContextData.RequestBasicData.RequestId, ContextData.CurrentTopic.TopicId }).Initiate();
            }
            ContextData.CurrentTopicVotedMembersCount += 1;
            await SaveState();
            if (ContextData.CurrentTopicVotedMembersCount < ContextData.Members.Count)
                await Workflow.ExpectNextStep(MemberTopicApproval);
            else
                await DiscussNextTopicOrFinalApproval();
        }

        [WorkFlowStep]
        private async Task ChefTopicApproval(dynamic chefResponse)
        {
            ContextData.ChefApprovals.Add(chefResponse);
            await SaveState();
            if (chefResponse.SkipMembers)
            {
                await new BasicCommand("ChefSkippedMemebersDecesionsForTopic", ContextData.CurrentTopic.Id).Initiate();
            }
            else if (chefResponse.Rejected)
            {
                await new BasicCommand("ChefRejectedTopic", ContextData.CurrentTopic.TopicId).Initiate();
            }

            else if (chefResponse.Accepted)
            {
                await new BasicCommand("ChefAcceptedTopic", ContextData.CurrentTopic.TopicId).Initiate();
            }
            await DiscussNextTopicOrFinalApproval();
        }

        private async Task DiscussNextTopicOrFinalApproval()
        {
            var isLastTopic = ContextData.CurrentTopic == ContextData.Topics.Last();
            if (isLastTopic)
            {
                await new BasicCommand("TopicsDiscussionFinished", ContextData.CurrentTopic.Id).Initiate();
                await Workflow.ExpectNextStep(ChefFinalApproval);
            }
            else
            {
                var currentTopicIndex = ContextData.CurrentTopic.TopicIndex;
                ContextData.CurrentTopic = ContextData.Topics[currentTopicIndex + 1];
                ContextData.CurrentTopicVotedMembersCount = 0;
                await SaveState();
                await SendInvetationsToMembers();
                await SendInvetationToChef();
            }
        }

        [WorkFlowStep]
        private async Task ChefFinalApproval(dynamic chefResponse)
        {
            ContextData.ChefFinalApproval = chefResponse;
            await SaveState();
            if (chefResponse.Accepted)
            {
                await new BasicCommand("ChefAcceptedRequest", ContextData.RequestBasicData.RequestId).Initiate();
            }
            else if (chefResponse.Rejected)
            {
                await new BasicCommand("ChefRejectedRequest", ContextData.RequestBasicData.RequestId).Initiate();
            }
            else if (chefResponse.CompleteData)
            {
                await new BasicCommand("ChefRequestDataComplete", ContextData.RequestBasicData.RequestId).Initiate();
            }
            await Workflow.Ended();
        }
    }
}
