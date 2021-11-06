using System.Collections.Generic;

namespace CIPLSharp
{
    public class CiplInstance
    {
        private CiplClass ciplClass;

        private readonly Dictionary<string, object> fields = new();

        public CiplInstance(CiplClass ciplClass)
        {
            this.ciplClass = ciplClass;
        }

        public object Get(Token name)
        {
            if (fields.TryGetValue(name.Lexeme, out var field))
                return field;

            var method = ciplClass.FindMethod(name.Lexeme);
            if (method is not null) return method.Bind(this);

            throw new RuntimeError(name, $"Undefined property `{name.Lexeme}` on instance of `{ciplClass.Name}`");
        }

        public override string ToString()
        {
            return $"[{ciplClass.Name} instance]";
        }

        public void Set(Token exprName, object val)
        {
            fields[exprName.Lexeme] = val;
        }
    }
}