using System.Collections.Generic;

namespace CIPLSharp
{
    public class CiplClass : ICiplCallable
    {
        public const string InitName = "init";
        
        public readonly string Name;
        private readonly Dictionary<string, CiplProcedure> methods;

        public CiplClass(string name, Dictionary<string, CiplProcedure> methods)
        {
            Name = name;

            this.methods = methods;
        }

        public override string ToString()
        {
            return $"<class {Name}>";
        }

        public int Arity()
        {
            var initializer = FindMethod(InitName);
            if (initializer == null) return 0;
            return initializer.Arity();
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var instance = new CiplInstance(this);
            var initializer = FindMethod(CiplClass.InitName);
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }
            
            
            return instance;
        }

        public CiplProcedure FindMethod(string name)
        {
            if (methods.TryGetValue(name, out var method))
                return method;

            return null;
        }
    }
}