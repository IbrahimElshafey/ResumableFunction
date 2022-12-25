namespace WorkflowInCode.Abstraction.Engine
{
    public class test
    {
        private readonly IWorkflowEngine wf;

        public test(IWorkflowEngine wf)
        {
            this.wf = wf;
            ////Sequence
            //wf.Start("AcceptancePath", null, null).ThenProcess().ThenProcess().EndPath();
            //wf.DefinePath("RejectionPath", null, null).ThenProcess(null).EndPath();

            //Path("Acceptance",Applicant.Start,Manager1.Accept,Manager2.Accept,Manager3.Accept,WF.End)
            //Path("Rejection",AnyOf(Manager1.Reject,Manager2.Reject,Manager3.Reject),WF.End)
            //any rework the process start again
            //Path("Rework",AnyOf(Manager1.Rework,Manager2.Rework,Manager3.Rework),Applicant.Start)

            //Custom rework path
            //Path("Manager2Rework",Manager2.Rework,Applicant.Start,Manager2.Any)

            //Parallel
            //Path("XPath",Start,XProcess,End)
            //Path("YPath",Start,YProcess,End)

            //Synchronization (Do D only after both B and C are completed)
            //Path("BPath",Start,BProcess)
            //Path("CPath",Start,CProcess)
            //PathCombine("BPath","CPath",D,End)


        }
    }
}
