using System.Collections.Generic;

namespace CIPLSharp
{
    public interface ICiplCallable
    {
        // How many arguments it needs
        int Arity();
        
        object Call(Interpreter interpreter, List<object> arguments);
    }
}