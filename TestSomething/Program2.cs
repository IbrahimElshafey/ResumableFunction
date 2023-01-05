public partial class Program2
{
    static void Main(string[] args)
    {
        HelloFrom("Generated Code");
    }

    static partial void HelloFrom(string name);
    //static void Main()
    //{
    //    var t = new Test();
    //    Console.WriteLine(t.Sum(5, 6));
    //    t = new Test2();
    //    Console.WriteLine(t.Sum(5, 6));
    //    t = new Test() { SumFunc = (x, y) => (x + y) * 100 };
    //    Console.WriteLine(t.Sum(5, 6));

    //}

    public class test
    {
        
        public test()
        {
           
            //int x, y, z;
            //WaitEvent("1");
            //z = 10;
            //x = 10;
            //y = z * x;
            //WaitEvent("2");
            //y -= 50;
            //WaitEvent("3");
            //z -= 50;
            //Console.WriteLine(x+y+z);

        }

        private void WaitEvent(string v)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITest
    {
        int Sum(int x, int y);
    }

    public class Test : ITest
    {
        public delegate int SumFuncSiganture(int x, int y);
        public SumFuncSiganture? SumFunc { get; set; }
        public virtual int Sum(int x, int y)
        {
            if(SumFunc is not null)
                return SumFunc(x, y);

            Console.WriteLine("Base implementation");
            return x+ y;
        }
    }

    public class Test2 : Test
    {
        public override int Sum(int x, int y)
        {
            Console.WriteLine("Drived implementation");
            return base.Sum(x,y)*10;
        }
    }
}
