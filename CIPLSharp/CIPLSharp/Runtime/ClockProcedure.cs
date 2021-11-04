using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class ClockProcedure : NativeProcedure
    {
        public override int Arity() => 0;

        public override object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)System.Environment.TickCount64;
        }
    }
}