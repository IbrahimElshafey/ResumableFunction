using System.Threading.Tasks;
using WorkflowInCode.Abstraction.Engine;
namespace WorkflowInCode.Abstraction.Samples
{
	public class Test
	{
		public async Task WhenAllTest()
		{
			var t1 = Task.Delay(1000);
			var t2 = Task.Delay(2000);
			var t3 = Task.Delay(3000);
			await Task.WhenAll(t1, t2, t3);
			await Task.Delay(4000);
		}
	}
}
