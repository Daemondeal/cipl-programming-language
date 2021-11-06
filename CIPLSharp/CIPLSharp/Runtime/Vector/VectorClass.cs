using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class VectorClass : CiplClass
    {

        public VectorClass(string name) 
            : base(name, new()
            {
                {"get", new GetMethod()},
                {"push", new PushMethod()},
                {"set", new SetMethod()},
                {"len", new LengthMethod()}
            }) 
        {
            
        }

        public override int Arity()
        {
            return 0;
        }
        
        public override object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new VectorInstance(this);

            return instance;
        }
        
    }
}