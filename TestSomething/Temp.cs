using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
namespace TestSomething
{
   public class Program2
    {
        delegate void Method();

        public static void Main1()
        {
            DynamicMethod dynMeth = new DynamicMethod("foo",
            typeof(void), new Type[0], typeof(Program2));

            // the "easy" way
            //WriteHello(dynMeth.GetILGenerator());

            // the "hard" way
            WriteHelloViaBytes(dynMeth.GetDynamicILInfo());

            Method meth = (Method)dynMeth.CreateDelegate(typeof(Method));
            meth();
        }

        static void WriteHelloViaBytes(DynamicILInfo info)
        {
            var writeLineMethod =
            typeof(System.Console).GetMethod("WriteLine", new Type[] { typeof(string) });

            int writeLineToken = info.GetTokenFor(writeLineMethod.MethodHandle);
            int helloWorldToken = info.GetTokenFor("Hello World!");

            MemoryStream codeStream = new MemoryStream();
            BinaryWriter codeWriter = new BinaryWriter(codeStream);

            codeWriter.Write((byte)OpCodes.Ldstr.Value);
            codeWriter.Write(helloWorldToken);

            codeWriter.Write((byte)OpCodes.Call.Value);
            codeWriter.Write(writeLineToken);

            codeWriter.Write((byte)OpCodes.Ret.Value);

            codeWriter.Flush();

            info.SetCode(codeStream.ToArray(), 1);

            MemoryStream sigStream = new MemoryStream();
            BinaryWriter sigWriter = new BinaryWriter(sigStream);

            // Signature format is tricky, described in 23.3 of
            // ECMA 335; see also CorHdr.h of .NET SDK.
            // Takes some time to work out; it's not well
            // described, IMHO.

            // IMAGE_CEE_CS_CALLCONV_LOCAL_SIG from CorHdr.h
            sigWriter.Write((byte)0x7);
            // no of parameters (signatures follow return type, recursively)
            sigWriter.Write((byte)0);
            // return type; here I use ELEMENT_TYPE_VOID as in CorHdr.h
            sigWriter.Write((byte)1);

            sigWriter.Flush();

            info.SetLocalSignature(sigStream.ToArray());
        }

        static void WriteHello(ILGenerator cg)
        {
            cg.Emit(OpCodes.Ldstr, "Hello World!");
            cg.Emit(OpCodes.Call,
            typeof(System.Console).GetMethod("WriteLine",
            new Type[] { typeof(string) }));
            cg.Emit(OpCodes.Ret);
        }
    }
}
