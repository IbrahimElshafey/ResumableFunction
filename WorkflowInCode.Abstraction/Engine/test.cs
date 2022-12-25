using System.Threading.Tasks;

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

            //Path("Acceptance",Applicant.Start,Manager1.Accept,Manager2.Accept,Manager3.Accept)
            //Path("Rejection",AnyOf(Manager1.Reject,Manager2.Reject,Manager3.Reject))
            //any rework the process start again
            //Path("Rework",AnyOf(Manager1.Rework,Manager2.Rework,Manager3.Rework),Applicant.Start)

            //Custom rework path
            //Path("Manager2Rework",Manager2.Rework,Applicant.Start,Manager2.Any)

            //Parallel
            //Path("XPath",Start.Any,XProcess.Any)
            //Path("YPath",Start.Any,YProcess.Any)

            //Synchronization (Do D only after both B and C are completed)
            //Path("BPath",Start.Any,BProcess.Any)
            //Path("CPath",Start.Any,CProcess.Any)
            //PathCombine("BPath","CPath",D)

            //Exclusive Choice [after a, do b or c]
            //Path("AStart",Start,A)
            //Path("BPath",A.BNode,B.Any)
            //Path("CPath",A.CNode,C.Any)


            //Simple Merge [perform 3 after 1 or 2 finishes]
            //Path("1Path",1.Any,3.Any)
            //Path("2Path",2.Any,3.Any)

            //Pattern 6 (Multi-Choice)
            //Depending on the nature of the emergency call, one or more of the despatch-police, despatch-fire-engine and despatch-ambulance tasks is immediately initiated.
            //Path("despatch-police",Start.PoliceRequired,Police.Any)
            //Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.Any)
            //Path("despatch-fire",Start.FireRequired,Fire.Any)


            //Pattern 7 (Structured Synchronizing Merge)
            //Depending on the type of emergency, either or both of the despatch - police and despatch-ambulance tasks are initiated simultaneously.
            //When all emergency vehicles arrive at the accident, the transfer - patient task commences.
            //Path("despatch-police",Start.PoliceRequired,Police.Any)
            //Path("despatch-ambulance",Start.AmbulanceRequired,Ambulance.Any)
            //CombineActivePaths("despatch-police","despatch-ambulance",TransferPatient.Any)


            //Pattern 8 (Multi-Merge)
            //

        }
    }
}
